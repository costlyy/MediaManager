using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Logging;
using MediaManager.VPN.SocketCommands;

namespace MediaManager.VPN
{
	public partial class VpnManager : IVpnManager
	{
		public const int BUTTON_STATE_VPN_OFF = 1;
		public const int BUTTON_STATE_VPN_RUNNING = 2;
		public const int BUTTON_STATE_VPN_RETRYING = 3;

		public const int MAX_DISCONNECT_ATTEMPTS = 100;
		public const int MAX_DESTROY_ATTEMPTS = 100;

		public enum VpnManagerState
		{
			LoadingConfigs = 0,
			Idle,
			Starting,
			LoadingProcess,
			LoadingSocket,
			VerifyConnection,
			Connected,
			Stopped,
			Disconnect,
			DisconnectAndKill,
			Terminated,
			Retrying,
			Error,
			Cleanup,
		}

		private string _currentError = "";
		private int _disconnectCounter = 0;
		private int _destroyCounter = 0;
		private Dictionary<int, IVpnComponent> _vpnComponents;
		private IVpnComponent _currentComponent;
		private IManager _configManager;
		private List<Control> _ownedControls;
		private SettingsData _userSettings;
		private List<VpnSocketCommand> _vpnCommandQueue;
		private List<VpnSocketResponse> _vpnResponseQueue;
		private Timer _errorHandler;

		private TimeSpan _connectSpan;
		private DateTime _connectTime;

		private Dictionary<int, List<string>> _errorMessages = new Dictionary<int, List<string>>();

		private VpnManagerState _state;
		public int State => (int)_state;

		#region CTOR & Public API

		#region Singleton
		private static VpnManager _instance;
		public static IVpnManager Instance => _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				LogWriter.Write("VpnManager # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new VpnManager();
		}
		#endregion

		private VpnManager()
		{
			_vpnComponents = new Dictionary<int, IVpnComponent>();

			_errorHandler = new Timer { Interval = 1000 };
			_errorHandler.Tick += (s, e) => ProcessErrors();
			_errorHandler.Start();
		}

		public void Update()
		{
			switch (_state)
			{
				case VpnManagerState.LoadingConfigs:
					ProcessLoadConfigs();
					break;
				case VpnManagerState.Idle:
					ProcessIdle();
					break;
				case VpnManagerState.Starting:
					ProcessStartManager();
					break;
				case VpnManagerState.LoadingProcess:
					ProcessLoadProcess();
					break;
				case VpnManagerState.LoadingSocket:
					ProcessLoadSocket();
					break;
				case VpnManagerState.VerifyConnection:
					ProcessVerifyConnection();
					break;
				case VpnManagerState.Connected:
					ProcessConnected();
					break;
				case VpnManagerState.Stopped:
					break;
				case VpnManagerState.DisconnectAndKill:
					ProcessDisconnect(true);
					break;
				case VpnManagerState.Disconnect:
					ProcessDisconnect();
					break;
				case VpnManagerState.Terminated:
					break;
				case VpnManagerState.Retrying:
					break;
				case VpnManagerState.Error:
					break;
				case VpnManagerState.Cleanup:
					CleanVpnManager();
					break;
			}

			foreach (var component in _vpnComponents)
			{
				component.Value.Update();
			}
		}

		private void ProcessErrors()
		{
			if (_errorMessages.Count <= 0)
			{
				return;
			}

			foreach (KeyValuePair<int, List<string>> errorMessage in _errorMessages)
			{
				if (errorMessage.Value.Count <= 0)
				{
					LogWriter.Write("Attempted to process errors for VPN component: " 
						+ errorMessage.Key + " but there were none.");

					continue;
				}

				LogWriter.Write("Start processing errors for VPN component: " + errorMessage.Key);

				foreach (var item in errorMessage.Value)
				{
					LogWriter.Write(item);
				}

				LogWriter.Write("End processing errors for VPN component: " + errorMessage.Key);
			}

			_errorMessages.Clear();
		}

		public bool Start()
		{
			if (_state == VpnManagerState.Error)
			{
				LogWriter.Write($"VpnManager # Moving to start from error state.");
			}

			if (_state != VpnManagerState.LoadingConfigs)
			{
				CleanVpnManager();
			}

			SetState(VpnManagerState.LoadingConfigs);
			return true;
		}

		public string GetError(ref string errorDetail)
		{
			if (_state != VpnManagerState.Error) return "";

			LogWriter.Write($"VpnManager # Returning active error to caller: {_currentError}.");

			SetState(VpnManagerState.LoadingConfigs);

			return _currentError;
		}

		public void SetControls(List<Control> controls)
		{
			if (controls == null)
			{
				// TODO: Handle error case
				return;
			}

			_ownedControls = controls;
		}

		public void SetSettings(ref SettingsData settings)
		{
			if (settings == null)
			{
				// TODO: Add error case.
				return;
			}

			_userSettings = settings;
		}

		public bool Disconnect(bool killProcess = false)
		{
			if (_state == VpnManagerState.Error) return false;

			SetState(killProcess ? VpnManagerState.DisconnectAndKill : VpnManagerState.Disconnect);
			return true;
		}

		public void Destroy()
		{
			ProcessDisconnect(true);
		}

		public bool Connect()
		{
			if (_state == VpnManagerState.Error) return false;

			SetState(VpnManagerState.Starting);
			return true;
		}

		public string GetUpTime()
		{
			string returnValue = "";

			switch (_state)
			{
				case VpnManagerState.Connected:

					var ticks = DateTime.Now.Ticks - _connectTime.Ticks;
					_connectSpan = TimeSpan.FromTicks(ticks);
					returnValue = _connectSpan.ToString(@"dd\:hh\:mm\:ss");
					break;

				default:
					returnValue = "00:00:00:00";
					break;
			}
			
			return returnValue;
		}

		public void QueueCommand(VpnSocketCommand command)
		{
			if (_vpnCommandQueue == null)
			{
				_vpnCommandQueue = new List<VpnSocketCommand>();
			}

			if (_vpnResponseQueue == null)
			{
				_vpnResponseQueue = new List<VpnSocketResponse>();
			}

			if (command == null)
			{
				return;
			}

			_vpnCommandQueue.Add(command);
		}

		public List<VpnSocketResponse> GetCommandResponses()
		{
			List<VpnSocketResponse> returnData = new List<VpnSocketResponse>();

			if (_vpnResponseQueue != null && _vpnResponseQueue.Count > 0)
			{
				foreach (VpnSocketResponse responseItem in _vpnResponseQueue)
				{
					returnData.Add(responseItem);
				}

				_vpnResponseQueue.Clear();

				return returnData;
			}

			return null;
		}

		#endregion

		#region State Wrappers

		private void SetState(VpnManagerState newState)
		{
			LogWriter.Write($"VpnManager # SetState from {_state} to {newState}.");
			_state = newState;
			HeartbeatManager.Instance.KeepAlive();
		}

		#endregion

		private bool DisconnectSocket(ref IVpnSocket socket)
		{
			if (socket == null)
			{
				return false;
			}

			switch ((VpnSocket.SocketState)socket.State)
			{
				case VpnSocket.SocketState.Idle:
					LogWriter.Write($"VpnManager # Disconnected VPN socket successfully.");
					return true;

				case VpnSocket.SocketState.WaitingForCommands:
					socket?.Disconnect();
					LogWriter.Write($"VpnManager # Disconnecting VPN socket, attempting to verify.");
					break;

				case VpnSocket.SocketState.Error:
					string error = "";
					socket.GetError(ref error);
					LogWriter.Write($"VpnManager # Encountered error while processing disconnect. Value: {error}");
					break;
			}

			return false;
		}

		private bool DestroyProcess(ref IVpnProcess process)
		{
			if (process == null)
			{
				return false;
			}

			switch ((VpnProcess.ProcessState)process.State)
			{
				case VpnProcess.ProcessState.Idle:
					LogWriter.Write($"VpnManager # Destroyed VPN process successfully.");
					return true;

				case VpnProcess.ProcessState.Running:
				case VpnProcess.ProcessState.Unresponsive:
					process.Destroy();
					LogWriter.Write($"VpnManager # Destroying VPN process, attempting to verify.");
					break;

				case VpnProcess.ProcessState.Error:
					string error = "";
					process.GetError(ref error);
					LogWriter.Write($"VpnManager # Encountered error while processing destroy. Value: {error}");
					break;
			}

			return false;
		}

		private void MaintainCommandQueue()
		{
			if (_vpnCommandQueue == null || _vpnCommandQueue.Count <= 0) return;

			if (_vpnResponseQueue == null)
			{
				_vpnResponseQueue = new List<VpnSocketResponse>();
			}

			_vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_SOCKET, out _currentComponent);

			IVpnSocket socket = _currentComponent as VpnSocket;

			if (socket == null) return;

			List<VpnSocketCommand> toRemoveCommands = new List<VpnSocketCommand>();

			foreach (VpnSocketCommand item in _vpnCommandQueue)
			{
				_vpnResponseQueue.Add(socket.SendCommand(item));
				toRemoveCommands.Add(item);
			}

			foreach (VpnSocketCommand toRemove in toRemoveCommands)
			{
				_vpnCommandQueue.Remove(toRemove);
			}
		}

		#region FSM Methods

		#region VPN Manager load Configs

		private void ProcessLoadConfigs()
		{
			if (_configManager == null)
			{
				_configManager = new ConfigManager();
				_configManager.SetControls(_ownedControls);
				_configManager.SetSettings(ref _userSettings);
				_configManager.Start();
			}

			_configManager.Update();

			if ((ConfigManager.ConfigManagerState)_configManager.State == ConfigManager.ConfigManagerState.MaintainConfigs)
			{
				LogWriter.Write($"VpnManager # Processed all configs and config data.");
				SetState(VpnManagerState.Idle);
			}
		}

		#endregion

		#region VPN Manager idle / wait for start listener.

		private void ProcessIdle()
		{
			_configManager.Update();
		}

		#endregion

		#region VPN Manager process initial trigger (start)

		private void ProcessStartManager()
		{
			LogWriter.Write($"VpnManager # Starting Vpn Manager!");
			SetState(VpnManagerState.LoadingProcess);
		}

		#endregion

		#region VPN Manager load process

		private void ProcessLoadProcess()
		{
			 _vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_PROCESS, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"VpnManager # Starting the OpenVPN process (initial).");
				_currentComponent = new VpnProcess(_userSettings);
				_currentComponent.Start();

				_vpnComponents.Add(VpnComponentTypes.COMPONENT_PROCESS, _currentComponent);
			}

			_currentComponent.Update();

			List<string> errorsList = new List<string>();

			switch ((VpnProcess.ProcessState)_currentComponent.State)
			{
				case VpnProcess.ProcessState.Idle:
					if (_currentComponent.Start())
					{
						LogWriter.Write($"VpnManager # Starting the OpenVPN process.");
					}
					break;

				case VpnProcess.ProcessState.Running:
					_currentComponent = null;
					LogWriter.Write($"VpnManager # OpenVPN process launched successfully, moving to socket management.");
					SetState(VpnManagerState.LoadingSocket);
					break;

				case VpnProcess.ProcessState.Unresponsive:
					LogWriter.Write($"VpnManager # OpenVPN process is unresponsive.");
					break;
				case VpnProcess.ProcessState.Error:

					string errorMsg = "", errorMegDetailed = "";
					errorMsg = _currentComponent.GetError(ref errorMegDetailed);

					if (errorMsg.Length > 0)
					{
						errorsList.Add(errorMsg);
						errorsList.Add(errorMegDetailed);
					}
					break;
			}

			if (errorsList.Count > 0) _errorMessages.Add(errorsList.GetHashCode(), errorsList);
		}

		#endregion

		#region VPN Manager load socket

		private void ProcessLoadSocket()
		{
			_vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_SOCKET, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"VpnManager # Creating new socket.");
				_currentComponent = new VpnSocket(VpnComponentTypes.COMPONENT_SOCKET, _userSettings);
				_currentComponent.Start();

				_vpnComponents.Add(VpnComponentTypes.COMPONENT_SOCKET, _currentComponent);
			}

			_currentComponent.Update();

			List<string> errorsList = new List<string>();

			switch ((VpnSocket.SocketState) _currentComponent.State)
			{
				case VpnSocket.SocketState.Idle:
					if (_currentComponent.Start())
					{
						LogWriter.Write($"VpnManager # Starting a socket connection.");
					}
					break;

				case VpnSocket.SocketState.Error:
					string errorMsg = "", errorMegDetailed = "";

					errorMsg = _currentComponent.GetError(ref errorMegDetailed);

					if (errorMsg.Length > 0)
					{
						errorsList.Add(errorMsg);
						errorsList.Add(errorMegDetailed);
					}
					break;

				default:
					LogWriter.Write($"VpnManager # Attempting to verify connection state.");
					SetState(VpnManagerState.VerifyConnection);
					break;
			}

			if (errorsList.Count > 0) _errorMessages.Add(errorsList.GetHashCode(), errorsList);
		}

		#endregion

		#region VPN Manager verify connection state

		private void ProcessVerifyConnection()
		{
			_vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_SOCKET, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"VpnManager # Attempted to verify empty socket, exiting.");
				SetState(VpnManagerState.Error);
				return;
			}

			_currentComponent.Update();

			List<string> errorsList = new List<string>();

			switch ((VpnSocket.SocketState)_currentComponent.State)
			{
				case VpnSocket.SocketState.WaitingForCommands:
					_currentComponent = null;
					LogWriter.Write($"VpnManager # Socket launched successfully and is waiting for commands.");
					_connectTime = DateTime.Now;
					SetState(VpnManagerState.Connected);
					break;

				case VpnSocket.SocketState.Error:
					string errorMsg = "", errorMegDetailed = "";

					errorMsg = _currentComponent.GetError(ref errorMegDetailed);

					if (errorMsg.Length > 0)
					{
						errorsList.Add(errorMsg);
						errorsList.Add(errorMegDetailed);
					}
					break;

				default:
					LogWriter.Write($"VpnManager # Still waiting for verification, state: {_state}");
					break;
			}

			if (errorsList.Count > 0) _errorMessages.Add(errorsList.GetHashCode(), errorsList);
		}

		#endregion

		#region VPN Manager process connected / main update

		private void ProcessConnected()
		{
			MaintainCommandQueue();
		}

		#endregion

		#region VPN Manager stop existing process

		private void ProcessDisconnect(bool killProcess = false)
		{
			if (_disconnectCounter < MAX_DISCONNECT_ATTEMPTS)
			{
				_vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_SOCKET, out _currentComponent);

				IVpnSocket socket = _currentComponent as VpnSocket;

				if (!DisconnectSocket(ref socket))
				{
					_disconnectCounter++;
					return;
				}	
			}
			
			if (_disconnectCounter >= MAX_DISCONNECT_ATTEMPTS)
			{
				LogWriter.Write($"VpnManager # Global disconnect counter exceeded parameters (value: {_disconnectCounter}, max: {MAX_DISCONNECT_ATTEMPTS}).");
			}
			
			if (!killProcess)
			{
				LogWriter.Write($"VpnManager # Successfully disconnected VPN socket, returning to idle as killProcess = false.");
				SetState(VpnManagerState.Idle);
				_disconnectCounter = 0;
				_destroyCounter = 0;
				return;
			}

			if (_destroyCounter < MAX_DESTROY_ATTEMPTS)
			{
				_vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_PROCESS, out _currentComponent);

				IVpnProcess process = _currentComponent as VpnProcess;

				if (!DestroyProcess(ref process))
				{
					_destroyCounter++;
					return;
				}
			}
			
			if (_destroyCounter >= MAX_DESTROY_ATTEMPTS)
			{ 
				LogWriter.Write($"VpnManager # Global destroy counter exceeded parameters (value: {_disconnectCounter}, max: {MAX_DESTROY_ATTEMPTS}).");
			}

			LogWriter.Write($"VpnManager # Successfully destroyed VPN process.");
			SetState(VpnManagerState.Idle);
		}

		#endregion

		#region VPN Manager cleanup

		private void CleanVpnManager()
		{
			LogWriter.Write($"VpnManager # Cleaned VpnManager, returning to idle.");
			SetState(VpnManagerState.Idle);
		}

		#endregion

		#endregion
	}
}
