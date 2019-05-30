using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Logging;

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

		private const int SCROLL_HIGHLIGHT_VARIANCE = 15;

		private SettingsData _data;

		private Size _backgroundOuterSize = new Size(909, 555);
		private Size _formSize = new Size(933, 580);
		private int _ignoreEventCount = 0;
		private int _scrollPosition = 0;
	    private static DriveLoadState _driveLoadState = DriveLoadState.Idle;
	    private Thread _drivesThread;
	    private static List<DriveInfo> _driveData;

	    private enum DriveLoadState
	    {
            Idle,
            Loading,
            Ready
	    }

	    public SettingsForm(ref SettingsData data)
	    {
	        LogWriter.Write($"SettingsForm # Loading - Started loading settings form.");


	        InitializeComponent();

	        Size = _formSize;
	        panelBackgroundGrey.Size = _backgroundOuterSize;
	        panelBackground.Size = new Size(panelBackground.Size.Width, 500);
	        panelBackground.AutoScrollPosition = new Point(0, 0);
	        panelBackground.VerticalScroll.Maximum = 800;
	        panelBackground.MouseWheel += panelBackground_Scroll;

	        DoubleBuffered = true;

	        BuildScrollTabs();

	        foreach (Control item in panelBackground.Controls)
	        {
	            if (item == null) continue;

	            item.MouseWheel += panelBackground_Scroll;
	        }

	        if (data == null)
	        {
	            LogWriter.Write($"SettingsForm # Loading - Creating new settings.");
	            _data = new SettingsData();
	        }
	        else
	        {
	            LogWriter.Write($"SettingsForm # Loading - Found saved settings: {data.ToString()}");
	            _data = data;
	            SetFieldsFromData();
	        }

	        _drivesThread = new Thread(ProcessLocalStorage);
	        _drivesThread.Start();

            LogWriter.Write($"SettingsForm # Loading - Finished loading settings form.");
        }

	    public new void Update()
	    {
            switch (_driveLoadState)
            {
                case DriveLoadState.Idle:
                    _driveLoadState = DriveLoadState.Loading;
                    break;

                case DriveLoadState.Loading:
                    LogWriter.Write($"SettingsForm # Update - Waiting for local drive data...");
                    break;

                case DriveLoadState.Ready:
                    if (cmbStorage1.DataSource == null)
                    {
                        cmbStorage1.DataSource = new List<DriveInfo>(_driveData);
                        ProcessSavedStorage();
                    }

                    if (cmbStorage2.DataSource == null)
                    {
                        cmbStorage2.DataSource = new List<DriveInfo>(_driveData);
                        ProcessSavedStorage();
                    }

                    if (cmbStorage3.DataSource == null)
                    {
                        cmbStorage3.DataSource = new List<DriveInfo>(_driveData);
                        ProcessSavedStorage();
                    }
                    break;
            }
        }

        private static void ProcessLocalStorage()
        {
            if (_driveLoadState == DriveLoadState.Ready) return;

            _driveLoadState = FindStorage() ? DriveLoadState.Ready : DriveLoadState.Loading;
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

		private void SetFieldsFromData()
		{
			tbxPathSab.Text = _data.SabBinaryPath;
			tbxPathOVPN.Text = _data.VpnBinaryPath;
			tbxPathOVPNConfig.Text = _data.VpnConfigPath;
			cbxVpnKill.Checked = _data.VpnAutoKill;
			cbxVpnReconnect.Checked = _data.VpnAutoReconnect;
			cbxVpnCOnnectOnLoad.Checked = _data.VpnConnectOnBoot;
			cbxLoadWithWindows.Checked = _data.LoadWithWindows;

			tbxVpnUserName.Text = _data.VpnUserName;
			tbxVpnPassword.Text = _data.VpnPassword;

			tbxSabAPI.Text = _data.SabAPI;
			tbxSabIP.Text = _data.SabIP;
			tbxSabPort.Text = _data.SabPort.ToString();

		    cbxSingleLogin.Checked = _data.VpnGlobalLogin;
		    cbxConfigLogin.Checked = !cbxSingleLogin.Checked;
		    grpLoginDetailsVpn.Enabled = _data.VpnGlobalLogin;

		}

		public SettingsData GetData()
		{
			return _data;
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
				if (!string.IsNullOrWhiteSpace(_data.Storage0))
				{
					if (drive.Name.ToUpper().Contains(_data.Storage0))
					{
						cmbStorage1.SelectedItem = drive;
						continue;
					}
				}

				if (!string.IsNullOrWhiteSpace(_data.Storage1))
				{
					if (drive.Name.ToUpper().Contains(_data.Storage1))
					{
						cmbStorage2.SelectedItem = drive;
						continue;
					}
				}

				if (!string.IsNullOrWhiteSpace(_data.Storage2))
				{
					if (drive.Name.ToUpper().Contains(_data.Storage2))
					{
						cmbStorage3.SelectedItem = drive;
						continue;
					}
				}
			}
		}

		private void btnBrowseFolder_Click(object sender, EventArgs e)
		{
		    if (!(sender is Button btnSource)) return;

			FolderBrowserDialog dialog = new FolderBrowserDialog();

			DialogResult result = dialog.ShowDialog();

			if (result != DialogResult.OK) return;
			var newPath = dialog.SelectedPath;

			if (newPath.Length <= 0)
			{
				return;
			}

		    int.TryParse(btnSource.Tag.ToString(), out var buttonTag);

			switch (buttonTag)
			{
				case 1:
					LogWriter.Write($"SettingsForm # Saving new Config Folder Path: {newPath}");
					_data.VpnConfigPath = newPath;
					break;
			}

			SetFieldsFromData();
		}
	
		private void btnBrowseFile_Click(object sender, EventArgs e)
		{
			Button btnSource = sender as Button;
			if (btnSource == null) return;

			OpenFileDialog dialog = new OpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Filter = "Binary File (*.exe)|*.exe";

			DialogResult result = dialog.ShowDialog();

			if (result != DialogResult.OK) return;
			var newPath = dialog.FileName;

			if (newPath.Length <= 0)
			{
				return;
			}

			int buttonTag;
			int.TryParse((string)btnSource.Tag, out buttonTag);

			switch (buttonTag)
			{
				case 1:
					LogWriter.Write($"SettingsForm # Saving new SABnzbd Binary Path: {newPath}");
					_data.SabBinaryPath = newPath;
					break;

				case 2:
					LogWriter.Write($"SettingsForm # Saving new OpenVPN Binary Path: {newPath}");
					_data.VpnBinaryPath = newPath;
					break;
			}

			SetFieldsFromData();
		}

		private void cbxVpnKill_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox control = sender as CheckBox;
			if (control == null) return;

			_data.VpnAutoKill = control.Checked;
		}

		private void cbxReconnect_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox control = sender as CheckBox;
			if (control == null) return;

			_data.VpnAutoReconnect = control.Checked;
		}

		private void cbxVpnCOnnectOnLoad_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox control = sender as CheckBox;
			if (control == null) return;

			_data.VpnConnectOnBoot = control.Checked;
		}

        private void cbxLoadWithWindows_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox control = sender as CheckBox;
            if (control == null) return;

            _data.LoadWithWindows = control.Checked;
        }

        private void bntSave_Click(object sender, EventArgs e)
		{
			_data.VpnUserName = tbxVpnUserName.Text;
			_data.VpnPassword = tbxVpnPassword.Text;

			_data.SabAPI = tbxSabAPI.Text;
			_data.SabIP = tbxSabIP.Text;
			_data.SabPort = Convert.ToInt32(tbxSabPort.Text);

		    _data.VpnGlobalLogin = cbxSingleLogin.Checked;

            _data.Save();
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cmbStorage_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox control = sender as ComboBox;
			if (control == null || control.Tag == null) return;

			if (_ignoreEventCount < 4)
			{
				_ignoreEventCount++;
				return;
			}

			int tag;
			int.TryParse(control.Tag.ToString(), out tag);

			DriveInfo selectedDrive = (DriveInfo) control.SelectedItem;

			if (selectedDrive == null) return;

			switch (tag)
			{
				case 0:
					_data.Storage0 = selectedDrive.Name;
					break;

				case 1:
					_data.Storage1 = selectedDrive.Name;
					break;

				case 2:
					_data.Storage2 = selectedDrive.Name;
					break;
			}
		}

		private bool IsIntWithinRange(int value, int target, int range = SCROLL_HIGHLIGHT_VARIANCE)
		{
			int max = target + range;
			int min = target - range;

			return (value >= min && value <= max);
		}

		private int GetTabLocationFromId(ScrollTabs tabId)
		{
			switch (tabId)
			{
				case ScrollTabs.One:
					return 1;
				case ScrollTabs.Two:
					return 250;
				case ScrollTabs.Three:
					return 625;
				case ScrollTabs.Four:
					return 800;
			}

			LogWriter.Write($"[Settings] GetTabLocationFromId - Invalid scroll ID: {tabId}");
			return 1;
		}

		private void panelBackground_Scroll(object sender, MouseEventArgs e)
		{
			if (e == null || e.Button != MouseButtons.None || e.Delta == 0)
			{
				return;
			}

			//// Make sure not to attempt to scroll while at maximum
			//if (Math.Abs(_scrollPosition) == panelBackground.VerticalScroll.Maximum)
			//{
			//	return;
			//}

			if (e.Delta <= -1)
			{
				if (_scrollPosition + 20 < panelBackground.VerticalScroll.Maximum)
				{
					_scrollPosition += 20;
					panelBackground.VerticalScroll.Value = _scrollPosition;
				}
				else
				{
					if (_scrollPosition != panelBackground.VerticalScroll.Maximum)
					{
						_scrollPosition = panelBackground.VerticalScroll.Maximum;
						panelBackground.AutoScrollPosition = new Point(0, _scrollPosition);
					}
				}
			}
			else
			{
				if (_scrollPosition - 20 < 0)
				{
					if (_scrollPosition != 1)
					{
						_scrollPosition = 1;
						panelBackground.VerticalScroll.Value = _scrollPosition;
					}
				}
				else
				{
					_scrollPosition = _scrollPosition - 20;
					panelBackground.AutoScrollPosition = new Point(0, _scrollPosition);
				}
			}

			// Create an acceptable variance for checking to enable tab indicators for scroll position.

			Color newColor = Color.FromArgb(32, 32, 32);

			if (IsIntWithinRange(_scrollPosition, GetTabLocationFromId(ScrollTabs.One)))
			{
				panelTab0.BackColor = newColor;
				panelTab1.BackColor = Color.Transparent;
				panelTab2.BackColor = Color.Transparent;
				panelTab3.BackColor = Color.Transparent;
			}
			else if (IsIntWithinRange(_scrollPosition, GetTabLocationFromId(ScrollTabs.Two)))
			{
				panelTab1.BackColor = newColor;
				panelTab0.BackColor = Color.Transparent;
				panelTab2.BackColor = Color.Transparent;
				panelTab3.BackColor = Color.Transparent;
			}
			else if (IsIntWithinRange(_scrollPosition, GetTabLocationFromId(ScrollTabs.Three)))
			{
				panelTab2.BackColor = newColor;
				panelTab0.BackColor = Color.Transparent;
				panelTab1.BackColor = Color.Transparent;
				panelTab3.BackColor = Color.Transparent;
			}
			else if (IsIntWithinRange(_scrollPosition, GetTabLocationFromId(ScrollTabs.Four)))
			{
				panelTab3.BackColor = newColor;
				panelTab0.BackColor = Color.Transparent;
				panelTab1.BackColor = Color.Transparent;
				panelTab2.BackColor = Color.Transparent;
			}
		}

		private void SetScrollPositionManually(ScrollTabs tabToScrollTo)
		{
			// Get the scroll values based on the tab selected.
			int targetLocation = GetTabLocationFromId(tabToScrollTo);

			// Sanity checks to prevent flashing.
			if (targetLocation <= 0)
			{
				return;
			}

			if (_scrollPosition == targetLocation)
			{
				return;
			}

			// Apply the change.
			LogWriter.Write($"[Settings] Set scroll position from: {_scrollPosition} to {targetLocation}");
			_scrollPosition = targetLocation;
			panelBackground.VerticalScroll.Value = _scrollPosition;
		}

		private void ScrollTabSelected(object sender, EventArgs e, ScrollTabs tabId)
		{
			panelTab0.BackColor = Color.Transparent;
			panelTab1.BackColor = Color.Transparent;
			panelTab2.BackColor = Color.Transparent;
			panelTab3.BackColor = Color.Transparent;

			Color newColor = Color.FromArgb(32, 32, 32);

			switch (tabId)
			{
				case ScrollTabs.One:
					panelTab0.BackColor = newColor;
					tbxPathSab.Select();
					break;

				case ScrollTabs.Two:
					panelTab1.BackColor = newColor;
					tbxPathOVPN.Select();
					break;

				case ScrollTabs.Three:
					panelTab2.BackColor = newColor;
					cmbStorage1.Select();
					break;

				case ScrollTabs.Four:
					panelTab3.BackColor = newColor;
					break;
			}

			SetScrollPositionManually(tabId);
		}

	    private void ToggleUseGlobalVpn(bool isGlobal)
	    {
	        cbxSingleLogin.Checked = isGlobal;
	        cbxConfigLogin.Checked = !isGlobal;
	        grpLoginDetailsVpn.Enabled = isGlobal;
	        LogWriter.Write($"SettingsForm # Toggle use global login to: {isGlobal}");
        }

	    private void cbxConfigLogin_Clicked(object sender, EventArgs e)
	    {
	        ToggleUseGlobalVpn(false);

	    }

	    private void cbxSingleLogin_Clicked(object sender, EventArgs e)
	    {
	        ToggleUseGlobalVpn(true);
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogWriter.Write($"SettingsForm # Closing settings form.");
            _drivesThread.Abort();
        }

		private void panelBackground_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
