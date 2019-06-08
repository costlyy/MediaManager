using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaManager.SABnzbd.JsonObjects
{
	public class JsonStatus : IJsonBase
	{
		[JsonProperty(PropertyName = "status")]
		public JsonStatusData Data { get; set; }
	}

	public class JsonStatusData
	{
		[JsonProperty(PropertyName = "LocalIPv4")]
		public string LocalIPv4 { get; set; }

		[JsonProperty(PropertyName = "IPv6")]
		public string LocalIPv6 { get; set; }

		[JsonProperty(PropertyName = "PublicIPv4")]
		public string PublicIPv4 { get; set; }

		[JsonProperty(PropertyName = "DNSLookup")]
		public string DNSLookup { get; set; }

		[JsonProperty(PropertyName = "Folders")]
		public List<string> Folders { get; set; }

		[JsonProperty(PropertyName = "CPUModel")]
		public string CPUModel { get; set; }

		[JsonProperty(PropertyName = "Pystone")]
		public int Pystone { get; set; }

		[JsonProperty(PropertyName = "DownloadDIR")]
		public string DownloadDir { get; set; }

		[JsonProperty(PropertyName = "DownloadDIRSpeed")]
		public int DownloadDirSpeed { get; set; }

		[JsonProperty(PropertyName = "CompleteDIR")]
		public string CompleteDir { get; set; }

		[JsonProperty(PropertyName = "CompleteDIRSpeed")]
		public int CompleteDirSpeed { get; set; }

		[JsonProperty(PropertyName = "LogLevel")]
		public int LogLevel { get; set; }

		[JsonProperty(PropertyName = "LogFile")]
		public string LogFile { get; set; }

		[JsonProperty(PropertyName = "ConfigFn")]
		public string ConfigFileName { get; set; }

		[JsonProperty(PropertyName = "NT")]
		public bool NT { get; set; }

		[JsonProperty(PropertyName = "Darwin")]
		public bool Darwin { get; set; }

		[JsonProperty(PropertyName = "HelpURI")]
		public string HelpURI { get; set; }

		[JsonProperty(PropertyName = "UpTime")]
		public string UpTime { get; set; }

		[JsonProperty(PropertyName = "Color_Scheme")]
		public string ColorScheme { get; set; }

		[JsonProperty(PropertyName = "WebDIR")]
		public string WebDir { get; set; }

		[JsonProperty(PropertyName = "Active_Lang")]
		public string ActiveLanguage { get; set; }

		[JsonProperty(PropertyName = "Restart_Req")]
		public bool RestartRequired { get; set; }

		[JsonProperty(PropertyName = "Power_Options")]
		public bool PowerOptions { get; set; }

		[JsonProperty(PropertyName = "PP_Pause_Event")]
		public bool PPPauseEvent { get; set; }

		[JsonProperty(PropertyName = "PID")]
		public int ProgramID { get; set; }

		[JsonProperty(PropertyName = "WebLogFile")]
		public string WebLogFile { get; set; }

		[JsonProperty(PropertyName = "New_Release")]
		public string NewRelease { get; set; }

		[JsonProperty(PropertyName = "New_Rel_Url")]
		public string NewReleaseUrl { get; set; }

		[JsonProperty(PropertyName = "Have_Warnings")]
		public int NumberOfWarnings { get; set; }

		[JsonProperty(PropertyName = "Warnings")]
		public List<JsonStatusWarning> Warnings { get; set; }

		[JsonProperty(PropertyName = "Servers")]
		public object Servers { get; set; }

		[JsonProperty(PropertyName = "Paused")]
		public bool Paused { get; set; }

		[JsonProperty(PropertyName = "diskspace2_norm")]
		public string DiskSpace2Norm { get; set; }

		[JsonProperty(PropertyName = "diskspace2")]
		public string DiskSpace2 { get; set; }

		[JsonProperty(PropertyName = "diskspacetotal2")]
		public string DiskSpace2Total { get; set; }

		[JsonProperty(PropertyName = "diskspace1_norm")]
		public string DiskSpace1Norm { get; set; }

		[JsonProperty(PropertyName = "diskspace1")]
		public string DiskSpace1 { get; set; }

		[JsonProperty(PropertyName = "diskspacetotal1")]
		public string DiskSpace1Total { get; set; }

		[JsonProperty(PropertyName = "session")]
		public string Session { get; set; }

		[JsonProperty(PropertyName = "cache_size")]
		public string CacheSize { get; set; }

		[JsonProperty(PropertyName = "url_base")]
		public string BaseUrl { get; set; }

		[JsonProperty(PropertyName = "my_home")]
		public string Home { get; set; }

		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }

		[JsonProperty(PropertyName = "my_lcldata")]
		public string LocalDataDir { get; set; }

		[JsonProperty(PropertyName = "cache_art")]
		public string CachedArt { get; set; }

		[JsonProperty(PropertyName = "finishaction")]
		public string FinishAction { get; set; }

		[JsonProperty(PropertyName = "paused_all")]
		public bool PausedAll { get; set; }

		[JsonProperty(PropertyName = "quota")]
		public string Quota { get; set; }

		[JsonProperty(PropertyName = "pause_int")]
		public string PauseInt { get; set; }

		[JsonProperty(PropertyName = "loadavg")]
		public string LoadAverage { get; set; }

		[JsonProperty(PropertyName = "speedlimit_abs")]
		public string SpeedLimitAbs { get; set; }

		[JsonProperty(PropertyName = "have_quota")]
		public bool HaveQuota { get; set; }

		[JsonProperty(PropertyName = "cache_max")]
		public string MaxCacheSize { get; set; }

		[JsonProperty(PropertyName = "speedlimit")]
		public string SpeedLimit { get; set; }

		[JsonProperty(PropertyName = "left_quota")]
		public string QuotaRemaining { get; set; }
	}

	public abstract class JsonStatusWarning 
	{
		[JsonProperty(PropertyName = "Text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "Type")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "Time")]
		public int Time { get; set; }
	}
	
}
