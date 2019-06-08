using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Downloads;
using MediaManager.Logging;
using MediaManager.Properties;
using MediaManager.SABnzbd.SabCommands;
using MediaManager.VPN;

namespace MediaManager.SABnzbd
{
	public partial class SabManager : ISabManager
	{
		//public const int BUTTON_STATE_SAB_OFF = 1;
		//public const int BUTTON_STATE_SAB_RUNNING = 2;
		//public const int BUTTON_STATE_SAB_RETRYING = 3;

		public enum SabManagerState
		{
			InitControls,
			Idle,
			Starting,
			LoadingProcess,
			LoadingHttpClient,
			VerifyHttpConnection,
			Running,
			Stopped,
			StopProcess,
			Terminated,
			Retrying,
			Restart,
			Error,
			Cleanup,
		}

		private enum ControlButtons
		{
			Invalid,
			DownloadToggle,
			DownloadPause
		}

		public enum ClientPauseState
		{
			Unpaused,
			Pausing,
			Paused,
			Unpausing,
		}

		private bool _pauseSab;
		public ClientPauseState PauseState { get; private set; }

		private string _currentError = "";
		private Dictionary<int, ISabComponent> _sabComponents;
		private ISabComponent _currentComponent;
	    private List<Control> _ownedControls;
		private SettingsData _userSettings;
		private List<SabClientCommand> _sabCommandQueue;
		private Timer _errorHandler;

		private DateTime _bootTimer;
		private DateTime _restartTimer;
		private bool _bootTimerStarted;

		private TimeSpan _connectSpan;
		private DateTime _connectTime;

		private Dictionary<string, List<string>> _errorMessages = new Dictionary<string, List<string>>();
		private Dictionary<ControlButtons, Button> _controlButtonsDictionary;

		private SabManagerState _state;
		public int State => (int)_state;

	    private uint errorCounter;

		private const int BOOT_TIME_GRACE_SECONDS = 2;
		private const int RESTART_TIME_SECONDS = 15;

		#region CTOR & Public API

		#region Singleton
		private static SabManager _instance;
		public static ISabManager Instance => _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				LogWriter.Write("SabManager # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new SabManager();
		}
		#endregion

		private SabManager()
		{
			_sabComponents = new Dictionary<int, ISabComponent>();

			_errorHandler = new Timer { Interval = 1000 };
			_errorHandler.Tick += (s, e) => ProcessErrors();
			_errorHandler.Start();

			SetState(SabManagerState.InitControls);
		}

		private void ProcessErrors()
		{
			if (_errorMessages.Count <= 0)
			{
				return;
			}

			foreach (KeyValuePair<string, List<string>> errorMessage in _errorMessages)
			{
				if (errorMessage.Value.Count <= 0)
				{
					LogWriter.Write("Attempted to process errors for SAB component: "
						+ errorMessage.Key + " but there were none.");

					continue;
				}

				LogWriter.Write("Start processing errors for SAB component: " + errorMessage.Key);

				foreach (var item in errorMessage.Value)
				{
					LogWriter.Write(item);
				}

				LogWriter.Write("End processing errors for SAB component: " + errorMessage.Key);
			}

			_errorMessages.Clear();
		}

		public bool Start()
		{
			if (_state == SabManagerState.Error)
			{
				LogWriter.Write($"SabManager # Moving to start from error state.");
			}

			if (_state != SabManagerState.Idle)
			{
				ProcessCleanSabManager();
			}

			SetState(SabManagerState.Starting);
			return true;
		}

		public string GetError(ref string errorDetail)
		{
			if (_state != SabManagerState.Error) return "";

			LogWriter.Write($"SabManager # Returning active error to caller: {_currentError}.");

			SetState(SabManagerState.Idle);

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
			LogWriter.Write($"SabManager # SetControls - Added {_ownedControls.Count} controls to manager.");
		}

		public void SetSettings(ref SettingsData settings)
		{
			if (settings == null)
			{
				// TODO: Add error case.
				return;
			}

			_userSettings = settings;
			LogWriter.Write($"SabManager # SetSettings - Added setting config.");
		}

		public bool StopProcess()
		{
			if (_state == SabManagerState.Error) return false;

			LogWriter.Write($"SabManager # Stopping process, callstack: ");
			LogWriter.PrintCallstack();

			SetState(SabManagerState.StopProcess);
			return true;
		}

		public bool StartProcess()
		{
			if (_state == SabManagerState.Error) return false;

			if (!CheckRunValidity())
			{
				LogWriter.Write($"SabManager # Failed to start process, environment is not ready.");
				return false;
			}

			LogWriter.Write($"SabManager # Starting process, callstack: ");
			LogWriter.PrintCallstack();

			SetState(SabManagerState.Starting);
			return true;
		}

		public void QueueCommand(SabClientCommand command)
		{
			if (_sabCommandQueue == null)
			{
				_sabCommandQueue = new List<SabClientCommand>();
			}

			if (command == null)
			{
				return;
			}

			_sabCommandQueue.Add(command);
		}

		public SabHttpData GetClientData()
		{
		    if (_state != SabManagerState.Running) return null;

			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_HTTP_CLIENT, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"SabManager # Could not get client data, client is null!", DebugPriority.Low);
				return null;
			}

			ISabHttpClient httpClient = _currentComponent as SabHttpClient;
			return httpClient?.GetClientData();
		}

		public void KillProcess()
		{
			ProcessStopProcess(true);
		}

		public string GetUpTime()
		{
			string returnValue = "";

			switch (_state)
			{
				case SabManagerState.Running:
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

		public void Restart()
		{
			if (_state == SabManagerState.Error)
			{
				LogWriter.Write($"SabManager # Moving to restart from error state.");
			}

			SetState(SabManagerState.Restart);
		}

		public bool IsConnected()
		{
			switch (_state)
			{
				case SabManagerState.Running:
					return true;
				default:
					return false;
			}
		}

		public void ToggleEnabledState(bool enabled)
		{
			if (_ownedControls == null)
			{
				LogWriter.Write($"SabManager # ToggleEnabledState - _ownedControls are NULL, cannot toggle enabled state.");
				return;
			}

			foreach (Control item in _ownedControls.Where(item => item != null))
			{
				item.Enabled = enabled;
			}

			LogWriter.Write($"SabManager # ToggleEnabledState - Enabled: {enabled}");
		}

		#endregion

		public void Update()
		{
			switch (_state)
			{
				case SabManagerState.InitControls:
					ProcessInitControls();
					break;
				case SabManagerState.Idle:
					ProcessIdle();
					break;
				case SabManagerState.Starting:
					ProcessStartManager();
					break;
				case SabManagerState.LoadingProcess:
					ProcessLoadProcess();
					break;
				case SabManagerState.LoadingHttpClient:
					ProcessLoadHttpClient();
					break;
				case SabManagerState.VerifyHttpConnection:
					ProcessVerifyConnection();
					break;
				case SabManagerState.Running:
					ProcessConnected();
					break;
				case SabManagerState.Stopped:
					break;
				case SabManagerState.StopProcess:
					ProcessStopProcess();
					break;
				case SabManagerState.Terminated:
					break;
				case SabManagerState.Retrying:
					break;
				case SabManagerState.Restart:
					ProcessRestart();
					break;
				case SabManagerState.Error:
					break;
				case SabManagerState.Cleanup:
					ProcessCleanSabManager();
					break;
			}

			foreach (var component in _sabComponents)
			{
				component.Value.Update();
			}
		}

		private void SetState(SabManagerState newState)
		{
			LogWriter.Write($"SabManager # SetState from {_state} to {newState}.");
			_state = newState;
			HeartbeatManager.Instance.KeepAlive();
		}

	    private void ProcessError(ISabComponent component)
	    {
	        if (component == null)
	            return;

	        List<string> errorsList = new List<string>();

            string errorMsg = "", errorMegDetailed = "";
	        errorMsg = component.GetError(ref errorMegDetailed);

		    if (errorMsg == null) return;

	        if (errorMsg.Length > 0)
	        {
                var sb = new StringBuilder();
	            sb.Append(errorMsg);

	            if (!string.IsNullOrEmpty(errorMegDetailed))
	            {
	                sb.Append(" FULL_ERRROR: ");
	                sb.Append(errorMegDetailed);
                }

                errorsList.Add(sb.ToString());
	        }

	        if (errorsList.Count > 0)
	        {
	            _errorMessages.Add($"#{errorCounter} - {component}", errorsList);
	            errorCounter++;
	        }
	    }

		#region UI Actions / Control Processing

		private void SetPauseState(ClientPauseState state)
		{
			LogWriter.Write($"SabManager # SetPauseState - From {PauseState} to {state}");
			PauseState = state;
		}

		private void ProcessPauseState(SabHttpData data)
		{
			if (data?.StatusData == null) return;

			switch (PauseState)
			{
				case ClientPauseState.Unpaused:

					if (!data.Paused)
					{
						if (_pauseSab)
						{
							QueueCommand(new SabCommands.SabManager.SetPaused());
							LogWriter.Write("SabManager # ProcessPauseState - Sent pause request to SABnzbd.");
							SetPauseState(ClientPauseState.Pausing);
						}
					}
					else
					{
						LogWriter.Write("SabManager # ProcessPauseState - Sent pause request to SABnzbd, but it is already paused.");
						SetPauseState(ClientPauseState.Paused);
					}

					break;

				case ClientPauseState.Pausing:

					if (data.Paused)
					{
						LogWriter.Write("SabManager # ProcessPauseState - Send pause cmd finished, moving to paused.");
						SetPauseState(ClientPauseState.Paused);
					}

					break;

				case ClientPauseState.Paused:

					if (!data.Paused)
					{
						QueueCommand(new SabCommands.SabManager.SetUnPaused());
						LogWriter.Write("SabManager # ProcessPauseState - Warning! SABnzbd un-paused without direction! What happened?", DebugPriority.High);
						SetPauseState(ClientPauseState.Unpaused);
					}

					if (!_pauseSab)
					{
						QueueCommand(new SabCommands.SabManager.SetUnPaused());
						LogWriter.Write("SabManager # ProcessPauseState - Sent un-pause request to SABnzbd.");
						SetPauseState(ClientPauseState.Unpausing);
					}

					break;

				case ClientPauseState.Unpausing:

					if (!data.Paused)
					{
						LogWriter.Write("SabManager # ProcessPauseState - Send un-pause cmd finished, moving to un-paused.");
						SetPauseState(ClientPauseState.Unpaused);
					}

					break;
			}
		}

		private void ActionDownloadToggle(object sender, EventArgs e)
		{
			LogWriter.Write($"SabManager # ActionDownloadToggle");

			if (!(sender is Button toggleButton))
			{
				LogWriter.Write($"SabManager # ActionDownloadToggle - Failed to cast sender to button, cannot process toggle request.", DebugPriority.High, true);
				return;
			}

			string error = null;

			switch (_state)
			{
				case SabManagerState.Idle:
				case SabManagerState.Stopped:

					if (!StartProcess())
					{
						LogWriter.Write("SabManager # ActionDownloadToggle - Failed to start SABnzbd client.");
						return;
					}

					toggleButton.Image = Resources.icon_stop_mid;
					LogWriter.Write("SabManager # ActionDownloadToggle - Started SabNZBD client.");

					break;

				case SabManagerState.Running:

					if (!StopProcess())
					{
						LogWriter.Write(
							"SabManager # ActionDownloadToggle - Failed to stop process, attempting to process errors.");

						GetError(ref error);

						if (!StopProcess())
						{
							LogWriter.Write(
								"SabManager # ActionDownloadToggle - Critical Error! Processed errors but still could not stop.",
								DebugPriority.High, true);
							break;
						}

						LogWriter.Write($"SabManager # ActionDownloadToggle - Got error: {error}");
					}

					DownloadManager.Instance.ClearAll();

					toggleButton.Image = Resources.icon_play_mid;

					LogWriter.Write("SabManager # ActionDownloadToggle - Stopped SABnzbd client.");

					break;

				case SabManagerState.Error:

					GetError(ref error);
					StopProcess();
					DownloadManager.Instance.ClearAll();

					toggleButton.Image = Resources.icon_play_mid;

					LogWriter.Write(
						$"SabManager # ActionDownloadToggle - Reset error locked SABnzbd client ({error}).");

					break;
			}
		}

		private void ActionDownloadPause(object sender, EventArgs e)
		{
			_pauseSab = !_pauseSab;
			LogWriter.Write($"SabManager # ActionDownloadPause - _pauseSab: {_pauseSab}");

			if (!(sender is Button toggleButton))
			{
				LogWriter.Write($"SabManager # ActionDownloadPause - Failed to cast sender to button");
				return;
			}

			toggleButton.BackColor = _pauseSab ? Color.FromArgb(64, 64, 64) : Color.FromArgb(28, 28, 28);
		}

		#endregion

		#region FSM - Init Controls

		private Action<object, EventArgs> GetActionForButton(ControlButtons button)
	    {
		    switch (button)
		    {
			    case ControlButtons.DownloadToggle: return ActionDownloadToggle;
			    case ControlButtons.DownloadPause: return ActionDownloadPause;
		    }

		    LogWriter.Write($"SabManager # GetActionForButton - Unknown / unhandled button type: {button}");
		    return null;
	    }

		private bool GetControlFromName(string buttonName, out ControlButtons button)
		{
			// Strip out the "btn" part of the control (if it exists) as the enum is more generic and doesn't use that prefix.
			if (buttonName.Contains("btn"))
			{
				buttonName = buttonName.Substring(buttonName.IndexOf("btn", StringComparison.Ordinal) + 3);
			}

			try
			{
				button = (ControlButtons)Enum.Parse(typeof(ControlButtons), buttonName, true);
				return true;
			}
			catch
			{
				button = ControlButtons.Invalid;
			}
			return false;
		}

		private void ProcessInitControls()
	    {
		    if (_ownedControls == null)
		    {
			    LogWriter.Write($"SabManager # ProcessInitControls - Waiting for controls to be assigned...");
			    return;
		    }

		    _controlButtonsDictionary = new Dictionary<ControlButtons, Button>();

		    foreach (Control item in _ownedControls.Where(control => control != null))
		    {
			    try
			    {
				    if (!GetControlFromName(item.Name, out ControlButtons button)) continue;

					item.Click += (s, e) => GetActionForButton(button).Invoke(s, e);

					_controlButtonsDictionary.Add(button, item as Button);
				}
			    catch (Exception e)
			    {
				    LogWriter.Write($"SabManager # ProcessInitControls - Caught exception while processing control: {item.Name}, ex: {e}");
			    }
		    }

			SetState(SabManagerState.Idle);
		}

	    #endregion 

		#region FSM - Idle / Wait for start listener.

		private void ProcessIdle()
		{
			// Do nothing, for now.
		}

		private void ProcessStartManager()
		{
			LogWriter.Write($"SabManager # Starting Sab Manager!");
			SetState(SabManagerState.LoadingProcess);
		}

		#endregion FSM - Idle / Wait for start listener.

		#region FSM - Load Process

		private void ProcessLoadProcess()
		{
			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_PROCESS, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"SabManager # Starting the SABnzbd process (initial).");
				_currentComponent = new SabProcess(_userSettings);
				_currentComponent.Start();

				_sabComponents.Add(SabComponentTypes.COMPONENT_PROCESS, _currentComponent);
			}

			_currentComponent.Update();

			switch ((SabProcess.ProcessState)_currentComponent.State)
			{
				case SabProcess.ProcessState.Idle:
					if (_currentComponent.Start())
					{
						LogWriter.Write($"SabManager # Starting the SABnzbd process.");
					}
					break;

				case SabProcess.ProcessState.Running:
					if (!_bootTimerStarted)
					{
						_bootTimerStarted = true;
						_bootTimer = DateTime.Now;
						break;
					}

					if (DateTime.Now <= _bootTimer + TimeSpan.FromSeconds(BOOT_TIME_GRACE_SECONDS)) return;

					List<Process> myProcesses = new List<Process>(Process.GetProcesses());
					bool foundProcess = false;

					foreach (Process item in myProcesses.Where(item => item != null))
					{
						if (!string.Equals(item.ProcessName, "sabnzbd", StringComparison.InvariantCultureIgnoreCase)) continue;

						LogWriter.Write($"SabManager # Found active SABnzbd process: {item.Id}.");
						foundProcess = true;
					}

					if (!foundProcess)
					{
						LogWriter.Write($"SabManager # Could not find sab process after launch (and grace period).");
						ProcessError(_currentComponent);
						_currentComponent.Stop();
						SetState(SabManagerState.Error);
						break;
					}

					_currentComponent = null;
					LogWriter.Write($"SabManager # Process launched successfully, moving to socket management.");
					SetState(SabManagerState.LoadingHttpClient);
					
					
					break;

				case SabProcess.ProcessState.Unresponsive:
					LogWriter.Write($"SabManager # SABnzbd process is unresponsive.");
					break;

				case SabProcess.ProcessState.Error:
				    ProcessError(_currentComponent);
				    _currentComponent.Stop();
				    SetState(SabManagerState.Error);
                    break;
			}
		}

		#endregion

		#region FSM - Load HTTP Client

		private void ProcessLoadHttpClient()
		{
			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_HTTP_CLIENT, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"SabManager # Creating new HttpClient.");
				_currentComponent = new SabHttpClient(SabComponentTypes.COMPONENT_HTTP_CLIENT, _userSettings);
				_currentComponent.Start();

				_sabComponents.Add(SabComponentTypes.COMPONENT_HTTP_CLIENT, _currentComponent);
			}

			_currentComponent.Update();

			switch ((SabHttpClient.ClientState) _currentComponent.State)
			{
				case SabHttpClient.ClientState.Idle:
					if (_currentComponent.Start())
					{
						LogWriter.Write($"SabManager # Starting a HttpClient connection.");
					}
					break;

				case SabHttpClient.ClientState.WaitingForCommands:
					_currentComponent = null;
					LogWriter.Write($"SabManager # HttpClient loaded successfully, moving to verify connectivity.");
					SetState(SabManagerState.VerifyHttpConnection);
					break;

				case SabHttpClient.ClientState.Error:
				    ProcessError(_currentComponent);
					break;
			}
		}

		#endregion

		#region FSM - Process connection verification

		private void ProcessVerifyConnection()
		{
			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_HTTP_CLIENT, out _currentComponent);

			if (_currentComponent == null)
			{
				LogWriter.Write($"SabManager # No HTTP client exists, cannot verify connection.");
				SetState(SabManagerState.Error);
				return;
			}

			_currentComponent.Update();

			switch ((SabHttpClient.ClientState)_currentComponent.State)
			{
				case SabHttpClient.ClientState.WaitingForCommands:
					_currentComponent = null;
					LogWriter.Write($"SabManager # HTTP client connected and waiting for commands.");
					_connectTime = DateTime.Now;
					SetState(SabManagerState.Running);
					break;

				case SabHttpClient.ClientState.Idle:
				case SabHttpClient.ClientState.Error:
					ProcessError(_currentComponent);
					SetState(SabManagerState.Error);
					break;
			}
		}

		#endregion

		#region FSM - Maintain connection / process commands

		private void ProcessConnected()
		{
			if (!CheckRunValidity())
			{
				return;
			}

			SabHttpData clientData = GetClientData();

			if (clientData != null)
			{
				ProcessPauseState(clientData);
			}

			MaintainCommandQueue();
		}

		private bool CheckRunValidity()
		{
			if (!Config.CORE_DOWNLOADER_REQUIRES_VPN) return true;

			bool valid = true;

			if (VpnManager.Instance == null)
			{
				LogWriter.Write($"SabManager # CheckRunValidity - Failed validity check: VpnManager.Instance == null");
				valid = false;
			}

			if ((VpnManager.VpnManagerState) VpnManager.Instance.State != VpnManager.VpnManagerState.Connected)
			{
				LogWriter.Write($"SabManager # CheckRunValidity - Failed validity check: VPN State is NOT Connected (state: {(VpnManager.VpnManagerState)VpnManager.Instance.State})", DebugPriority.High, true);
				valid = false;
			}

			if (!valid && _state != SabManagerState.Idle)
			{
				StopProcess();
			}

			return valid;
		}

		private void MaintainCommandQueue()
		{
			if (_sabCommandQueue == null || _sabCommandQueue.Count <= 0) return;

			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_HTTP_CLIENT, out _currentComponent);

			ISabHttpClient client = _currentComponent as SabHttpClient;

			if (client == null) return;

			foreach (SabClientCommand item in _sabCommandQueue)
			{
				client.SendCommand(item);
			}

			_sabCommandQueue.Clear();
		}

		#endregion

		#region FSM - Stop main SAB process

		private void ProcessStopProcess(bool force = false)
		{
			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_HTTP_CLIENT, out _currentComponent);

			ISabHttpClient client = _currentComponent as SabHttpClient;

			if (client != null)
			{
				if (!client.Disconnect())
				{
					if (!force)
					{
						LogWriter.Write($"SabManager # Stop - Waiting for client disconnect.");
						return;
					}

					LogWriter.Write($"SabManager # Stop - Client disconnecting...");
				}
			}

			_sabComponents.TryGetValue(SabComponentTypes.COMPONENT_PROCESS, out _currentComponent);

			ISabComponent sabProcess = _currentComponent as SabProcess;

			if (sabProcess != null)
			{
				if (!sabProcess.Stop())
				{
					if (!force)
					{
						LogWriter.Write($"SabManager # Stop - Waiting for process stop.");
						return;
					}

					LogWriter.Write($"SabManager # Stop - Process stopping...");
				}
			}

			LogWriter.Write($"SabManager # Fully shut down SABnzbd process and web client connection.");

			SetState(SabManagerState.Idle);
		}

		#endregion

		#region FSM - Restart

		private void ProcessRestart()
		{
			if (_bootTimerStarted)
			{
				ProcessStopProcess();
				ProcessCleanSabManager();
				_restartTimer = DateTime.Now;
			}
			else
			{
				if (DateTime.Now <= _restartTimer + TimeSpan.FromSeconds(RESTART_TIME_SECONDS)) return;

				LogWriter.Write($"SabManager # ProcessRestart - Restart timer elapsed, rebooting processes.");
				SetState(SabManagerState.Starting);
			}
		}

		#endregion

		#region FSM - Clean SAB manager

		private void ProcessCleanSabManager()
		{
			_currentComponent = null;
			_bootTimerStarted = false;
			LogWriter.Write($"SabManager # Cleaned SabManager.");
			SetState(SabManagerState.Idle);
		}

		#endregion
	}
}
