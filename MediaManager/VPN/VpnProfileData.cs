using System.Collections.Generic;
using System.Xml;
using MediaManager.Core.Profile;

namespace MediaManager.VPN
{
	class VpnProfileData : ProfileData<VpnProfileData>
	{
		#region Ctor 
		private string Name { get; set; }
		private string FullPath { get; set; }

		public VpnProfileData(string path) : base(path)
		{
			Name = GetType().Name;
			FullPath = string.Format($"{path}{@"\"}{Name}{".xml"}");
		}
		#endregion

		public string BinaryPath { get; set; }
		public string ConfigPath { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string ActiveConfig { get; set; }
		public bool AutoReconnect { get; set; }
		public bool AutoConnectOnBoot { get; set; }

		public List<VpnConfig> VpnConfig { get; set; }

		public override ProfileData<VpnProfileData> Import()
		{
			if (!StartImport(Name, out XmlReader reader))
			{
				return null;
			}

			string value = reader.ReadElementString(nameof(BinaryPath));
			BinaryPath = value;

			value = reader.ReadElementString(nameof(ConfigPath));
			ConfigPath = value;

			value = reader.ReadElementString(nameof(Username));
			Username = value;

			value = reader.ReadElementString(nameof(Password));
			Password = value;

			value = reader.ReadElementString(nameof(ActiveConfig));
			ActiveConfig = value;

			value = reader.ReadElementString(nameof(AutoReconnect));
			if (bool.TryParse(value, out var boolResult))
			{
				AutoReconnect = boolResult;
			}

			value = reader.ReadElementString(nameof(AutoConnectOnBoot));
			if (bool.TryParse(value, out boolResult))
			{
				AutoConnectOnBoot = boolResult;
			}

			if (VpnConfig == null)
			{
				VpnConfig = new List<VpnConfig>(6)
				{
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig()
				};
			}

			for (int index = 0; index < 6; index++)
			{
				reader.ReadStartElement("VpnConfig" + "_" + index);

				value = reader.ReadElementString("Path");
				VpnConfig[index].Path = value;

				value = reader.ReadElementString("Priority");
				if (int.TryParse(value, out var intResult))
				{
					VpnConfig[index].Priority = intResult;
				}

				reader.ReadEndElement();
			}
		

			StopImport(reader);

			return this;
		}

		public override void Export()
		{
			XmlWriter writer = StartExport(Name);

			writer.WriteElementString(nameof(BinaryPath), BinaryPath);
			writer.WriteElementString(nameof(ConfigPath), ConfigPath);

			writer.WriteElementString(nameof(Username), Username);
			writer.WriteElementString(nameof(Password), Password);

			writer.WriteElementString(nameof(ActiveConfig), ActiveConfig);

			writer.WriteElementString(nameof(AutoReconnect), AutoReconnect.ToString());
			writer.WriteElementString(nameof(AutoConnectOnBoot), AutoConnectOnBoot.ToString());

			if (VpnConfig == null)
			{
				VpnConfig = new List<VpnConfig>(6)
				{
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig(),
					new VpnConfig()
				};
			}

			for (int index = 0; index < 6; index++)
			{
				if (VpnConfig[index] == null) continue;

				writer.WriteStartElement("VpnConfig" + "_" + index);
				writer.WriteElementString("Path", VpnConfig[index].Path);
				writer.WriteElementString("Priority", VpnConfig[index].Priority.ToString());
				writer.WriteEndElement();
			}
		

			StopExport(writer);
		}
	}

	public sealed class VpnConfig
	{
		public string Path { get; set; }
		public int Priority { get; set; }
	}
}
