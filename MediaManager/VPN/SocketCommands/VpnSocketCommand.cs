namespace MediaManager.VPN.SocketCommands
{
	public abstract class VpnSocketCommand
	{
		public const string COMMAND_UNKNOWN = "UnknownCommand";
		public const string COMMAND_UNIMPLEMENTED = "CommandNotImplemened";

		public VpnSocketCommand()
		{
		}

		public virtual string CommandText()
		{
			return COMMAND_UNKNOWN;
		}

	}
}
