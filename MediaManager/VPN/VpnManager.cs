using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
			InitControls = 0,
			LoadingConfigs,
			Idle,
			Starting,
			LoadingProcess,
			LoadingSocket,
			VerifyConnection,
			Connected,
			Stopped,
			Disconnect,
			DisconnectAndKill,
			Pause,
			Terminated,
			Retrying,
			Restart,
			Error,
			Cleanup,
		}

		private enum ControlButtons
		{
			Invalid,
			VpnToggle,
			VpnPause,
			NextConfig
		}

		private enum DisplayBoxes
		{
			Invalid,
			ActiveConfig,
			ExternalIp,
		}

		private enum DisplayLabels
		{
			Invalid,
			CountdownTimer
		}

		private string _currentError = "";
		private int _disconnectCounter;
		private int _destroyCounter;
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
		private Dictionary<ControlButtons, Button> _controlButtonsDictionary = new Dictionary<ControlButtons, Button>();
		private Dictionary<DisplayBoxes, TextBox> _displayBoxesDictionary = new Dictionary<DisplayBoxes, TextBox>();
		private Dictionary<DisplayLabels, Label> _displayLabelsDictionary = new Dictionary<DisplayLabels, Label>();

		private DateTime _ipUpdateTime;
		private int _additionalSeconds;

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
				case VpnManagerState.InitControls:
					ProcessInitControls();
					break;
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
				case VpnManagerState.Pause:
					ProcessPause();
					break;
				case VpnManagerState.Terminated:
					break;
				case VpnManagerState.Retrying:
					break;
				case VpnManagerState.Restart:
					ProcessRestart();
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

			UpdateExternalIp();
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

			SetState(VpnManagerState.InitControls);
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
			LogWriter.Write($"VpnManager # SetControls - Added {_ownedControls.Count} controls to manager.");
		}

		public void SetSettings(ref SettingsData settings)
		{
			if (settings == null)
			{
				// TODO: Add error case.
				return;
			}

			_userSettings = settings;
			LogWriter.Write($"VpnManager # SetSettings - Added setting config.");
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

		public void Restart()
		{
			if (_state == VpnManagerState.Error)
			{
				LogWriter.Write($"VpnManager # Moving to restart from error state.");
			}

			SetState(VpnManagerState.Restart);
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

		public bool IsConnected()
		{
			switch (_state)
			{
				case VpnManagerState.VerifyConnection:
				case VpnManagerState.Connected:
				case VpnManagerState.Retrying:
					return true;
				default:
					return false;
			}
		}

		public void ToggleEnabledState(bool enabled)
		{
			if (_ownedControls == null)
			{
				LogWriter.Write($"VpnManager # ToggleEnabledState - Owned controls are NULL, cannot toggle state.");
				return;
			}

			foreach (Control item in _ownedControls.Where(item => item != null))
			{
				item.Enabled = enabled;
			}

			LogWriter.Write($"VpnManager # ToggleEnabledState - Enabled: {enabled}");
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

		#region UI Actions / Control Processing

		private void ActionVpnToggle(object sender, EventArgs e)
		{
			LogWriter.Write($"VpnManager # ActionVpnToggle");

			switch (_state)
			{
				default:
					LogWriter.Write($"VpnManager # ActionVpnToggle - VPN manager in an unsuitable state for action, waiting...");
					break;

				case VpnManagerState.Idle:
				case VpnManagerState.Stopped:
				case VpnManagerState.Terminated:
					Connect();
					break;

				case VpnManagerState.Starting:
				case VpnManagerState.LoadingProcess:
				case VpnManagerState.LoadingSocket:
				case VpnManagerState.VerifyConnection:
				case VpnManagerState.Connected:
				case VpnManagerState.Retrying:
				case VpnManagerState.Restart:
				case VpnManagerState.Error:
					Disconnect(true);
					break;
			}
		}

		private void ActionVpnPause(object sender, EventArgs e)
		{
			LogWriter.Write($"VpnManager # ActionVpnPause");

			switch (_state)
			{
				default:
					LogWriter.Write($"VpnManager # ActionVpnToggle - VPN manager in an unsuitable state for action, waiting...");
					break;

				case VpnManagerState.Pause:
					Connect();
					break;

				case VpnManagerState.Connected:
					SetState(VpnManagerState.Pause);
					break;
			}
		}

		private void UpdateExternalIp()
		{
			int baseSec, rangeSec;

			switch ((HeartbeatManager.OperatingMode)HeartbeatManager.Instance.State)
			{
				default:
					return;

				case HeartbeatManager.OperatingMode.Active:
					baseSec = Config.MAIN_ACTIVE_LOCAL_IP_BASE;
					rangeSec = Config.MAIN_ACTIVE_LOCAL_IP_RANGE;
					break;

				case HeartbeatManager.OperatingMode.Idle:
					baseSec = Config.MAIN_IDLE_LOCAL_IP_BASE;
					rangeSec = Config.MAIN_IDLE_LOCAL_IP_RANGE;
					break;
			}

			DateTime target = _ipUpdateTime + TimeSpan.FromSeconds(baseSec + _additionalSeconds);
			TimeSpan timeSpanNow = DateTime.Now - target;

			if (_displayLabelsDictionary.TryGetValue(DisplayLabels.CountdownTimer, out Label countdownLabel))
			{
				countdownLabel.Text = Math.Abs(timeSpanNow.Seconds).ToString();
			}

			if (DateTime.Now <= target) return;

			_additionalSeconds = new Random().Next(0, rangeSec);
			_ipUpdateTime = DateTime.Now;

			try
			{
				string extIp = new WebClient().DownloadString("http://icanhazip.com");

				LogWriter.Write($"VpnManager # UpdateExternalIp - External IP: {extIp.Trim()}");

				if (_displayBoxesDictionary.TryGetValue(DisplayBoxes.ExternalIp, out TextBox ipBox))
				{
					ipBox.Text = extIp;
				}
			}
			catch (Exception ex)
			{
				LogWriter.Write($"VpnManager # UpdateExternalIp - Caught exception while getting external IP:\n{ex}");
			}
		}

		#endregion

		#region FSM Methods

		#region VPN Manager - Init Controls

		private Action<object, EventArgs> GetActionForButton(ControlButtons button)
		{
			switch (button)
			{
				case ControlButtons.VpnToggle: return ActionVpnToggle;
				case ControlButtons.VpnPause: return ActionVpnPause;
			}

			LogWriter.Write($"VpnManager # GetActionForButton - Unknown / unhandled button type: {button}");
			return null;
		}

		private bool GetControlIdFromName(string name, out ControlButtons control)
		{
			// Strip out the "btn" part of the control (if it exists) as the enum is more generic and doesn't use that prefix.
			if (name.Contains("btn"))
			{
				name = name.Substring(name.IndexOf("btn", StringComparison.Ordinal) + 3);
			}

			try
			{
				control = (ControlButtons)Enum.Parse(typeof(ControlButtons), name, true);
				return true;
			}
			catch
			{
				control = ControlButtons.Invalid;
			}
			return false;
		}

		private bool GetControlIdFromName(string name, out DisplayBoxes control)
		{
			// Strip out the "btn" part of the control (if it exists) as the enum is more generic and doesn't use that prefix.
			if (name.Contains("tbx"))
			{
				name = name.Substring(name.IndexOf("tbx", StringComparison.Ordinal) + 3);
			}

			try
			{
				control = (DisplayBoxes)Enum.Parse(typeof(DisplayBoxes), name, true);
				return true;
			}
			catch
			{
				control = DisplayBoxes.Invalid;
			}
			return false;
		}

		private bool GetControlIdFromName(string name, out DisplayLabels control)
		{
			// Strip out the "btn" part of the control (if it exists) as the enum is more generic and doesn't use that prefix.
			if (name.Contains("lbl"))
			{
				name = name.Substring(name.IndexOf("lbl", StringComparison.Ordinal) + 3);
			}

			try
			{
				control = (DisplayLabels)Enum.Parse(typeof(DisplayLabels), name, true);
				return true;
			}
			catch
			{
				control = DisplayLabels.Invalid;
			}
			return false;
		}

		private void ProcessInitControls()
		{
			if (_ownedControls == null)
			{
				LogWriter.Write($"VpnManager # ProcessInitControls - Waiting for controls to be assigned...");
				return;
			}

			foreach (Control item in _ownedControls.Where(control => control != null))
			{
				try
				{
					if (GetControlIdFromName(item.Name, out ControlButtons button))
					{ 
						item.Click += (s, e) => GetActionForButton(button).Invoke(s, e);
						_controlButtonsDictionary.Add(button, item as Button);
						continue;
					}

					if (GetControlIdFromName(item.Name, out DisplayBoxes box))
					{
						_displayBoxesDictionary.Add(box, item as TextBox);
						continue;
					}

					if (GetControlIdFromName(item.Name, out DisplayLabels label))
					{
						_displayLabelsDictionary.Add(label, item as Label);
						continue;
					}
				}
				catch (Exception e)
				{
					LogWriter.Write($"VpnManager # ProcessInitControls - Caught exception while processing control: {item.Name}, ex: {e}");
				}
			}

			SetState(VpnManagerState.LoadingConfigs);
		}

		#endregion

		#region VPN Manager - load Configs

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

		#region VPN Manager - idle / wait for start listener.

		private void ProcessIdle()
		{
			_configManager.Update();
		}

		#endregion

		#region VPN Manager - process initial trigger (start)

		private void ProcessStartManager()
		{
			LogWriter.Write($"VpnManager # Starting Vpn Manager!");
			SetState(VpnManagerState.LoadingProcess);
		}

		#endregion

		#region VPN Manager - load process

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

		#region VPN Manager - load socket

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

		#region VPN Manager - verify connection state

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

		#region VPN Manager - process connected / main update

		private void ProcessConnected()
		{
			// Run the status update command every tick.
			QueueCommand(new SocketCommands.VpnManager.GetStateCommand());

			// Push requested commands to the VPN software via the socket.
			MaintainCommandQueue();
		}

		#endregion

		#region VPN Manager - stop existing process

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

		#region VPN Manager - pause

		private void ProcessPause()
		{
			if (_disconnectCounter > 0) return;

			_vpnComponents.TryGetValue(VpnComponentTypes.COMPONENT_SOCKET, out _currentComponent);

			IVpnSocket socket = _currentComponent as VpnSocket;

			if (!DisconnectSocket(ref socket)) return;

			_disconnectCounter++;
			LogWriter.Write($"VpnManager # ProcessPause - Disconnected for pause.");
		}

		#endregion

		#region VPN Manager - Restart

		private void ProcessRestart()
		{
			
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
