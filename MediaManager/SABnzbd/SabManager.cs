using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Logging;
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
			Error,
			Cleanup,
		}

		private string _currentError = "";
		private Dictionary<int, ISabComponent> _sabComponents;
		private ISabComponent _currentComponent;
	    private List<Control> _ownedControls;
		private SettingsData _userSettings;
		private List<SabClientCommand> _sabCommandQueue;
		private Timer _errorHandler;

		private DateTime _bootTimer;
		private bool _bootTimerStarted;

		private TimeSpan _connectSpan;
		private DateTime _connectTime;

		private Dictionary<string, List<string>> _errorMessages = new Dictionary<string, List<string>>();

		private SabManagerState _state;
		public int State => (int)_state;

	    private uint errorCounter = 0;

		private const int BOOT_TIME_GRACE_SECONDS = 2;

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

		public void Destroy()
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

		#endregion

		public void Update()
		{
			switch (_state)
			{
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

			MaintainCommandQueue();
		}

		private bool CheckRunValidity()
		{
			if (!Config.CORE_DOWNLOADER_REQUIRES_VPN) return true;

			bool valid = true;

			if (VpnManager.Instance == null)
			{
				LogWriter.Write($"SabManager # Failed validity check: VpnManager.Instance == null");
				valid = false;
			}

			if ((VpnManager.VpnManagerState) VpnManager.Instance.State != VpnManager.VpnManagerState.Connected)
			{
				LogWriter.Write($"SabManager # Failed validity check: State != Connected ({(VpnManager.VpnManagerState)VpnManager.Instance.State})");
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

		#region FSM - Clean SAB manager

		private void ProcessCleanSabManager()
		{
			_currentComponent = null;
			_bootTimerStarted = false;
			LogWriter.Write($"SabManager # Cleaned SabManager, returning to idle.");
			SetState(SabManagerState.Idle);
		}

		#endregion
	}
}
