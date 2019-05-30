using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication.ExtendedProtection.Configuration;
using System.Text;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Downloads;
using MediaManager.GUI.Status;
using MediaManager.Logging;
using MediaManager.Properties;
using MediaManager.SABnzbd;
using MediaManager.SABnzbd.JsonObjects;
using MediaManager.VPN;
using MediaManager.VPN.SocketCommands;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using VpnManager = MediaManager.VPN.VpnManager;

namespace MediaManager.GUI
{
	public partial class MainForm : Form
	{
		
		private SettingsData _settingData;
		private SettingsForm _settingsForm;
		private Dictionary<int, List<string>> _errorMessages = new Dictionary<int, List<string>>();
		private DriveDisplay _statusDriveDisplay;
		private DateTime _ipUpdateTime;
		private int _additionalSeconds = 0;

		private int _debugCount = 0;

		public MainForm(string[] args)
		{
			LogWriter.Write("Constructed MainForm.");

			_settingData = new SettingsData(Properties.Settings.Default);

			InitializeComponent();
			InitializeKeepAlive();
			InitializeStatus();

			StartVpnManager();

#if DEBUG
			LogWriter.Write("Application running in DEBUG mode.");
			lblDebugMode.Visible = true;
#endif

			if (_settingData.VpnConnectOnBoot)
			{
				//ConnectVpn();
			}

			TitleVersion.Text = Program.CURRENT_VERSION;
			StartPosition = FormStartPosition.Manual;
			Location = new Point(0, 0);

			DownloadManager.Initialize();
			DownloadManager.Instance.AddParentPanel(panelDownloadManager);

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
			Properties.Settings.Default.Save();
			VpnManager.Instance?.Destroy();
			SabManager.Instance?.Destroy();
			LogWriter.Write($"MainForm # Finished save, program closed.");
		}

		#region Quick Actions

		private Timer _streamingModeTimer = null;
		private TimeSpan _streamTime;
		private DateTime _streamEndTime;

		private bool _restoreVpnAfter = false;
		private bool _restoreDownloadsAfter = false;

		private void btnRestart_Click(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Shutdown whole system? This will cancel any running processes and downloads.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

			switch (result)
			{
				default:
					goto case DialogResult.No;

				case DialogResult.Yes:
					Helpers.ExitWindows(Helpers.EWX_REBOOT | Helpers.EWX_FORCE);
					return;
				case DialogResult.No:
					return;
			}
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			panelShade.Location = new Point(0,0);
			panelShade.BackColor = Color.FromArgb(128, 128, 128, 128);
			panelShade.Size = new Size(1920, 1080);
			panelShade.Visible = true;

			_settingsForm = new SettingsForm(ref _settingData);
			_settingsForm.ShowDialog();
			_settingData = _settingsForm.GetData();

			panelShade.Location = new Point(1920, 1080);
			panelShade.Visible = false;
		}

		private bool IsStreamingModeEnabled()
		{
			return _streamingModeTimer != null;
		}

		private void EnableStreamingMode()
		{
			_streamingModeTimer = new Timer { Interval = 1000 };
			_streamingModeTimer.Tick += StreamTimerTick;
			_streamingModeTimer.Start();

			_streamTime = TimeSpan.FromMinutes(90d);
			_streamEndTime = DateTime.Now + _streamTime;

			btnToggleStreamMode.Text = "Streaming Mode On";
			btnToggleStreamMode.ForeColor = Color.Green;
			lblStreamMode.Text = _streamTime.TotalMinutes + "m";

			if (IsVpnConnected())
			{
				_restoreVpnAfter = true;
			}

			DisableVpnManager();

			if (IsSabConnected())
			{
				_restoreDownloadsAfter = true;
			}
			
			DisableSabManager();

			LogWriter.Write($"MainForm # Started Streaming mode.");
		}

		private void DisableStreamingMode()
		{
			_streamingModeTimer.Stop();
			_streamingModeTimer = null;

			_streamTime = TimeSpan.FromMinutes(0d);

			btnToggleStreamMode.Text = "Streaming Mode Off";
			btnToggleStreamMode.ForeColor = Color.FromKnownColor(KnownColor.Control);
			lblStreamMode.Text = "0m";

			EnableVpnManager();

			if (_restoreVpnAfter)
			{
				LogWriter.Write($"MainForm # Automatically restoring VPN state after streaming.");
				ConnectVpn();
			}

			EnableSabManager();

			if (_restoreDownloadsAfter)
			{
				LogWriter.Write($"MainForm # Attempting to restore downloader running state.");

				if (SabManager.Instance == null)
				{
					SabManager.Initialize();

					if (SabManager.Instance == null)
					{
						LogWriter.Write($"MainForm # Error! Failed to re-init downloader core.");
						return;
					}

					LogWriter.Write($"MainForm # Re-initialised downloader singleton.");
				}

				SabManager.Instance.SetSettings(ref _settingData);

				if (!SabManager.Instance.StartProcess())
				{
					LogWriter.Write($"MainForm # Failed to start process, attempting to process errors.");

					string error = "";
					SabManager.Instance.GetError(ref error);

					if (!SabManager.Instance.StartProcess())
					{
						LogWriter.Write($"MainForm # Critical Error! Processed errors but still could not start.", DebugPriority.High, true);
						return;
					}

					LogWriter.Write($"MainForm # Got error: {error}");
				}

				btnDownloadToggle.Image = Resources.icon_stop_mid;
			}

			LogWriter.Write($"MainForm # Manually stopped Streaming mode (re-enabled VPN etc).");
		}

		private void btnToggleStreamMode_Click(object sender, EventArgs e)
		{
			if (!IsStreamingModeEnabled())
			{
				EnableStreamingMode();
			}
			else
			{
				DisableStreamingMode();
			}
		}

		private void StreamTimerTick(object sender, EventArgs e)
		{
			TimeSpan timeDiff = _streamEndTime - DateTime.Now;

			// Check to see if the timer has elapsed.
			if (timeDiff.TotalSeconds < 0)
			{
				btnToggleStreamMode_Click(null, null);
				LogWriter.Write($"MainForm # Streaming mode timer has elapsed, turning off streaming mode.");
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

			lblStreamMode.Text = formattedOutput.ToString();
		}

		#endregion Quick Actions

		#region Status Display

		public void MainUpdate()
		{
			//_debugCount++;

			//if (_debugCount > 150)
			//{
			//	LogWriter.Write("MainForm # DEBUG - Firing clear all.");

			//	DownloadManager.Instance.ClearAll();
			//	_debugCount = -9999;
			//}

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

			//try
			//{
			//	var localDrives = DriveInfo.GetDrives();
			
			//	foreach (DriveInfo drive in localDrives.Where(drive => drive != null))
			//	{
			//		var sapceInGb = 0.0f;
			//		string[] stringSpace = new string[2];

   //                 // First drive storage display, specifically only for the first drive.
			//		if (!string.IsNullOrWhiteSpace(_settingData.Storage0))
			//		{
			//			if (drive.Name.ToUpper().Contains(_settingData.Storage0))
			//			{
			//				lblStorageTitle0.Text = $"Free Space {drive.Name}";

			//				sapceInGb = drive.TotalFreeSpace / 1024f / 1024f / 1024f;

			//			    lblStorage0.ForeColor = GetDriveDisplayColour(sapceInGb);

   //                         stringSpace = sapceInGb.ToString(CultureInfo.InvariantCulture).Split('.');
			//				lblStorage0.Text = $"{stringSpace[0]}.{stringSpace[1].Substring(0, 2)} GB";
			//			}
			//		}

   //                 // Second drive display specifics.
			//		if (string.IsNullOrWhiteSpace(_settingData.Storage1)) continue;

			//		if (!drive.Name.ToUpper().Contains(_settingData.Storage1)) continue;

			//		lblStorageTitle1.Text = $"Free Space {drive.Name}";

			//		sapceInGb = drive.TotalFreeSpace / 1024f / 1024f / 1024f;

			//		lblStorage1.ForeColor = GetDriveDisplayColour(sapceInGb);
					
			//		stringSpace = sapceInGb.ToString(CultureInfo.InvariantCulture).Split('.');
			//		lblStorage1.Text = $"{stringSpace[0]}.{stringSpace[1].Substring(0, 2)} GB";
			//	}
			//}
			//catch (Exception)
			//{
			//}
		}

		#endregion Status Display

		#region SABnzbd Manager

		private bool _pauseSab = false;
		private PauseState _pauseState;

		private enum PauseState
		{
			Unpaused,
			Pausing,
			Paused,
			Unpausing,
		}

		private void SetPauseState(PauseState state)
		{
			LogWriter.Write($"MainForm # Setting pause state from {_pauseState} to {state}");
			_pauseState = state;
		}

		private bool IsSabConnected()
		{
			if (SabManager.Instance == null)
			{
				return false;
			}

			switch ((SabManager.SabManagerState) SabManager.Instance.State)
			{
				case SabManager.SabManagerState.Running:
					return true;
				default:
					return false;
			}
		}

		private void DisableSabManager()
		{
			SabManager.Instance?.StopProcess();

			foreach (Control item in panelDownloadManager.Controls)
			{
				if (item == null) continue;

				item.Enabled = false;
			}

			LogWriter.Write($"MainForm # Disabled SAB Downloader.");
		}

		private void EnableSabManager()
		{
			foreach (Control item in panelDownloadManager.Controls)
			{
				if (item == null) continue;

				item.Enabled = true;
			}

			LogWriter.Write($"MainForm # Enabled SAB Downloader.");
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
							LogWriter.Write($"MainForm # Killed unexpected SABnzbd process.");
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
		        ProcessSabClientData(clientData);
		    }

		    switch ((SabManager.SabManagerState)SabManager.Instance.State)
			{

				case SabManager.SabManagerState.Idle:
				case SabManager.SabManagerState.Stopped:

					lblSabState.Text = "STOPPED";
					lblSabState.ForeColor = Color.Red;

					if (btnShowDownloads.Enabled)
					{
						btnShowDownloads.Enabled = false;
					}

					if (btnDownloadToggle.Image != Resources.icon_play_mid)
					{
						btnDownloadToggle.Image = Resources.icon_play_mid;
					}

					break;

				case SabManager.SabManagerState.Running:

					if (!btnShowDownloads.Enabled)
					{
						btnShowDownloads.Enabled = true;
					}

					if (clientData?.StatusData == null)
					{
						if (_pauseState != PauseState.Unpaused && _pauseState != PauseState.Paused)
						{
							lblSabState.Text = _pauseState.ToString().ToUpper();
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
						if (_pauseState != PauseState.Unpaused && _pauseState != PauseState.Paused)
						{
							lblSabState.Text = _pauseState.ToString().ToUpper();
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
						if (_pauseState != PauseState.Unpaused && _pauseState != PauseState.Paused)
						{
							lblSabState.Text = _pauseState.ToString().ToUpper();
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

					if (btnShowDownloads.Enabled)
					{
						btnShowDownloads.Enabled = false;
					}

					lblSabState.Text = "STARTING";
					lblSabState.ForeColor = Color.Yellow;

					break;

			    case SabManager.SabManagerState.Error:

				    if (btnShowDownloads.Enabled)
				    {
					    btnShowDownloads.Enabled = false;
				    }

					lblSabState.Text = "ERROR";
			        lblSabState.ForeColor = Color.Red;

			        if (btnDownloadToggle.Image != Resources.icon_stop_mid)
			        {
			            btnDownloadToggle.Image = Resources.icon_stop_mid;
			        }

                    break;
            }
		}

		private void ProcessSabClientData(SabManager.SabHttpData data)
		{
			lblSabUptime.Text = SabManager.Instance.GetUpTime();
			ProcessPauseState(data);
		}

		private void ProcessPauseState(SabManager.SabHttpData data)
		{
			if (data?.StatusData == null) return;

			// If the manager hasn't been started yet, wait for it to happen.
			if (SabManager.Instance == null)
			{
				return;
			}

			switch (_pauseState)
			{
				case PauseState.Unpaused:

					if (!data.Paused)
					{
						if (_pauseSab)
						{
							SabManager.Instance.QueueCommand(new SABnzbd.SabCommands.SabManager.SetPaused());
							LogWriter.Write($"MainForm # Sent pause request to SABnzbd.");
							SetPauseState(PauseState.Pausing);
						}
					}
					else
					{
						LogWriter.Write($"MainForm # Sent pause request to SABnzbd, but it is already paused.");
						SetPauseState(PauseState.Paused);
					}

					break;

				case PauseState.Pausing:

					if (data.Paused)
					{
						LogWriter.Write($"MainForm # Send pause cmd finished, moving to paused.");
						SetPauseState(PauseState.Paused);
					}

					break;

				case PauseState.Paused:

					if (!data.Paused)
					{
						SabManager.Instance.QueueCommand(new SABnzbd.SabCommands.SabManager.SetUnPaused());
						LogWriter.Write($"MainForm # Warning! SABnzbd un-paused without direction! What happened?", DebugPriority.High);
						SetPauseState(PauseState.Unpaused);
					}

					if (!_pauseSab)
					{
						SabManager.Instance.QueueCommand(new SABnzbd.SabCommands.SabManager.SetUnPaused());
						LogWriter.Write($"MainForm # Sent un-pause request to SABnzbd.");
						SetPauseState(PauseState.Unpausing);
					}	

					break;

				case PauseState.Unpausing:

					if (!data.Paused)
					{
						LogWriter.Write($"MainForm # Send un-pause cmd finished, moving to un-paused.");
						SetPauseState(PauseState.Unpaused);
					}

					break;
			}
		}

		private void btnDownloadToggle_Click(object sender, EventArgs e)
		{
			if (SabManager.Instance == null)
			{
				SabManager.Initialize();
				SabManager.Instance?.SetSettings(ref _settingData);

				if (SabManager.Instance == null || !SabManager.Instance.StartProcess())
				{
					LogWriter.Write($"MainForm # Failed to start SABnzbd (initial).");
					return;
				}

				btnDownloadToggle.Image = Resources.icon_stop_mid;
				LogWriter.Write($"MainForm # Started SABnzbd Manager (initial).");
			}

			string error = "";

			switch ((SabManager.SabManagerState) SabManager.Instance.State)
			{
				case SabManager.SabManagerState.Idle:
				case SabManager.SabManagerState.Stopped:

					SabManager.Instance.SetSettings(ref _settingData);

					if (!SabManager.Instance.StartProcess())
					{
						LogWriter.Write($"MainForm # Failed to start SABnzbd.");
						return;
					}

					btnDownloadToggle.Image = Resources.icon_stop_mid;

					LogWriter.Write($"MainForm # Started SABnzbd Manager.");

					break;

				case SabManager.SabManagerState.Running:

					SabManager.Instance.SetSettings(ref _settingData);

					if (!SabManager.Instance.StopProcess())
					{
						LogWriter.Write($"MainForm # Failed to stop process, attempting to process errors.");

						SabManager.Instance.GetError(ref error);

						if (!SabManager.Instance.StopProcess())
						{
							LogWriter.Write($"MainForm # Critical Error! Processed errors but still could not stop.", DebugPriority.High, true);
							break;
						}
						
						LogWriter.Write($"MainForm # Got error: {error}");
					}

					DownloadManager.Instance?.ClearAll();

					btnDownloadToggle.Image = Resources.icon_play_mid;

					LogWriter.Write($"MainForm # Stopped SABnzbd Manager.");

					break;

			    case SabManager.SabManagerState.Error:

			        SabManager.Instance.GetError(ref error);

                    SabManager.Instance.SetSettings(ref _settingData);
			        SabManager.Instance.StopProcess();

			        DownloadManager.Instance?.ClearAll();

			        btnDownloadToggle.Image = Resources.icon_play_mid;

			        LogWriter.Write($"MainForm # Reset error'd SABnzbd Manager ({error}).");

                    break;
            }
		}

		private void btnDownloadPause_CheckedChanged(object sender, EventArgs e)
		{
			_pauseSab = !_pauseSab;
		}

		#endregion SABnzbd Manager

		#region Parent Controls

		private void btnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnHide_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		#endregion Parent Controls

		#region VPN Manager

		private bool IsVpnConnected()
		{
			if (VpnManager.Instance == null)
			{
				return false;
			}

			switch ((VpnManager.VpnManagerState) VpnManager.Instance.State)
			{
				case VpnManager.VpnManagerState.VerifyConnection:
				case VpnManager.VpnManagerState.Connected:
				case VpnManager.VpnManagerState.Retrying:
					return true;
				default:
					return false;
			}
		}

		private void MaintainVpnManager()
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
							LogWriter.Write($"MainForm # Killed unexpected OpenVPN process.");
						}
						catch (Exception)
						{
						}
					}
				}

				return;
			}

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

				case VpnManager.VpnManagerState.Connected:

					VpnManager.Instance.QueueCommand(new VPN.SocketCommands.VpnManager.GetStateCommand());

					break;

				case VpnManager.VpnManagerState.Error:
					break;
			}

			UpdateVpnButton();
			UpdateExternalIP();
		}

		private void UpdateExternalIP()
		{
			int baseSec, rangeSec;

			switch ((HeartbeatManager.OperatingMode) HeartbeatManager.Instance.State)
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

			lblExtIpTimer.Text = Math.Abs(timeSpanNow.Seconds).ToString();

			if (DateTime.Now <= target) return;

			_additionalSeconds = new Random().Next(0, rangeSec);
			_ipUpdateTime = DateTime.Now;

			try
			{
				string extIp = new WebClient().DownloadString("http://icanhazip.com");
				LogWriter.Write($"MainForm # External IP update: {extIp.Trim()}");
				tbxMyIP.Text = extIp;
			}
			catch (Exception ex)
			{
				LogWriter.Write($"MainForm # Caught exception while getting external IP:\n{ex}");
			}
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
			int buttonTag;
			int.TryParse(btnToggleVpn.Tag.ToString(), out buttonTag);

			switch ((VpnManager.VpnManagerState) VpnManager.Instance.State)
			{
				case VpnManager.VpnManagerState.Connected:

					if (buttonTag != VpnManager.BUTTON_STATE_VPN_RUNNING)
					{
						btnToggleVpn.Tag = VpnManager.BUTTON_STATE_VPN_RUNNING;
						btnToggleVpn.Image = Resources.icon_stop_mid;
					}
					break;

				case VpnManager.VpnManagerState.LoadingProcess:
				case VpnManager.VpnManagerState.LoadingSocket:

					btnToggleVpn.Image = Resources.icon_stop_mid;					
					break;

				case VpnManager.VpnManagerState.Disconnect:
				case VpnManager.VpnManagerState.Terminated:
				case VpnManager.VpnManagerState.Stopped:
				case VpnManager.VpnManagerState.Idle:
				case VpnManager.VpnManagerState.Error:

					if (buttonTag != VpnManager.BUTTON_STATE_VPN_OFF)
					{
						btnToggleVpn.Tag = VpnManager.BUTTON_STATE_VPN_OFF;
						btnToggleVpn.Image = Resources.icon_play_mid;
					}
					break;

				case VpnManager.VpnManagerState.Retrying:

					if (buttonTag != VpnManager.BUTTON_STATE_VPN_RETRYING)
					{
						btnToggleVpn.Tag = VpnManager.BUTTON_STATE_VPN_RETRYING;
						btnToggleVpn.Image = btnToggleVpn.Image = Resources.icon_retry_mid;
					}
					break;
			}
		}

		private void StartVpnManager()
		{
			if (VpnManager.Instance == null)
			{
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
					LogWriter.Write($"MainForm # Started a new VPN Manager.");
				}
				else
				{
					List<string> errorMessages = new List<string>();

					string errorMsg = "", errorMegDetailed = "";

					errorMsg = VpnManager.Instance.GetError(ref errorMegDetailed);

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

		private void DisconnectVpn()
		{
			VpnManager.Instance?.Disconnect(true);
		}

		private void ConnectVpn()
		{
			VpnManager.Instance?.Connect();
		}

		private void btnToggleVpn_Click(object sender, EventArgs e)
		{
			Button item = sender as Button;
			if (item == null) return;

			int buttonTag;
			int.TryParse(item.Tag.ToString(), out buttonTag);

			switch (buttonTag)
			{
				case VpnManager.BUTTON_STATE_VPN_OFF:
					ConnectVpn();
					break;

				case VpnManager.BUTTON_STATE_VPN_RUNNING:
					DisconnectVpn();
					break;

				case VpnManager.BUTTON_STATE_VPN_RETRYING:
					break;
			}
		}

		private void EnableVpnManager()
		{
			foreach (Control item in panelVpnManager.Controls)
			{
				if (item == null) continue;

				item.Enabled = true;
			}

			LogWriter.Write($"MainForm # Enabled VPN Manager.");
		}

		private void DisableVpnManager()
		{
			VpnManager.Instance?.Disconnect();

			foreach (Control item in panelVpnManager.Controls)
			{
				if (item == null) continue;

				item.Enabled = false;
			}

			LogWriter.Write($"MainForm # Disabled VPN Manager.");
		}

		#endregion VPN Manager 

		private void btnKillAll_Click(object sender, EventArgs e)
		{
			LogWriter.Write($"MainForm # User requested to kill all running processes.");
			ForceCleanup();
		}

		public void ForceCleanup()
		{
			LogWriter.Write($"MainForm # Performing force cleanup....");

			SabManager.Instance?.StopProcess();
			VpnManager.Instance?.Disconnect(true);
		}

		private void btnShowDownloads_Click(object sender, EventArgs e)
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

		private void btnVpnPause_Click(object sender, EventArgs e)
		{

		}
	}
}
