using System.Xml;
using MediaManager.Core.Profile;

namespace MediaManager.SABnzbd
{
	public class SabProfileData : ProfileData<SabProfileData>
	{
		#region Ctor 
		private string Name { get; set; }
		private string FullPath { get; set; }

		public SabProfileData(string path) : base(path)
		{
			Name = GetType().Name;
			FullPath = string.Format($"{path}{@"\"}{Name}{".xml"}");
		}
		#endregion

		public string BinaryPath { get; set; }
		public string IP { get; set; }
		public string ApiKey { get; set; }
		
		public int Port { get; set; }
		
		public bool RunWithoutVpn { get; set; }

		public override ProfileData<SabProfileData> Import()
		{
			if (!StartImport(Name, out XmlReader reader))
			{
				return null;
			}

			string value = reader.ReadElementString(nameof(BinaryPath));
			BinaryPath = value;

			value = reader.ReadElementString(nameof(IP));
			IP = value;

			value = reader.ReadElementString(nameof(Port));
			if (int.TryParse(value, out int intResult))
			{
				Port = intResult;
			}

			value = reader.ReadElementString(nameof(ApiKey));
			ApiKey = value;

			value = reader.ReadElementString(nameof(RunWithoutVpn));
			if (bool.TryParse(value, out bool boolResult))
			{
				RunWithoutVpn = boolResult;
			}

			StopImport(reader);

			return this;
		}

		public override void Export()
		{
			XmlWriter writer = StartExport(Name);

			writer.WriteElementString("BinaryPath", BinaryPath);
			writer.WriteElementString("IP", IP);
			writer.WriteElementString("Port", Port.ToString());
			writer.WriteElementString("ApiKey", ApiKey);
			writer.WriteElementString("RunWithoutVpn", RunWithoutVpn.ToString());

			StopExport(writer);
		}
	}
}