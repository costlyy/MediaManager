using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Logging;
using MediaManager.Properties;
using MediaManager.SABnzbd;
using MediaManager.VPN;

namespace MediaManager.Actions
{
	class ActionsManager : IManagerAdvanced
	{
		public enum ActionsManagerState
		{
			Idle,
			InitControls,
			Running,
			Error,
		}

		private enum ActionButtons
		{
			Invalid,
			KillAll,
			RestartMachine,
			RestartSab,
			RestartVpn,
			RestartVpnSab,
			ClearLogins,
			FactoryReset,
			ToggleStreamMode,
			CheckForUpdate,
		}

		private const int ACTION_STREAM_MODE_TIMER_TICK_MS = 1000;
		private const double ACTION_STREAM_MODE_TOTAL_DURATION_M = 90.0;

		private ActionsManagerState _state;
		public int State => (int)_state;

		private string _errorString;
		private string _detailedErrorString;

		public DateTime StartTime { get; }

		private List<Control> _controls;

		private Timer _streamingModeTimer;
		private TimeSpan _streamTime;
		private DateTime _streamEndTime;

		private bool _restoreVpnAfter;
		private bool _restoreDownloadsAfter;

		private Label _streamTimeLabel;
		private Dictionary<ActionButtons, Button> _actionButtonsDictionary;

		#region Singleton
		private static ActionsManager _instance;
		public static ActionsManager Instance => _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				LogWriter.Write("ActionsManager # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new ActionsManager();
		}

		private ActionsManager()
		{
			StartTime = DateTime.Now;
		}

		#endregion

		public bool Start()
		{
			if (_state == ActionsManagerState.Error)
			{
				LogWriter.Write($"ActionsManager # Failed to start ActionsManager due to error.");
				LogWriter.PrintCallstack();
				return false;
			}

			LogWriter.Write($"ActionsManager # Starting ActionsManager.");

			SetState(ActionsManagerState.InitControls);
			return true;
		}

		public void Update()
		{
			try
			{
				switch (_state)
				{
					case ActionsManagerState.Idle:
						break;
					case ActionsManagerState.InitControls:
						ProcessInitControls();
						break;
					case ActionsManagerState.Running:
						break;
					case ActionsManagerState.Error:
						break;
				}
			}
			catch (Exception generalException)
			{
				_errorString = "There was a general exception in ConfigManager.";
				_detailedErrorString = generalException.ToString();
				SetState(ActionsManagerState.Error);
			}
		}

		public void SetControls(List<Control> controls)
		{
			if (controls == null)
			{
				LogWriter.Write($"ActionsManager # Passed controls data was NULL.", DebugPriority.High);
				_errorString = "ActionsManager # Passed controls data was NULL";
				SetState(ActionsManagerState.Error);
				return;
			}

			_controls = controls;
		}

		public bool GetError(ref string error)
		{
			if (_state != ActionsManagerState.Error) return false;

			SetState(ActionsManagerState.Idle);

			error = _detailedErrorString;
			return true;
		}

		public string GetUpTime()
		{
			return "00:00:00";
		}

		public void SaveData()
		{
			// TODO: Save profile data
		}

		private void SetState(ActionsManagerState newState)
		{
			LogWriter.Write($"ActionsManager # SetState from {_state} to new state: {newState}.");
			_state = newState;
		}

		private void StreamTimerTick(object sender, EventArgs e)
		{
			TimeSpan timeDiff = _streamEndTime - DateTime.Now;

			// Check to see if the timer has elapsed.
			if (timeDiff.TotalSeconds < 0)
			{
				ActionToggleStreamMode(null, null);
				LogWriter.Write("ActionsManager # StreamTimerTick - Streaming mode timer has elapsed, turning off streaming mode.");
				return;
			}

			var formattedOutput = new StringBuilder();

			if (timeDiff.TotalSeconds < 60)
			{
				formattedOutput.Append("<");
			}

			string timeSpanString = timeDiff.TotalMinutes.ToString(CultureInfo.InvariantCulture);

			foreach (char c in timeSpanString)
			{
				if (c.Equals('.'))
				{
					break;
				}

				formattedOutput.Append(c);
			}

			formattedOutput.Append("m");

			_streamTimeLabel.Text = formattedOutput.ToString();
		}

		private bool IsStreamingModeEnabled()
		{
			return _streamingModeTimer != null;
		}

		private void EnableStreamingMode()
		{
			if (!_actionButtonsDictionary.TryGetValue(ActionButtons.ToggleStreamMode, out Button toggleButton))
			{
				LogWriter.Write($"ActionsManager # EnableStreamingMode - Could not locate UI controls.");
				return;
			}

			_streamingModeTimer = new Timer { Interval = ACTION_STREAM_MODE_TIMER_TICK_MS };
			_streamingModeTimer.Tick += StreamTimerTick;
			_streamingModeTimer.Start();

			_streamTime = TimeSpan.FromMinutes(ACTION_STREAM_MODE_TOTAL_DURATION_M);
			_streamEndTime = DateTime.Now + _streamTime;

			toggleButton.Text = "Streaming Mode On";
			toggleButton.ForeColor = Color.Green;
			_streamTimeLabel.Text = _streamTime.TotalMinutes + "m";

			if (VpnManager.Instance.IsConnected())
			{
				_restoreVpnAfter = true;
			}

			VpnManager.Instance.Disconnect();
			VpnManager.Instance.ToggleEnabledState(false);

			if (SabManager.Instance.IsConnected())
			{
				_restoreDownloadsAfter = true;
			}

			SabManager.Instance.KillProcess();
			SabManager.Instance.ToggleEnabledState(false);

			LogWriter.Write($"ActionsManager # EnableStreamingMode - Started Streaming mode. _restoreVpnAfter: {_restoreVpnAfter}, _restoreDownloadsAfter: {_restoreDownloadsAfter}");
		}

		private void DisableStreamingMode()
		{
			if (!_actionButtonsDictionary.TryGetValue(ActionButtons.ToggleStreamMode, out Button toggleButton))
			{
				LogWriter.Write($"ActionsManager # DisableStreamingMode - Could not locate UI controls.");
				return;
			}

			_streamingModeTimer.Stop();
			_streamingModeTimer = null;

			_streamTime = TimeSpan.FromMinutes(0d);

			toggleButton.Text = "Streaming Mode Off";
			toggleButton.ForeColor = Color.FromArgb(215, 215, 215);
			_streamTimeLabel.Text = "0m";

			VpnManager.Instance.ToggleEnabledState(true);

			if (_restoreVpnAfter)
			{
				LogWriter.Write($"ActionsManager # DisableStreamingMode - Automatically restoring VPN state after streaming.");
				VpnManager.Instance.Connect();
			}

			SabManager.Instance.ToggleEnabledState(true);

			if (_restoreDownloadsAfter)
			{
				LogWriter.Write("ActionsManager # DisableStreamingMode - Attempting to restore downloader running state.");

				if (!SabManager.Instance.StartProcess())
				{
					LogWriter.Write(
						"ActionsManager # DisableStreamingMode - Failed to start process, attempting to process errors.");

					string error = "";
					if (SabManager.Instance.GetError(ref error))
					{
						if (!SabManager.Instance.StartProcess())
						{
							LogWriter.Write(
								"ActionsManager # DisableStreamingMode - Critical Error! Processed errors but still could not start.",
								DebugPriority.High, true);
							return;
						}

						LogWriter.Write($"ActionsManager # DisableStreamingMode - Got error: {error}");
					}
				}
			}

			LogWriter.Write("ActionsManager # DisableStreamingMode - Manually stopped Streaming mode (re-enabled VPN etc).");
		}

		#region Button Actions

		private void ActionKillAll(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionKillAll");
			SabManager.Instance.StopProcess();
			VpnManager.Instance.Disconnect(true);
		}

		private void ActionRestartMachine(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionRestartMachine");

			DialogResult result = MessageBox.Show("Shutdown whole system? This will cancel any running processes and downloads.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

			switch (result)
			{
				default:
					goto case DialogResult.No;

				case DialogResult.Yes:
					Helpers.ExitWindows(Helpers.EWX_REBOOT | Helpers.EWX_FORCE | Helpers.EWX_FORCEIFHUNG);
					return;
				case DialogResult.No:
					return;
			}
		}

		private void ActionRestartSabnzbd(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionRestartSabnzbd");

			SabManager.Instance?.Restart();
		}

		private void ActionRestartVpn(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionRestartVpn");

			VpnManager.Instance?.Restart();
		}

		private void ActionRestartVpnSab(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionRestartVpnSab");
			// TODO: this is going to take a while to process, need to enqueue the requests somehow.
		}

		private void ActionClearLogins(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionClearLogins");
		}

		private void ActionFactoryReset(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionFactoryReset");
		}

		private void ActionToggleStreamMode(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionToggleStreamMode");

			if (VpnManager.Instance == null)
			{
				LogWriter.Write($"ActionsManager # ActionToggleStreamMode - VpnManager is NULL, cannot start stream mode.", DebugPriority.High, true);
				return;
			}

			if (SabManager.Instance == null)
			{
				LogWriter.Write($"ActionsManager # ActionToggleStreamMode - SabManagerr is NULL, cannot start stream mode.", DebugPriority.High, true);
				return;
			}

			if (!IsStreamingModeEnabled())
			{
				EnableStreamingMode();
			}
			else
			{
				DisableStreamingMode();
			}
		}

		private void ActionCheckForUpdate(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionCheckForUpdate");
		}

		#endregion

		private Action<object, EventArgs> GetActionForButton(ActionButtons button)
		{
			switch (button)
			{
				case ActionButtons.KillAll: return ActionKillAll;
				case ActionButtons.RestartMachine: return ActionRestartMachine;
				case ActionButtons.RestartSab: return ActionRestartSabnzbd;
				case ActionButtons.RestartVpn: return ActionRestartVpn;
				case ActionButtons.RestartVpnSab: return ActionRestartVpnSab;
				case ActionButtons.ClearLogins: return ActionClearLogins;
				case ActionButtons.FactoryReset: return ActionFactoryReset;
				case ActionButtons.ToggleStreamMode: return ActionToggleStreamMode;
				case ActionButtons.CheckForUpdate: return ActionCheckForUpdate;
			}

			LogWriter.Write($"ActionsManager # GetActionForButton - Unknown / unhandled button type: {button}");
			return null;
		}

		private bool GetActionFromName(string buttonName, out ActionButtons button)
		{
			// Strip out the "btn" part of the control (if it exists) as the enum is more generic and doesn't use that prefix.
			if (buttonName.Contains("btn"))
			{
				buttonName = buttonName.Substring(buttonName.IndexOf("btn", StringComparison.Ordinal) + 3);
			}

			try
			{
				button = (ActionButtons)Enum.Parse(typeof(ActionButtons), buttonName, true);
				return true;
			}
			catch
			{
				button = ActionButtons.Invalid;
			}
			return false;
		}

		private void ProcessControl(Control item)
		{
			if (item.Name.Equals("lblStreamMode", StringComparison.CurrentCultureIgnoreCase))
			{
				_streamTimeLabel = (Label)item;
			}

			if (!GetActionFromName(item.Name, out ActionButtons button)) return;

			item.Click += (s, e) => GetActionForButton(button).Invoke(s, e);
			_actionButtonsDictionary.Add(button, item as Button);
		}

		#region FSM methods

		private void ProcessInitControls()
		{
			_actionButtonsDictionary = new Dictionary<ActionButtons, Button>();

			foreach (Control item in _controls.Where(control => control != null))
			{
				try
				{
					ProcessControl(item);
				}
				catch (Exception e)
				{
					LogWriter.Write($"ActionsManager # ProcessInitControls - Caught exception while processing control: {item.Name}, ex: {e}");
				}
			}

			SetState(ActionsManagerState.Running);
		}

		#endregion

	}
}
