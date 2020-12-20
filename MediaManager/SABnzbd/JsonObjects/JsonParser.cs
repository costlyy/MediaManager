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

			if (rawJSON.Contains("Key Incorrect"))
			{
				return false;
			}

			if (rawJSON.Contains("Key Required"))
			{
				return false;
			}

			return true;
		}

		public static bool IsKeyCorrect(string rawJSON)
		{
			if (rawJSON.Length <= 0)
			{
				return false;
			}

			return !rawJSON.Contains("Key Incorrect");
		}

		public static bool IsKeyRequired(string rawJSON)
		{
			if (rawJSON.Length <= 0)
			{
				return false;
			}

			return !rawJSON.Contains("Key Incorrect") && rawJSON.Contains("Key Required");
		}

		public static string Format()
		{
			return "api?output=json&apikey=";
		}
	}
}
