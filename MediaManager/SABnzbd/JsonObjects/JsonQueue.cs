using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaManager.SABnzbd.JsonObjects
{
	public class JsonQueue : IJsonBase
	{
		[JsonProperty(PropertyName = "queue")]
		public JsonQueueData Data { get; set; }
	}

	public class JsonQueueData
	{
		[JsonProperty(PropertyName = "noofslots_total")]
		public int NoOfSlotsTotal { get; set; }

		[JsonProperty(PropertyName = "paused")]
		public bool Paused { get; set; }

		[JsonProperty(PropertyName = "finish")]
		public int Finish { get; set; }

		[JsonProperty(PropertyName = "speedlimit_abs")]
		public string SpeedLimitAbs { get; set; }

		[JsonProperty(PropertyName = "slots")]
		public List<JsonQueueSlot> Slots { get; set; }

		[JsonProperty(PropertyName = "speed")]
		public string Speed { get; set; }

		[JsonProperty(PropertyName = "size")]
		public string Size { get; set; }

		[JsonProperty(PropertyName = "rating_enable")]
		public bool RatingEnabled { get; set; }

		[JsonProperty(PropertyName = "eta")]
		public string Eta { get; set; }

		[JsonProperty(PropertyName = "refresh_rate")]
		public string RefreshRate { get; set; }

		[JsonProperty(PropertyName = "start")]
		public int Start { get; set; }

		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }

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

		[JsonProperty(PropertyName = "limit")]
		public string Limit { get; set; }

		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }

		[JsonProperty(PropertyName = "have_warnings")]
		public string HaveWarnings { get; set; }

		[JsonProperty(PropertyName = "cache_art")]
		public string CachedArt { get; set; }

		[JsonProperty(PropertyName = "sizeleft")]
		public string SizeLeft { get; set; }

		[JsonProperty(PropertyName = "finishaction")]
		public object FinishAction { get; set; }

		[JsonProperty(PropertyName = "paused_all")]
		public bool PausedAll { get; set; }

		[JsonProperty(PropertyName = "quota")]
		public string Quota { get; set; }

		[JsonProperty(PropertyName = "have_quota")]
		public bool HaveQuota { get; set; }

		[JsonProperty(PropertyName = "mbleft")]
		public string MbLeft { get; set; }

		[JsonProperty(PropertyName = "categories")]
		public string[] Categories { get; set; }

		[JsonProperty(PropertyName = "timeleft")]
		public string TimeLeft { get; set; }

		[JsonProperty(PropertyName = "pause_int")]
		public string PauseInt { get; set; }

		[JsonProperty(PropertyName = "noofslots")]
		public int NoOfSlots { get; set; }

		[JsonProperty(PropertyName = "mb")]
		public string SizeMb { get; set; }

		[JsonProperty(PropertyName = "loadavg")]
		public string LoadAverage { get; set; }

		[JsonProperty(PropertyName = "cache_max")]
		public string MaxCacheSize { get; set; }

		[JsonProperty(PropertyName = "kbpersec")]
		public string KbPerSecond { get; set; }

		[JsonProperty(PropertyName = "speedlimit")]
		public string SpeedLimit { get; set; }

		[JsonProperty(PropertyName = "cache_size")]
		public string CacheSize { get; set; }

		[JsonProperty(PropertyName = "left_quota")]
		public string QuotaRemaining { get; set; }

		[JsonProperty(PropertyName = "queue_details")]
		public string QueueDetails { get; set; }

		[JsonProperty(PropertyName = "scripts")]
		public string[] Scripts { get; set; }
	}

	public class JsonQueueSlot : IJsonBase
	{
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }

		[JsonProperty(PropertyName = "index")]
		public int Index { get; set; }

		[JsonProperty(PropertyName = "password")]
		public string Password { get; set; }

		[JsonProperty(PropertyName = "missing")]
		public int Missing { get; set; }

		[JsonProperty(PropertyName = "avg_age")]
		public string AvergaeAge { get; set; }

		[JsonProperty(PropertyName = "script")]
		public string Script { get; set; }

		[JsonProperty(PropertyName = "has_rating")]
		public bool HasRating { get; set; }

		[JsonProperty(PropertyName = "mb")]
		public string Mb { get; set; }

		[JsonProperty(PropertyName = "sizeleft")]
		public string SizeMbLeft { get; set; }

		[JsonProperty(PropertyName = "filename")]
		public string FileName { get; set; }

		[JsonProperty(PropertyName = "priority")]
		public string Priority { get; set; }

		[JsonProperty(PropertyName = "cat")]
		public string Category { get; set; }

		[JsonProperty(PropertyName = "mbleft")]
		public string MbLeft { get; set; }

		[JsonProperty(PropertyName = "eta")]
		public string EstimatedTime { get; set; }

		[JsonProperty(PropertyName = "timeleft")]
		public string TimeLeft { get; set; }

		[JsonProperty(PropertyName = "percentage")]
		public string Percentage { get; set; }

		[JsonProperty(PropertyName = "nzo_id")]
		public string NzoID { get; set; }

		[JsonProperty(PropertyName = "unpackopts")]
		public string UnpackOptions { get; set; }

		[JsonProperty(PropertyName = "size")]
		public string Size { get; set; }
	}
}
