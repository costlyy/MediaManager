using System;
using System.Xml;
using MediaManager.Logging;
using Microsoft.Win32;

namespace MediaManager.Core.Profile
{
	public sealed class CoreProfileData : ProfileData<CoreProfileData>
	{
		#region Ctor 
		private string Name { get; set; }
		private string FullPath { get; set; }

		public CoreProfileData(string path) : base(path)
		{
			Name = GetType().Name;
			FullPath = string.Format($"{path}{@"\"}{Name}{".xml"}");
		}
		#endregion

		public bool StartWithWindows { get; set; }
		public bool AutoDeleteLogs { get; set; }
		public int AutoDeleteDays { get; set; }

		public string Storage0 { get; set; }
		public string Storage1 { get; set; }
		public string Storage2 { get; set; }
		
		public override ProfileData<CoreProfileData> Import()
		{
			if (!StartImport(Name, out XmlReader reader))
			{
				return null;
			}

			string value = reader.ReadElementString(nameof(StartWithWindows));
			if (bool.TryParse(value, out bool boolResult))
			{
				StartWithWindows = boolResult;
			}

			value = reader.ReadElementString(nameof(AutoDeleteLogs));
			if (bool.TryParse(value, out boolResult))
			{
				AutoDeleteLogs = boolResult;
			}

			value = reader.ReadElementString(nameof(AutoDeleteDays));
			if (int.TryParse(value, out int intResult))
			{
				AutoDeleteDays = intResult;
			}

			value = reader.ReadElementString(nameof(Storage0));
			Storage0 = value;

			value = reader.ReadElementString(nameof(Storage1));
			Storage1 = value;

			value = reader.ReadElementString(nameof(Storage2));
			Storage2 = value;

			StopImport(reader);

			return this;
		}

		public override void Export()
		{
			XmlWriter writer = StartExport(Name);

			writer.WriteElementString(nameof(StartWithWindows), StartWithWindows.ToString());
			writer.WriteElementString(nameof(AutoDeleteLogs), AutoDeleteLogs.ToString());
			writer.WriteElementString(nameof(AutoDeleteDays), AutoDeleteDays.ToString());

			writer.WriteElementString(nameof(Storage0), Storage0);
			writer.WriteElementString(nameof(Storage1), Storage1);
			writer.WriteElementString(nameof(Storage2), Storage2);

			StopExport(writer);

			try
			{
				RegistryKey loadKey = Registry.CurrentUser.OpenSubKey("Software", true);

				if (loadKey == null)
				{
					LogWriter.Write("CoreProfileData # Could not open user Software registry directory. Need admin rights.");
					return;
				}

				RegistryKey rootKey = loadKey.OpenSubKey("MediaManager", true);

				if (rootKey == null)
				{
					loadKey.CreateSubKey("MediaManager");
				}

				rootKey = loadKey.OpenSubKey("MediaManager", true);

				if (rootKey != null)
				{
					rootKey.SetValue("LoadWithSystem", StartWithWindows);
					LogWriter.Write($"CoreProfileData # Set registry entry for StartWithWindows to {StartWithWindows} in {rootKey}");
					rootKey.Close();
				}

				loadKey.Close();
			}
			catch (Exception ex)
			{
				LogWriter.Write($"CoreProfileData # An exception occurred while writing settings data. Value: \r\r{ex}");
				LogWriter.PrintCallstack();
			}
		}
	}
}
