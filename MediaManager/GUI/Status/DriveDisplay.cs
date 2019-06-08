using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Logging;

namespace MediaManager.GUI.Status
{
	public class DriveDisplay
	{
		public struct DriveTableEntry
		{
			public string DiskLabel;
			public long DiskFreeBytes;
			public float DiskFreeGB;
			public Color DiskColor;
		}

		public Dictionary<int, DriveTableEntry> DiskDrives => _diskDrivesDictionary;
		private Dictionary<int, DriveTableEntry> _diskDrivesDictionary;

		private TableLayoutPanel _controlsPanel;

		public DriveDisplay(SettingsData baseData)
		{
			_diskDrivesDictionary = new Dictionary<int, DriveTableEntry>();

			DriveTableEntry tempDrive = new DriveTableEntry
			{
				DiskLabel = baseData.Storage0
			};

			_diskDrivesDictionary.Add(0, tempDrive);

			tempDrive = new DriveTableEntry
			{
				DiskLabel = baseData.Storage1
			};

			_diskDrivesDictionary.Add(1, tempDrive);

			tempDrive = new DriveTableEntry
			{
				DiskLabel = baseData.Storage2
			};

			_diskDrivesDictionary.Add(2, tempDrive);
		}

		public void AttachControls(TableLayoutPanel tableControl)
		{
			if (tableControl == null) return;
			_controlsPanel = tableControl;
			LogWriter.Write($"DriveDisplay # Attached control panel '{tableControl.Name}' to drive display system.");
		}

		public void Update()
		{
			var localDrives = DriveInfo.GetDrives();
			Dictionary<int, DriveTableEntry> tempDict = null;
			DriveTableEntry tempEntry = new DriveTableEntry();

			foreach (DriveInfo drive in localDrives.Where(drive => drive != null))
			{
				try
				{
					foreach (var tableEntry in _diskDrivesDictionary)
					{
						// First drive storage display, specifically only for the first drive.
						if (string.IsNullOrWhiteSpace(tableEntry.Value.DiskLabel))
						{
							continue;
						}

						if (!drive.Name.ToUpper().Contains(tableEntry.Value.DiskLabel))
						{
							continue;
						}

						if (tempDict == null)
						{
							tempDict = new Dictionary<int, DriveTableEntry>();
						}

						tempEntry.DiskLabel = drive.Name.ToUpper();
						tempEntry.DiskFreeBytes = drive.TotalFreeSpace;
						tempEntry.DiskColor = GetDriveDisplayColour(tempEntry.DiskFreeBytes);
						tempEntry.DiskFreeGB = tempEntry.DiskFreeBytes / 1024f / 1024f / 1024f;

						if (tempDict.ContainsValue(tempEntry))
						{
							continue;
						}

						// / 1024f / 1024f / 1024f;
						tempDict.Add(tempDict.Count, tempEntry);
					}
				}
				catch (Exception)
				{
				}
			}

			if (tempDict != null)
			{
				_diskDrivesDictionary = tempDict;
			}

			if (_controlsPanel == null)
			{
				return;
			}

			for (int i = 0; i < _diskDrivesDictionary.Count; i++)
			{
				var label = _controlsPanel.GetControlFromPosition(i, 0);

				if (label is Label)
				{
					label.Text = _diskDrivesDictionary[i].DiskLabel;
				}

				label = _controlsPanel.GetControlFromPosition(i, 1);

				if (label is Label)
				{
					label.Text = _diskDrivesDictionary[i].DiskFreeGB.ToString(CultureInfo.InvariantCulture);
				}
			}

			

			foreach (var control in _controlsPanel.Controls)
			{

				control.ToString();
				control.ToString();
			}
		}

		private Color GetDriveDisplayColour(long freeSpaceBytes)
		{
			float spaceInGb = freeSpaceBytes / 1024f / 1024f / 1024f;

			if (spaceInGb < 10.0f)
			{
				return Color.Red;
			}

			return spaceInGb < 50.0f ? Color.Yellow : Color.White;
		}
	}
}
