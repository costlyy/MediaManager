using Newtonsoft.Json;

namespace MediaManager.SABnzbd.JsonObjects
{
	public class JsonVersion : IJsonBase
	{
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
	}
}
