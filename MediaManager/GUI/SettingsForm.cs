using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Core.Profile;
using MediaManager.Logging;
using MediaManager.SABnzbd;
using MediaManager.VPN;

namespace MediaManager.GUI
{
	public partial class SettingsForm : Form
	{
		private enum ScrollTabs
		{
			One,
			Two,
			Three,
			Four,
		}

		private enum SettingsPanel
		{
			PanelSab,
			PanelVpn,
			PanelInterface,
			PanelGeneral
		}

		private const int SCROLL_HIGHLIGHT_VARIANCE = 15;

		private readonly CoreProfileData _coreProfileData;
		private readonly VpnProfileData _vpnProfileData;
		private readonly SabProfileData _sabProfileData;

		private Size _backgroundOuterSize = new Size(909, 500);
		private Size _formSize = new Size(950, 520);

		private Point _panelPoint = new Point(150, 10);
		private Point _panelSize = new Point(780, 500);


		private int _ignoreEventCount;
		private int _scrollPosition;
		private static DriveLoadState _driveLoadState = DriveLoadState.Idle;
		private Thread _drivesThread;
		private static List<DriveInfo> _driveData;

		private enum DriveLoadState
		{
			Idle,
			Loading,
			Ready
		}

		public SettingsForm()
		{
			_coreProfileData = new CoreProfileData(ProfileManager.Instance.GetProfilePath());
			var tempCoreData = _coreProfileData.Import();
			if (tempCoreData == null)
			{
				LogWriter.Write($"SettingsForm # Loading - Failed to load core profile data.", DebugPriority.High, true);
				return;
			}
			_coreProfileData = tempCoreData as CoreProfileData;

			_vpnProfileData = new VpnProfileData(ProfileManager.Instance.GetProfilePath());
			var tempVpnData = _vpnProfileData.Import();
			if (tempVpnData == null)
			{
				LogWriter.Write($"SettingsForm # Loading - Failed to load VPN profile data.", DebugPriority.High, true);
			}
			_vpnProfileData = tempVpnData as VpnProfileData;

			_sabProfileData = new SabProfileData(ProfileManager.Instance.GetProfilePath());
			var tempSabData = _sabProfileData.Import();
			if (tempSabData == null)
			{
				LogWriter.Write($"SettingsForm # Loading - Failed to load SAB NZBD profile data.", DebugPriority.High, true);
			}
			_sabProfileData = tempSabData as SabProfileData;

			LogWriter.Write($"SettingsForm # Loading - Started loading settings form.");

			InitializeComponent();

			Size = _formSize;
			//panelBackgroundGrey.Size = _backgroundOuterSize;
			//panelBackground.Size = new Size(panelBackground.Size.Width, 500);
			//panelBackground.AutoScrollPosition = new Point(0, 0);
			//panelBackground.VerticalScroll.Maximum = 800;

			DoubleBuffered = true;

			BuildScrollTabs();
			SetupPanels();
			ShowDefaultMenu();

			_drivesThread = new Thread(ProcessLocalStorage);
			_drivesThread.Start();

			SetFieldsFromData();

			LogWriter.Write($"SettingsForm # Loading - Finished loading settings form.");
		}

		private void BuildScrollTabs()
		{
			panelTab0.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.One);

			foreach (Control item in panelTab0.Controls)
			{
				if (item == null) continue;

				item.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.One);
			}

			panelTab1.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.Two);

			foreach (Control item in panelTab1.Controls)
			{
				if (item == null) continue;

				item.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.Two);
			}

			panelTab2.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.Three);

			foreach (Control item in panelTab2.Controls)
			{
				if (item == null) continue;

				item.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.Three);
			}

			panelTab3.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.Four);

			foreach (Control item in panelTab3.Controls)
			{
				if (item == null) continue;

				item.Click += (o, args) => ScrollTabSelected(o, args, ScrollTabs.Four);
			}

			panelTab1.BackColor = Color.Transparent;
			panelTab2.BackColor = Color.Transparent;
			panelTab3.BackColor = Color.Transparent;
		}

		private void SetupPanels()
		{
			panelSab.Visible = false;
			panelSab.Location = new Point(160, 10);

			panelVpn.Visible = false;
			panelVpn.Location = new Point(160, 10);

			panelInterface.Visible = false;
			panelInterface.Location = new Point(160, 10);

			panelGeneral.Visible = false;
			panelGeneral.Location = new Point(160, 10);
		}

		private void ShowDefaultMenu()
		{
			ScrollTabSelected(null, null, ScrollTabs.One);
		}

		#region Every Tick Update

		public new void Update()
		{
			UpdateGeneral();
			UpdateSab();
			UpdateVpn();
			UpdateInterface();
		}

		private void UpdateGeneral()
		{

		}

		private void UpdateSab()
		{

		}

		private void UpdateVpn()
		{

		}

		private void UpdateInterface()
		{
			switch (_driveLoadState)
			{
				case DriveLoadState.Idle:
					_driveLoadState = DriveLoadState.Loading;
					break;

				case DriveLoadState.Loading:
					LogWriter.Write($"SettingsForm # UpdateInterface - Waiting for local drive data...");
					break;

				case DriveLoadState.Ready:
					if (cbmInterfaceDisk1.DataSource == null)
					{
						cbmInterfaceDisk1.DataSource = new List<DriveInfo>(_driveData);
						ProcessSavedStorage();
					}

					if (cbmInterfaceDisk2.DataSource == null)
					{
						cbmInterfaceDisk2.DataSource = new List<DriveInfo>(_driveData);
						ProcessSavedStorage();
					}

					if (cbmInterfaceDisk3.DataSource == null)
					{
						cbmInterfaceDisk3.DataSource = new List<DriveInfo>(_driveData);
						ProcessSavedStorage();
					}

					break;
			}
		}

		#endregion

		private void SetFieldsFromData()
		{
			if (_coreProfileData != null)
				SetCoreProfileFields();

			if (_sabProfileData != null)
				SetSabProfileFields();

			if (_vpnProfileData != null)
				SetVpnProfileFields();
		}

		private void SetCoreProfileFields()
		{
			cbxGeneralLoadWithWindows.Checked = _coreProfileData.StartWithWindows;
		}

		private void SetSabProfileFields()
		{
			tbxSabBinary.Text = _sabProfileData.BinaryPath;
			tbxSabApiKey.Text = _sabProfileData.ApiKey;
			tbxSabIpAddress.Text = _sabProfileData.IP;
			tbxSabPort.Text = _sabProfileData.Port.ToString();
			cbxSabRunWithoutVpn.Checked = _sabProfileData.RunWithoutVpn;
		}

		private void SetVpnProfileFields()
		{
			tbxVpnBinaryPath.Text = _vpnProfileData.BinaryPath;
			tbxVpnConfigPath.Text = _vpnProfileData.ConfigPath;
			tbxVpnUserName.Text = _vpnProfileData.Username;
			tbxVpnPassword.Text = _vpnProfileData.Password;
			cbxVpnAutoReconnect.Checked = _vpnProfileData.AutoReconnect;
			cbxVpnLoadWithWindows.Checked = _vpnProfileData.AutoConnectOnBoot;
		}

		#region Interface Settings

		private static void ProcessLocalStorage()
		{
			if (_driveLoadState == DriveLoadState.Ready) return;

			_driveLoadState = FindStorage() ? DriveLoadState.Ready : DriveLoadState.Loading;
		}

		private static bool FindStorage()
		{
			if (_driveData == null)
			{
				_driveData = new List<DriveInfo>();
			}

			LogWriter.Write($"SettingsForm # Starting local drive check...");

			var drives = DriveInfo.GetDrives();

			LogWriter.Write($"SettingsForm # Find Storage - Found {drives.Length} local drives.");

			foreach (DriveInfo item in drives.Where(item => item != null))
			{
				try
				{
					if (item.IsReady)
					{
						_driveData.Add(item);
					}
				}
				catch (Exception)
				{
				}
			}

			return _driveData != null && _driveData.Count > 0;
		}

		private void ProcessSavedStorage()
		{
			if (_driveData == null) return;

			foreach (DriveInfo drive in _driveData)
			{
				if (!string.IsNullOrWhiteSpace(_coreProfileData.Storage0))
				{
					if (drive.Name.ToUpper().Contains(_coreProfileData.Storage0))
					{
						cbmInterfaceDisk1.SelectedItem = drive;
						continue;
					}
				}

				if (!string.IsNullOrWhiteSpace(_coreProfileData.Storage1))
				{
					if (drive.Name.ToUpper().Contains(_coreProfileData.Storage1))
					{
						cbmInterfaceDisk2.SelectedItem = drive;
						continue;
					}
				}

				if (!string.IsNullOrWhiteSpace(_coreProfileData.Storage2))
				{
					if (drive.Name.ToUpper().Contains(_coreProfileData.Storage2))
					{
						cbmInterfaceDisk3.SelectedItem = drive;
						continue;
					}
				}
			}
		}

		private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			LogWriter.Write($"SettingsForm # Closing settings form.");
			_drivesThread.Abort();
		}

		#endregion

		#region Sab NZBD Settings

		private void btnSabBinaryBrowse_Click(object sender, EventArgs e)
		{
			_sabProfileData.BinaryPath =
				ShowFileBrowse(Environment.SpecialFolder.MyComputer.ToString(), "All files (*.*)|*.*");
		}

		private void tbxSabApi_TextChanged(object sender, EventArgs e)
		{
			if (!(sender is TextBox castControl)) return;
			_sabProfileData.ApiKey = castControl.Text;
		}

		private void tbxSabIp_TextChanged(object sender, EventArgs e)
		{
			if (!(sender is TextBox castControl)) return;
			_sabProfileData.IP = castControl.Text;
		}

		private void tbxSabPort_TextChanged(object sender, EventArgs e)
		{
			if (!(sender is TextBox castControl)) return;
			int.TryParse(castControl.Text, out int castInt);
			_sabProfileData.Port = castInt;
		}

		private void cbxRunWithoutVpn_CheckedChanged(object sender, EventArgs e)
		{
			_sabProfileData.RunWithoutVpn = cbxSabRunWithoutVpn.Checked;
		}

		#endregion

		#region VPN Settings

		private void btnVpnBinaryBrowse_Click(object sender, EventArgs e)
		{
			_vpnProfileData.BinaryPath = ShowFileBrowse(Environment.SpecialFolder.MyComputer.ToString(), "All files (*.*)|*.*");
		}

		private void btnVpnConfigBrowse_Click(object sender, EventArgs e)
		{
			_vpnProfileData.ConfigPath = ShowFolderBrowse();
		}

		private void tbxVpnUsername_TextChanged(object sender, EventArgs e)
		{
			if (!(sender is TextBox castControl)) return;
			_vpnProfileData.Username = castControl.Text;
		}

		private void tbxVpnPassword_TextChanged(object sender, EventArgs e)
		{
			if (!(sender is TextBox castControl)) return;
			_vpnProfileData.Password = castControl.Text;
		}

		#endregion

		private string ShowFolderBrowse(Environment.SpecialFolder rootFolder = Environment.SpecialFolder.MyComputer)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog {RootFolder = rootFolder};

			DialogResult result = dialog.ShowDialog();

			return result != DialogResult.OK ? "" : dialog.SelectedPath;
		}

		private string ShowFileBrowse(string rootFolder, string filter)
		{
			var dialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Multiselect = false,
				InitialDirectory = rootFolder,
				Filter = filter
			};

			DialogResult result = dialog.ShowDialog();

			return result != DialogResult.OK ? "" : dialog.FileName;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			_coreProfileData.Export();
			_sabProfileData.Export();
			_vpnProfileData.Export();
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		//private void cmbStorage_SelectedIndexChanged(object sender, EventArgs e)
		//{
		//	ComboBox control = sender as ComboBox;
		//	if (control == null || control.Tag == null) return;

		//	if (_ignoreEventCount < 4)
		//	{
		//		_ignoreEventCount++;
		//		return;
		//	}

		//	int tag;
		//	int.TryParse(control.Tag.ToString(), out tag);

		//	DriveInfo selectedDrive = (DriveInfo) control.SelectedItem;

		//	if (selectedDrive == null) return;

		//	switch (tag)
		//	{
		//		case 0:
		//			_data.Storage0 = selectedDrive.Name;
		//			break;

		//		case 1:
		//			_data.Storage1 = selectedDrive.Name;
		//			break;

		//		case 2:
		//			_data.Storage2 = selectedDrive.Name;
		//			break;
		//	}
		//}

		private void ScrollTabSelected(object sender, EventArgs e, ScrollTabs tabId)
		{
			panelTab0.BackColor = Color.Transparent;
			panelTab1.BackColor = Color.Transparent;
			panelTab2.BackColor = Color.Transparent;
			panelTab3.BackColor = Color.Transparent;

			Color newColor = Color.FromArgb(28, 28, 28);

			switch (tabId)
			{
				case ScrollTabs.One:
					panelTab0.BackColor = newColor;

					panelGeneral.Visible = true;
					panelSab.Visible = false;
					panelVpn.Visible = false;
					panelInterface.Visible = false;

					break;

				case ScrollTabs.Two:
					panelTab1.BackColor = newColor;

					panelGeneral.Visible = false;
					panelSab.Visible = true;
					panelVpn.Visible = false;
					panelInterface.Visible = false;

					break;

				case ScrollTabs.Three:
					panelTab2.BackColor = newColor;

					panelGeneral.Visible = false;
					panelSab.Visible = false;
					panelVpn.Visible = true;
					panelInterface.Visible = false;

					break;

				case ScrollTabs.Four:
					panelTab3.BackColor = newColor;

					panelGeneral.Visible = false;
					panelSab.Visible = false;
					panelVpn.Visible = false;
					panelInterface.Visible = true;

					break;
			}
		}
	}
}
