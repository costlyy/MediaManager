using System.Collections.Generic;
using Newtonsoft.Json;

namespace MediaManager.SABnzbd.JsonObjects
{
	static class JsonParser
	{
		public static bool IsValid(string rawJSON)
		{
			if (rawJSON.Length <= 0)
			{
				return false;
			}

			if (rawJSON.Contains("Invalid API"))
			{
				return false;
			}

			return true;
		}

		public static string Format()
		{
			return "api?output=json&apikey=";
		}
	}
}
