using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaManager.SABnzbd.JsonObjects;

namespace MediaManager.SABnzbd
{
	public partial class SabManager
	{
		public class SabHttpData
		{
			public bool Paused { get; set;}
			public string UpTime { get; set; }

			public JsonVersion Version { get; set; }
			public JsonStatus StatusData { get; set; }
			public JsonQueue QueueData { get; set; }
		}
	}
}
