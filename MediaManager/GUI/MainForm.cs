using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using MediaManager.Actions;
using MediaManager.Core;
using MediaManager.Downloads;
using MediaManager.GUI.Status;
using MediaManager.Logging;
using MediaManager.Properties;
using MediaManager.SABnzbd;
using MediaManager.VPN.SocketCommands;
using VpnManager = MediaManager.VPN.VpnManager;

namespace MediaManager.GUI
{
	public partial class MainForm : Form
	{
		
		private SettingsData _settingData;
		private SettingsForm _settingsForm;
		private readonly Dictionary<int, List<string>> _errorMessages = new Dictionary<int, List<string>>();
		private DriveDisplay _statusDriveDisplay;

		public MainForm(string[] args)
		{
			LogWriter.Write("Constructed MainForm.");

			_settingData = new SettingsData(Settings.Default);

			InitializeComponent();
			InitializeKeepAlive();
			InitializeStatus();

			StartVpnManager();
			StartSabManager();

#if DEBUG
			LogWriter.Write("Application running in DEBUG mode.");
			lblDebugMode.Visible = true;
#endif

			if (_settingData.VpnConnectOnBoot)
			{
				//ConnectVpn();
			}

			lblVersion.Text = Application.ProductVersion;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(0, 0);

			DownloadManager.Initialize();
			DownloadManager.Instance.AddParentPanel(panelDownloadManager);

			InitActionsPanel();

#if DEBUG
			//// TODO: Remove this.
			//var slot = new JsonQueueSlot
			//{
			//	NzoID = "SABnzbd_nzo_1zvuxq",
			//	FileName = "Black.Sails.S01E01.720p.BluRay.x264-DEMAND",
			//	Index = 0,
			//	Status = "Downloading",
			//	Category = "sonarr",
			//	EstimatedTime = "13:50 Tue 14 Aug",
			//	AvergaeAge = "1347d",
			//	HasRating = true,
			//	Size = "3.1 GB",
			//	MbLeft = "3129.75",
			//	Mb = "0",
			//	SizeMbLeft = "0",
			//	Percentage = "0",
			//	TimeLeft = "71:16:28:39",
			//	Missing = 1,
			//	Password = "",
			//	UnpackOptions = "3",
			//};
			//DownloadManager.Instance.AddItemManually(new DownloadItem(slot));

			//slot = new JsonQueueSlot
			//{
			//	FileName = "TEST_FILE_NAME_HERE_1",
			//	NzoID = "111111111111"
			//};
			//DownloadManager.Instance.AddItemManually(new DownloadItem(slot));

			//slot = new JsonQueueSlot
			//{
			//	FileName = "TEST_FILE_NAME_HERE_2",
			//	NzoID = "2222222222"
			//};
			//DownloadManager.Instance.AddItemManually(new DownloadItem(slot));
#endif
		}

		private void InitializeStatus()
		{
			_statusDriveDisplay = new DriveDisplay(_settingData);
			_statusDriveDisplay.AttachControls(statusTableDisk);
			_statusDriveDisplay?.Update();
			LogWriter.Write("Forms # Initialized status manager components.");
		}

		private void InitializeKeepAlive()
		{
			Click += (o, args) => HeartbeatManager.Instance.KeepAlive();

			foreach (var item in Controls)
			{
				if (item is Panel panel)
				{
					InitializeNestedPanels(panel);
					LogWriter.Write($"Forms # Initialized panel: {panel.Name}.{panel.GetHashCode()}", DebugPriority.Low);
				}
			}

			LogWriter.Write("Forms # Finished secondary initialization.");
		}

		private void InitializeNestedPanels(Panel parent)
		{
			if (parent == null) return; // finished, break out.

			foreach (Control item in parent.Controls)
			{
				if (item is Form form)
				{
					AddKeepAliveNotifier(form);
					continue;
				}

				if (item is Button button)
				{
					AddKeepAliveNotifier(button);
					continue;
				}

				if (!(item is Panel panel)) continue;

				LogWriter.Write($"Forms # Initialized panel: {panel.Name}.{panel.GetHashCode()}", DebugPriority.Low);

				InitializeNestedPanels(panel);
			}
		}

		private void AddKeepAliveNotifier(Form control)
		{
			if (control == null) return;

			LogWriter.Write($"controls # Initialized form: {control.Name}.{control.GetHashCode()}", DebugPriority.Low);

			control.Click += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseEnter += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseLeave += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseDown += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseDoubleClick += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseUp += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseWheel += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.KeyDown += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.KeyPress += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.KeyUp += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.GotFocus += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.LostFocus += (o, args) => HeartbeatManager.Instance.KeepAlive();
		}

		private void AddKeepAliveNotifier(Button control)
		{
			if (control == null) return;

			LogWriter.Write($"Forms # Initialized button: {control.Name}.{control.GetHashCode()}", DebugPriority.Low);

			control.Click += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseEnter += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseLeave += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseDown += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseDoubleClick += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseUp += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.MouseWheel += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.KeyDown += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.KeyPress += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.KeyUp += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.GotFocus += (o, args) => HeartbeatManager.Instance.KeepAlive();
			control.LostFocus += (o, args) => HeartbeatManager.Instance.KeepAlive();
		}

		private void ProcessErrors()
		{
			if (_errorMessages.Count <= 0)
			{
				return;
			}

			foreach (KeyValuePair<int, List<string>> errorMessage in _errorMessages)
			{
				LogWriter.Write("Start processing errors for component: " + errorMessage.Key);

				foreach (var item in errorMessage.Value)
				{
					LogWriter.Write(item);
				}

				LogWriter.Write("End processing errors for component: " + errorMessage.Key);
			}

			_errorMessages.Clear();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			_settingData.Save();
			Settings.Default.Save();
			VpnManager.Instance?.Destroy();
			SabManager.Instance?.KillProcess();
			LogWriter.Write($"MainForm # Finished save, program closed.");
		}

		#region Border / Main Form Controls

		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnHide_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			panelShade.Location = new Point(0, 0);
			panelShade.BackColor = Color.FromArgb(128, 128, 128, 128);
			panelShade.Size = new Size(1920, 1080);
			panelShade.Visible = true;

			_settingsForm = new SettingsForm(ref _settingData);
			_settingsForm.ShowDialog();
			_settingData = _settingsForm.GetData();

			panelShade.Location = new Point(1920, 1080);
			panelShade.Visible = false;
		}

		#endregion Parent Controls

		#region Actions

		private void InitActionsPanel()
		{
			ActionsManager.Initialize();

			List<Control> controls = new List<Control>();

			foreach (Control item in panelQuickActions.Controls)
			{
				if (item == null)
				{
					continue;
				}

				controls.Add(item);
			}

			ActionsManager.Instance.SetControls(controls);
			ActionsManager.Instance.SetSettings(ref _settingData);
			ActionsManager.Instance.Start();
		}

		#endregion Quick Actions

		#region Status Display

		public void MainUpdate()
		{
			ActionsManager.Instance?.Update();
			MaintainVpnManager();
			UpdateStorageStatus();
			MaintainSabManager();
			UpdateDownloadManager();
			ProcessErrors();

		    _settingsForm?.Update();
        }

		private void UpdateDownloadManager()
		{
			if (SabManager.Instance != null)
			{
				DownloadManager.Instance.SetData(SabManager.Instance.GetClientData());
			}

			DownloadManager.Instance.Update();
		}

		private void UpdateStorageStatus()
		{
			_statusDriveDisplay?.Update();
		}

		#endregion Status Display

		#region SABnzbd Manager

		private void StartSabManager()
		{
			if (SabManager.Instance != null) return;

			List<Control> sabControls = new List<Control>();

			foreach (Control item in panelDownloadManager.Controls)
			{
				sabControls.Add(item);
			}

			SabManager.Initialize();

			if (SabManager.Instance == null)
			{
				LogWriter.Write($"MainForm # StartSabManager - Failed to initialise SabNZBD manager.", DebugPriority.High, true);
				return;
			}

			SabManager.Instance.SetSettings(ref _settingData);
			SabManager.Instance.SetControls(sabControls);
		}

		private void MaintainSabManager()
		{
			if (SabManager.Instance == null)
			{
				List<Process> myProcesses = new List<Process>(Process.GetProcesses());

				foreach (Process item in myProcesses.Where(item => item != null))
				{
					if (string.Equals(item.ProcessName, "sabnzbd", StringComparison.InvariantCultureIgnoreCase))
					{
						try
						{
							item.Kill();
							LogWriter.Write("MainForm # Killed unexpected SABnzbd process.");
						}
						catch (Exception)
						{
						}
					}
				}
			}

			SabManager.Instance?.Update();

			if (SabManager.Instance == null)
			{
				lblSabState.Text = "STOPPED";
				lblSabState.ForeColor = Color.Red;
				return;
			}

			var clientData = SabManager.Instance?.GetClientData();

		    if (clientData != null)
		    {
			    lblSabUptime.Text = SabManager.Instance.GetUpTime();
			}

		    switch ((SabManager.SabManagerState)SabManager.Instance.State)
			{

				case SabManager.SabManagerState.Idle:
				case SabManager.SabManagerState.Stopped:

					lblSabState.Text = "STOPPED";
					lblSabState.ForeColor = Color.Red;

					if (btnDownloadToggle.Image != Resources.icon_play_mid)
					{
						btnDownloadToggle.Image = Resources.icon_play_mid;
					}

					break;

				case SabManager.SabManagerState.Running:

					if (clientData?.StatusData == null)
					{
						if (SabManager.Instance.PauseState != SabManager.ClientPauseState.Unpaused && SabManager.Instance.PauseState != SabManager.ClientPauseState.Paused)
						{
							lblSabState.Text = SabManager.Instance.PauseState.ToString().ToUpper();
							lblSabState.ForeColor = Color.Yellow;
						}
						else
						{
							lblSabState.Text = "RUNNING";
							lblSabState.ForeColor = Color.Green;
						}

						if (btnDownloadToggle.Image != Resources.icon_stop_mid)
						{
							btnDownloadToggle.Image = Resources.icon_stop_mid;
						}

						return;
					}

					if (clientData.Paused)
					{
						if (SabManager.Instance.PauseState != SabManager.ClientPauseState.Unpaused && SabManager.Instance.PauseState != SabManager.ClientPauseState.Paused)
						{
							lblSabState.Text = SabManager.Instance.PauseState.ToString().ToUpper();
							lblSabState.ForeColor = Color.Yellow;
						}
						else
						{
							lblSabState.Text = "PAUSED";
							lblSabState.ForeColor = Color.Yellow;
						}

						if (btnDownloadToggle.Image != Resources.icon_stop_mid)
						{
							btnDownloadToggle.Image = Resources.icon_stop_mid;
						}
					}
					else
					{
						if (SabManager.Instance.PauseState != SabManager.ClientPauseState.Unpaused && SabManager.Instance.PauseState != SabManager.ClientPauseState.Paused)
						{
							lblSabState.Text = SabManager.Instance.PauseState.ToString().ToUpper();
							lblSabState.ForeColor = Color.Yellow;
						}
						else
						{
							lblSabState.Text = "RUNNING";
							lblSabState.ForeColor = Color.Green;
						}

						if (btnDownloadToggle.Image != Resources.icon_stop_mid)
						{
							btnDownloadToggle.Image = Resources.icon_stop_mid;
						}
					}

					break;

				case SabManager.SabManagerState.LoadingHttpClient:
				case SabManager.SabManagerState.LoadingProcess:
				case SabManager.SabManagerState.VerifyHttpConnection:

					lblSabState.Text = "STARTING";
					lblSabState.ForeColor = Color.Yellow;

					break;

			    case SabManager.SabManagerState.Error:

				    lblSabState.Text = "ERROR";
			        lblSabState.ForeColor = Color.Red;

			        if (btnDownloadToggle.Image != Resources.icon_stop_mid)
			        {
			            btnDownloadToggle.Image = Resources.icon_stop_mid;
			        }

                    break;
            }
		}

		#endregion SABnzbd Manager

		#region VPN Manager

		private void MaintainVpnManager()
		{
			ProcessResponses();

			VpnManager.Instance.Update();

			lblVpnUptime.Text = VpnManager.Instance.GetUpTime();

			switch ((VpnManager.VpnManagerState) VpnManager.Instance.State)
			{
				case VpnManager.VpnManagerState.Idle:
					lblVpnState.Text = "STOPPED";
					lblVpnState.ForeColor = Color.Red;
					break;

				case VpnManager.VpnManagerState.LoadingProcess:
				case VpnManager.VpnManagerState.LoadingSocket:
					lblVpnState.Text = "INITIALISE";
					lblVpnState.ForeColor = Color.Yellow;
					break;
			}

			UpdateVpnButton();
		}

		private void ProcessResponses()
		{
			var commandResponses = VpnManager.Instance.GetCommandResponses();

			if (commandResponses == null) return;

			foreach (VpnSocketResponse responseItem in commandResponses)
			{
				var stateResponse = responseItem as GetStateResponse;

				if (stateResponse == null) continue;

				lblVpnState.Text = stateResponse.State;
				lblVpnState.ForeColor = stateResponse.GetColor();
			}
		}

		private void UpdateVpnButton()
		{
			int.TryParse(btnVpnToggle.Tag.ToString(), out var buttonTag);

			switch ((VpnManager.VpnManagerState) VpnManager.Instance.State)
			{
				case VpnManager.VpnManagerState.Connected:

					if (buttonTag != VpnManager.BUTTON_STATE_VPN_RUNNING)
					{
						btnVpnToggle.Tag = VpnManager.BUTTON_STATE_VPN_RUNNING;
						btnVpnToggle.Image = Resources.icon_stop_mid;
					}
					break;

				case VpnManager.VpnManagerState.LoadingProcess:
				case VpnManager.VpnManagerState.LoadingSocket:

					btnVpnToggle.Image = Resources.icon_stop_mid;					
					break;

				case VpnManager.VpnManagerState.Disconnect:
				case VpnManager.VpnManagerState.Terminated:
				case VpnManager.VpnManagerState.Stopped:
				case VpnManager.VpnManagerState.Idle:
				case VpnManager.VpnManagerState.Error:

					if (buttonTag != VpnManager.BUTTON_STATE_VPN_OFF)
					{
						btnVpnToggle.Tag = VpnManager.BUTTON_STATE_VPN_OFF;
						btnVpnToggle.Image = Resources.icon_play_mid;
					}
					break;

				case VpnManager.VpnManagerState.Retrying:

					if (buttonTag != VpnManager.BUTTON_STATE_VPN_RETRYING)
					{
						btnVpnToggle.Tag = VpnManager.BUTTON_STATE_VPN_RETRYING;
						btnVpnToggle.Image = btnVpnToggle.Image = Resources.icon_retry_mid;
					}
					break;
			}
		}

		private void StartVpnManager()
		{
			if (VpnManager.Instance == null)
			{
				if (VpnManager.Instance == null)
				{
					List<Process> myProcesses = new List<Process>(Process.GetProcesses());

					foreach (Process item in myProcesses.Where(item => item != null))
					{
						if (string.Equals(item.ProcessName, "openvpn", StringComparison.InvariantCultureIgnoreCase))
						{
							try
							{
								item.Kill();
								LogWriter.Write("MainForm # Killed unexpected OpenVPN process.");
							}
							catch (Exception)
							{
							}
						}
					}
				}

				List<Control> vpnControls = new List<Control>();

				foreach (Control item in panelVpnManager.Controls)
				{
					vpnControls.Add(item);
				}

				VpnManager.Initialize();

				if (VpnManager.Instance == null)
				{
					MessageBox.Show("Cannot start VPN until the VPN Manager is initialized.", "StopProcess right there, criminal scum!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					return;
				}

				VpnManager.Instance.SetSettings(ref _settingData);
				VpnManager.Instance.SetControls(vpnControls);

				if (VpnManager.Instance.Start())
				{
					LogWriter.Write("MainForm # Started a new VPN Manager.");
				}
				else
				{
					List<string> errorMessages = new List<string>();

					string errorMegDetailed = "";

					var errorMsg = VpnManager.Instance.GetError(ref errorMegDetailed);

					if (errorMsg.Length > 0)
					{
						errorMessages.Add(errorMsg);
						errorMessages.Add(errorMegDetailed);

						_errorMessages.Add(errorMsg.GetHashCode(), errorMessages);
					}

					MessageBox.Show("The VPN configs are still being loaded, please wait.", "Please be patient.", MessageBoxButtons.OK, MessageBoxIcon.Information);

					VpnManager.Instance.Start();
				}
			}

			VpnManager.Instance.Start();
		}

		#endregion VPN Manager 

		public void ForceCleanup()
		{
			LogWriter.Write("MainForm # Performing force cleanup....");

			SabManager.Instance?.StopProcess();
			VpnManager.Instance?.Disconnect(true);
		}


		// TODO: Move this into the sab manager class
		private void LblTitleSab_Click(object sender, EventArgs e)
		{
			var sabAddress = new StringBuilder();
			sabAddress.Append("http://");
			sabAddress.Append(_settingData.SabIP);
			sabAddress.Append(":");
			sabAddress.Append(_settingData.SabPort);
			sabAddress.Append("/sabnzbd/");

			LogWriter.Write($"MainForm # Starting new Downloads View at: {sabAddress}");
			Process.Start(sabAddress.ToString());
		}

	}
}
