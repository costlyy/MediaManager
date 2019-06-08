using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MediaManager.Core;
using MediaManager.Logging;
using MediaManager.VPN.SocketCommands;

namespace MediaManager.VPN
{
	public partial class VpnManager
	{
		private class VpnSocket : IDisposable, IVpnSocket
		{
			public enum SocketState
			{
				Idle,
				Connect,
				WaitingForCommands,
				Disconnect,
				Cleanup,
				Error,
			}

			private enum Signal
			{
				Hup,
				Term,
				Usr1,
				Usr2
			}

			private const int BUFFER_SIZE = 1024;
			private const string COMMAND_TERMINATED = "CMD:TERMINATED";

			private SocketState _state;
			public int State => (int) _state;

			public int Id { get; }

			private readonly SettingsData _settings;
			private Socket _vpnSocket;

			private string _activeError;
			private string _activeErrorDetail;

			#region CTOR & Public Interface

			public VpnSocket(int Id, SettingsData settingData)
			{

				this.Id = Id;
				_settings = settingData;
				LogWriter.Write($"VpnSocket # Constructed new VPN Socket ID: {Id}.");
			}

			public void Update()
			{
				try
				{
					switch (_state)
					{
						case SocketState.Idle:
							break;
						case SocketState.Connect:
							MaintainConnect();
							break;
						case SocketState.WaitingForCommands:
							MaintainWaitForCommands();
							break;
						case SocketState.Disconnect:
							MaintainDisconnect();
							break;
						case SocketState.Cleanup:
							MaintainCleanup();
							break;
						case SocketState.Error:
							break;
					}
				}
				catch (Exception exception)
				{
					LogWriter.Write($"VpnSocket # Caught a general exception in VpnSocket:\n{exception}");
					_activeError = "Caught a general exception in VpnSocket";
					_activeErrorDetail = exception.ToString();
					SetState(SocketState.Error);
				}
			}

			public bool Start()
			{
				if (_settings == null)
				{
					SetState(SocketState.Error);
					_activeError = "Socket was passed an empty SettingData object, cannot start.";
					_activeErrorDetail = "";
					return false;
				}

				if (GetState() != SocketState.Idle)
				{
					LogWriter.Write($"VpnSocket # Socket was dirty before attempted start, doing precautionary cleanup.");
					MaintainCleanup();
				}

				SetState(SocketState.Connect);
				return true;
			}

			public string GetError(ref string errorDetail)
			{
				LogWriter.Write($"VpnSocket # Returning error codes to caller system.");
				MaintainCleanup();
				SetState(SocketState.Idle);
				errorDetail = _activeErrorDetail;
				return _activeError;
			}

			public bool Disconnect()
			{
				if (GetState() == SocketState.Disconnect) return true;

				SetState(SocketState.Disconnect);
				return true;
			}

			public bool Destroy()
			{
				Disconnect();
				return true;
			}

			public void Dispose()
			{
				if (_vpnSocket != null)
				{
					//SendSignal(Signal.Term);
					_vpnSocket.Dispose();
				}
			}

			public VpnSocketResponse SendCommand(VpnSocketCommand command)
			{
				if (GetState() != SocketState.WaitingForCommands)
				{
					LogWriter.Write($"VpnSocket # Cannot run commands, socket is not ready!");
					return null;
				}

				try
				{
					_vpnSocket.Send(Encoding.Default.GetBytes(command.CommandText() + "\r\n"));

					var buffer = new byte[BUFFER_SIZE];
					var builder = new StringBuilder();

					while (true)
					{
						Thread.Sleep(100); // TODO: Is this going to freeze the GUI thread too much? maybe it needs to be thread pooled.

						_vpnSocket.ReceiveTimeout = 1000;

						// TODO: Lag here when the program stops responding.

						var incomingBuffer = _vpnSocket.Receive(buffer, 0, buffer.Length, 0);
						var message = Encoding.UTF8.GetString(buffer).Replace("\0", "");

						if (incomingBuffer < buffer.Length)
						{
							var parsedMessage = "";

							if (message.Contains("\r\nEND"))
							{
								parsedMessage = message.Substring(0, message.IndexOf("\r\nEND", StringComparison.Ordinal));
								builder.Append(parsedMessage);
							}
							else if (message.Contains("SUCCESS: "))
							{
								parsedMessage = message.Replace("SUCCESS: ", "").Replace("\r\n", "");
								builder.Append(parsedMessage);
							}
							else if (message.Contains("ERROR: "))
							{
								parsedMessage = message.Replace("ERROR: ", "").Replace("\r\n", "");
								throw new ArgumentException(parsedMessage);
							}
							else
							{
								LogWriter.Write($"VpnSocket # Unknown return message from VPN Socket command: {parsedMessage}");
							}

							break;
						}

						builder.Append(message);
					}

					VpnSocketResponse responseObj = new GetStateResponse(builder.ToString());
					return responseObj;

				}
				catch (SocketException exception)
				{
					LogWriter.Write($"VpnSocket # Socket exception caught, probable loss of socket connectivity.\n\n {exception}");
					//return COMMAND_TERMINATED;

					VpnSocketResponse responseObj = new GetStateResponse("CMD:TERMINATED");
					return responseObj;

				}
			}

			#endregion CTOR & Public Interface

			#region State Wrappers

			private void SetState(SocketState newState)
			{
				LogWriter.Write($"VpnSocket # Setting state from {_state} to {newState}.");
				_state = newState;
				HeartbeatManager.Instance.KeepAlive();
			}

			private SocketState GetState()
			{
				return _state;
			}

			#endregion

			#region VPN Socket connect to instance

			private void MaintainConnect()
			{
				LogWriter.Write($"VpnSocket # Loading VPN socket.");

				_vpnSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_vpnSocket.Connect(ConfigManager.LOCAL_HOST, ConfigManager.LOCAL_PORT);

				if (ConfirmConnection())
				{
					LogWriter.Write($"VpnSocket # Socket connection confirmed, moving to wait for commands.");
					SetState(SocketState.WaitingForCommands);
				}
				else
				{
					LogWriter.Write($"VpnSocket # Error! Could not establish socket connection.");
					SetState(SocketState.Error);
					_activeError = "Could not establish socket connection.";
				}
			}

			#endregion

			#region VPN Socket running / waiting for commands

			private void MaintainWaitForCommands()
			{
				// Do nothing... for now.
			}

			#endregion

			#region VPN Socket disconnect from instance

			private void MaintainDisconnect()
			{
				if (_vpnSocket != null)
				{
					//SendSignal(Signal.Term);
					_vpnSocket.Dispose();
				}

				LogWriter.Write($"VpnSocket # Disconnected socket, returning to idle.");
				SetState(SocketState.Idle);
			}

			#endregion
			
			#region VPN Socket clean up connection

			private void MaintainCleanup()
			{
				LogWriter.Write($"VpnSocket # Cleaning VPN Socket.");
			}

			#endregion

			private bool ConfirmConnection()
			{
				var tempBuffer = new byte[BUFFER_SIZE];
				int receiveBuffer = _vpnSocket.Receive(tempBuffer, 0, tempBuffer.Length, SocketFlags.None);
				return receiveBuffer >= 1;
			}
		}
	}
}
