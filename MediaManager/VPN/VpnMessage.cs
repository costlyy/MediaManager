namespace MediaManager.VPN
{
	public partial class VpnManager
	{
		public class VpnMessage
		{
			public enum MessageCode
			{
				MsgVpnProcessTerminated,
				MsgKillAll,
			}

			public MessageCode Code { get; private set; }

			public VpnMessage(MessageCode code)
			{
				Code = code;
			}
		}
	}
}
