using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Logging;
using MediaManager.SABnzbd;
using MediaManager.VPN;

namespace MediaManager.Actions
{
	class ActionsManager : IManager
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

		private ActionsManagerState _state;
		public int State => (int)_state;

		private string _errorString;
		private string _detailedErrorString;

		private SettingsData _settings;
		private List<Control> _controls;

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

		public void SetSettings(ref SettingsData settings)
		{
			if (settings == null)
			{
				LogWriter.Write($"ActionsManager # Passed settings data was NULL.", DebugPriority.High);
			}

			_settings = settings;
		}

		public string GetError(ref string detailedError)
		{
			SetState(ActionsManagerState.Idle);

			detailedError = _detailedErrorString;
			return _errorString;
		}

		public string GetUpTime()
		{
			return "00:00:00";
		}

		private void SetState(ActionsManagerState newState)
		{
			LogWriter.Write($"ActionsManager # SetState from {_state} to new state: {newState}.");
			_state = newState;
		}

		#region Button Actions

		private void ActionKillAll(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionKillAll");
			SabManager.Instance?.StopProcess();
			VpnManager.Instance?.Disconnect(true);
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
		}

		private void ActionCheckForUpdate(object sender, EventArgs e)
		{
			LogWriter.Write($"ActionsManager # ActionCheckForUpdate");
		}

		#endregion

		private Action<object, EventArgs> GetActionForButton(string buttonName)
		{
			// Strip out the "btn" part of the control (if it exists) as the enum is more generic and doesn't use that prefix.
			if (buttonName.Contains("btn"))
			{
				buttonName = buttonName.Substring(buttonName.IndexOf("btn", StringComparison.Ordinal) + 3);
			}

			var button = (ActionButtons) Enum.Parse(typeof(ActionButtons), buttonName, true);

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

		private void ProcessControl(Control item)
		{ 
			item.Click += (s, e) => GetActionForButton(item.Name).Invoke(s, e);
		}

		#region FSM methods

		private void ProcessInitControls()
		{
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
