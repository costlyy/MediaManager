using System;
using System.IO;
using System.Reflection;
using System.Text;
using MediaManager.Logging;
using MediaManager.Properties;
using Microsoft.Win32;

namespace MediaManager.Core
{
	public class SettingsData
	{
		private readonly Settings _settings;

		public SettingsData(Settings savedSettings)
		{
			_settings = savedSettings;

			SabBinaryPath = savedSettings.SabBinaryPath;
			VpnConfigPath = savedSettings.ovpnConfigDir;
			VpnBinaryPath = savedSettings.ovpnBinaryDir;
			VpnUserName = savedSettings.vpnUserName;
			VpnPassword = savedSettings.vpnPassword;
			VpnAutoKill = savedSettings.vpnAutoKill;
			VpnAutoReconnect = savedSettings.vpnAutoReconnect;
			VpnConnectOnBoot = savedSettings.vpnConnectOnBoot;

			VpnConfig0 = savedSettings.vpnConfig0;
			VpnConfigPriority0 = savedSettings.vpnConfigPriority0;
			VpnConfig1 = savedSettings.vpnConfig1;
			VpnConfigPriority1 = savedSettings.vpnConfigPriority1;
			VpnConfig2 = savedSettings.vpnConfig2;
			VpnConfigPriority2 = savedSettings.vpnConfigPriority2;
			VpnConfig3 = savedSettings.vpnConfig3;
			VpnConfigPriority3 = savedSettings.vpnConfigPriority3;
			VpnConfig4 = savedSettings.vpnConfig4;
			VpnConfigPriority4 = savedSettings.vpnConfigPriority4;
			VpnConfig5 = savedSettings.vpnConfig5;
			VpnConfigPriority5 = savedSettings.vpnConfigPriority5;

			Storage0 = savedSettings.storage0;
			Storage1 = savedSettings.storage1;
			Storage2 = savedSettings.storage2;

			SabApi = savedSettings.sabAPI;
			SabIP = _settings.sabIP;
			SabPort = _settings.sabPort;

		    VpnGlobalLogin = _settings.vpnGlobalLogin;

            try
			{
				RegistryKey loadKey = Registry.CurrentUser.OpenSubKey("Software", true);

				if (loadKey == null)
				{
					LogWriter.Write("Could not open user Software registry directory. Need admin rights.");
					return;
				}

				RegistryKey rootKey = loadKey.OpenSubKey("MediaManager", true);

				// Registry key values are saved as strings in Windows by default.
				string rawValue = rootKey?.GetValue("LoadWithSystem") as string;

				if (rawValue == null) return;

				bool doLoadWithWindows = rawValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);

				LoadWithWindows = doLoadWithWindows;
				LogWriter.Write($"Loaded registry entry for LoadWithSystem, value: {doLoadWithWindows}");
				rootKey.Close();
				loadKey.Close();
			}
			catch (Exception ex)
			{
				LogWriter.Write($"An exception occurred while retrieving settings data. Value: \r\r{ex}");
				LogWriter.PrintCallstack();
			}
		}

		public SettingsData()
		{
		}

		public void Save()
		{
			_settings.SabBinaryPath = SabBinaryPath;
			_settings.ovpnConfigDir= VpnConfigPath; 
			_settings.ovpnBinaryDir = VpnBinaryPath;

			_settings.vpnUserName = VpnUserName;
			_settings.vpnPassword = VpnPassword;
			_settings.vpnAutoKill = VpnAutoKill;

			_settings.vpnAutoReconnect = VpnAutoReconnect;
			_settings.vpnConnectOnBoot = VpnConnectOnBoot;

			_settings.vpnConfig0 = VpnConfig0;
			_settings.vpnConfig1 = VpnConfig1;
			_settings.vpnConfig2 = VpnConfig2;
			_settings.vpnConfig3 = VpnConfig3;
			_settings.vpnConfig4 = VpnConfig4;
			_settings.vpnConfig5 = VpnConfig5;

			_settings.vpnConfigPriority0 = VpnConfigPriority0;
			_settings.vpnConfigPriority1 = VpnConfigPriority1;
			_settings.vpnConfigPriority2 = VpnConfigPriority2;
			_settings.vpnConfigPriority3 = VpnConfigPriority3;
			_settings.vpnConfigPriority4 = VpnConfigPriority4;
			_settings.vpnConfigPriority5 = VpnConfigPriority5;

			_settings.storage0 = Storage0;
			_settings.storage1 = Storage1;
			_settings.storage2 = Storage2;

			_settings.sabAPI = SabApi;
			_settings.sabIP = SabIP;
			_settings.sabPort = SabPort;

		    _settings.vpnGlobalLogin = VpnGlobalLogin;

			_settings.Save();

		    LogWriter.Write("SettingsForm # Saved data settings.");

            // Below this point is registry entry for load with system only.
            try
			{
				RegistryKey loadKey = Registry.CurrentUser.OpenSubKey("Software", true);

				if (loadKey == null)
				{
					LogWriter.Write("Could not open user Software registry directory. Need admin rights.");
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
					rootKey.SetValue("LoadWithSystem", LoadWithWindows);
					LogWriter.Write($"Set registry entry for _loadWithWindows to {LoadWithWindows} in {rootKey}");
					rootKey.Close();
				}

				loadKey.Close();
			}
			catch (Exception ex)
			{
				LogWriter.Write($"An exception occurred while writing settings data. Value: \r\r{ex}");
				LogWriter.PrintCallstack();
			}
		}

	    public override string ToString()
	    {
	        var sb = new StringBuilder();
	        sb.Append("[  ");

            //foreach (SettingsProperty item in _settings.Properties)
	        foreach (PropertyInfo item in typeof(Settings).GetProperties())
            {
	            if (item.Name.Length <= 0) continue;

                try
                {
                    sb.Append($"[{item.Name}:{item.GetValue(_settings, null)}], ");
                }
                catch (TargetParameterCountException)
                {
                }
            }
	        sb.Append(" ]");

	        return sb.ToString();
	    }

	    #region Properties

	    public string SabBinaryPath { get; set; }

	    public string VpnConfigPath { get; set; }

	    public string VpnBinaryPath { get; set; }

	    public string VpnUserName { get; set; }

		public string VpnPassword { get; set; }

		public string ActiveVpnConfg { get; set; }

		public string ActiveVpnRootConfg => Path.Combine(VpnConfigPath, ActiveVpnConfg);

		public bool VpnAutoKill { get; set; }

		public bool VpnAutoReconnect { get; set; }

		public bool VpnConnectOnBoot { get; set; }

		public bool LoadWithWindows { get; set; }

		public string VpnConfig0 { get; set; }

		public int VpnConfigPriority0 { get; set; }

		public string VpnConfig1 { get; set; }

		public int VpnConfigPriority1 { get; set; }

		public string VpnConfig2 { get; set; }

		public int VpnConfigPriority2 { get; set; }

		public string VpnConfig3 { get; set; }

		public int VpnConfigPriority3 { get; set; }

		public string VpnConfig4 { get; set; }

		public int VpnConfigPriority4 { get; set; }

		public string VpnConfig5 { get; set; }

		public int VpnConfigPriority5 { get; set; }

		public string Storage0 { get; set; }

		public string Storage1 { get; set; }

		public string Storage2 { get; set; }

		public string SabApi { get; set; }

		public string SabIP { get; set; }

		public int SabPort { get; set; }

		public bool VpnGlobalLogin { get; set; }

	    #endregion
    }
}
