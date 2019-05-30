using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MediaManager.Logging;
using MediaManager.Properties;
using Microsoft.Win32;

namespace MediaManager.Core
{
	public class SettingsData
	{
		private Settings _settings;

		public SettingsData(Settings savedSettings)
		{
			_settings = savedSettings;

			_sabBinaryPath = savedSettings.SabBinaryPath;
			_vpnConfigPath = savedSettings.ovpnConfigDir;
			_vpnBinaryPath = savedSettings.ovpnBinaryDir;
			_vpnUserName = savedSettings.vpnUserName;
			_vpnPassword = savedSettings.vpnPassword;
			_vpnAutoKill = savedSettings.vpnAutoKill;
			_vpnAutoReconnect = savedSettings.vpnAutoReconnect;
			_vpnConnectOnBoot = savedSettings.vpnConnectOnBoot;

			_vpnConfig0 = savedSettings.vpnConfig0;
			_vpnConfigPriority0 = savedSettings.vpnConfigPriority0;
			_vpnConfig1 = savedSettings.vpnConfig1;
			_vpnConfigPriority1 = savedSettings.vpnConfigPriority1;
			_vpnConfig2 = savedSettings.vpnConfig2;
			_vpnConfigPriority2 = savedSettings.vpnConfigPriority2;

			_storage0 = savedSettings.storage0;
			_storage1 = savedSettings.storage1;
			_storage2 = savedSettings.storage2;

			_sabAPI = savedSettings.sabAPI;
			_sabIP = _settings.sabIP;
			_sabPort = _settings.sabPort;

		    _vpnGlobalLogin = _settings.vpnGlobalLogin;

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
					return;
				}
				
				// Registry key values are saved as strings in Windows by default.
				string rawValue = rootKey.GetValue("LoadWithSystem") as string;

				if (rawValue == null) return;

				bool doLoadWithWindows = rawValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);

				_loadWithWindows = doLoadWithWindows;
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
			_settings.SabBinaryPath = _sabBinaryPath;
			_settings.ovpnConfigDir= _vpnConfigPath; 
			_settings.ovpnBinaryDir = _vpnBinaryPath;

			_settings.vpnUserName = _vpnUserName;
			_settings.vpnPassword = _vpnPassword;
			_settings.vpnAutoKill = _vpnAutoKill;

			_settings.vpnAutoReconnect = _vpnAutoReconnect;
			_settings.vpnConnectOnBoot = _vpnConnectOnBoot;

			_settings.vpnConfig0 = _vpnConfig0;
			_settings.vpnConfig1 = _vpnConfig1;
			_settings.vpnConfig2 = _vpnConfig2;

			_settings.vpnConfigPriority0 = _vpnConfigPriority0;
			_settings.vpnConfigPriority1 = _vpnConfigPriority1;
			_settings.vpnConfigPriority2 = _vpnConfigPriority2;

			_settings.storage0 = _storage0;
			_settings.storage1 = _storage1;
			_settings.storage2 = _storage2;

			_settings.sabAPI = _sabAPI;
			_settings.sabIP = _sabIP;
			_settings.sabPort = _sabPort;

		    _settings.vpnGlobalLogin = _vpnGlobalLogin;

			_settings.Save();

		    LogWriter.Write($"SettingsForm # Saved data settings.");

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
				rootKey.SetValue("LoadWithSystem", _loadWithWindows);
				LogWriter.Write($"Set registry entry for _loadWithWindows to {_loadWithWindows} in {rootKey}");
				rootKey.Close();
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
	            if (item == null || item.Name.Length <= 0) continue;

                try
                {
                    sb.Append($"[{item.Name}:{item.GetValue(_settings, null)}], ");
                }
                catch (TargetParameterCountException e)
                {
                }
            }
	        sb.Append(" ]");

	        return sb.ToString();
	    }

	    #region Properties

        private string _sabBinaryPath;
		public string SabBinaryPath
		{
			get { return _sabBinaryPath; }
			set { _sabBinaryPath = value; }
		}

		private string _vpnConfigPath;
		public string VpnConfigPath
		{
			get { return _vpnConfigPath; }
			set { _vpnConfigPath = value; }
		}

		private string _vpnBinaryPath;
		public string VpnBinaryPath
		{
			get { return _vpnBinaryPath; }
			set { _vpnBinaryPath = value; }
		}

		private string _vpnUserName;
		public string VpnUserName
		{
			get { return _vpnUserName; }
			set { _vpnUserName = value; }
		}

		private string _vpnPassword;
		public string VpnPassword
		{
			get { return _vpnPassword; }
			set { _vpnPassword = value; }
		}

		private string _activeVpnConfg;
		public string ActiveVpnConfg
		{
			get { return _activeVpnConfg; }
			set { _activeVpnConfg = value; }
		}

		public string ActiveVpnRootConfg => Path.Combine(_vpnConfigPath, _activeVpnConfg);

		private bool _vpnAutoKill;
		public bool VpnAutoKill
		{
			get { return _vpnAutoKill; }
			set { _vpnAutoKill = value; }
		}

		private bool _vpnAutoReconnect;
		public bool VpnAutoReconnect
		{
			get { return _vpnAutoReconnect; }
			set { _vpnAutoReconnect = value; }
		}

		private bool _vpnConnectOnBoot;
		public bool VpnConnectOnBoot
		{
			get { return _vpnConnectOnBoot; }
			set { _vpnConnectOnBoot = value; }
		}

        private bool _loadWithWindows;
        public bool LoadWithWindows
        {
            get { return _loadWithWindows; }
            set { _loadWithWindows = value; }
        }

        private string _vpnConfig0;
		public string VpnConfig0
		{
			get { return _vpnConfig0; }
			set { _vpnConfig0 = value; }
		}

		private int _vpnConfigPriority0;
		public int VpnConfigPriority0
		{
			get { return _vpnConfigPriority0; }
			set { _vpnConfigPriority0 = value; }
		}

		private string _vpnConfig1;
		public string VpnConfig1
		{
			get { return _vpnConfig1; }
			set { _vpnConfig1 = value; }
		}

		private int _vpnConfigPriority1;
		public int VpnConfigPriority1
		{
			get { return _vpnConfigPriority1; }
			set { _vpnConfigPriority1 = value; }
		}

		private string _vpnConfig2;
		public string VpnConfig2
		{
			get { return _vpnConfig2; }
			set { _vpnConfig2 = value; }
		}

		private int _vpnConfigPriority2;
		public int VpnConfigPriority2
		{
			get { return _vpnConfigPriority2; }
			set { _vpnConfigPriority2 = value; }
		}

		private string _storage0;
		public string Storage0
		{
			get { return _storage0; }
			set { _storage0 = value; }
		}

		private string _storage1;
		public string Storage1
		{
			get { return _storage1; }
			set { _storage1 = value; }
		}

		private string _storage2;
		public string Storage2
		{
			get { return _storage2; }
			set { _storage2 = value; }
		}

		private string _sabAPI;
		public string SabAPI
		{
			get { return _sabAPI; }
			set { _sabAPI = value; }
		}

		private string _sabIP;
		public string SabIP
		{
			get { return _sabIP; }
			set { _sabIP = value; }
		}

		private int _sabPort;
		public int SabPort
		{
			get { return _sabPort; }
			set { _sabPort = value; }
		}

	    private bool _vpnGlobalLogin;
	    public bool VpnGlobalLogin
        {
	        get { return _vpnGlobalLogin; }
	        set { _vpnGlobalLogin = value; }
	    }

        #endregion
    }
}
