using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Logging;
using Newtonsoft.Json;

namespace MediaManager.SABnzbd.JsonObjects
{
	public class JsonVersion : IJsonBase
	{
		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }
	}
}
