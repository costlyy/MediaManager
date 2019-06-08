namespace MediaManager
{
	public class Config
	{
		public const int MAIN_ACTIVE_UPDATE_TICK_MS = 100;
		public const int MAIN_IDLE_UPDATE_TICK_MS = 3 * 1000;

		public const int MAIN_ACTIVE_EXTERNAL_TICK_S = 6;
		public const int MAIN_IDLE_EXTERNAL_TICK_S = 60 * 2;

		public const int MAIN_ACTIVE_TIMEOUT_S = 120;

		public const int MAIN_ACTIVE_LOCAL_IP_BASE = 3;
		public const int MAIN_ACTIVE_LOCAL_IP_RANGE = 3;

		public const int MAIN_IDLE_LOCAL_IP_BASE = 20;
		public const int MAIN_IDLE_LOCAL_IP_RANGE = 42;

		public const bool CORE_LEAK_EXCEPTIONS = true;
		public const bool CORE_DOWNLOADER_REQUIRES_VPN = true;
	}
}
