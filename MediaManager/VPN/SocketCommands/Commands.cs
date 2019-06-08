namespace MediaManager.VPN.SocketCommands
{
	public partial class VpnManager
	{
		public class GetStatusCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "status";
			}
		}

		public class GetStateCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "state";
			}
		}

		public class GetStateAllCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "state all";
			}
		}

		public class SetStateOnCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "state on";
			}
		}

		public class SetStateOnAllCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "state on all";
			}
		}
		public class SetStateOffCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "state off";
			}
		}

		public class GetVersionCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "version";
			}
		}

		public class GetPidCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "pid";
			}
		}

		public class SetMuteCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "mute";
			}
		}

		public class GetEchoCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "echo";
			}
		}

		public class GetHelpCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "help";
			}
		}

		public class GetNetCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "net";
			}
		}

		public class GetLogAllCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "state off"; //is this correct?
			}
		}

		public class SetLogOnCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "log on";
			}
		}

		public class SetLogOnAllCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "log on all";
			}
		}

		public class SetLogOffCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "log off";
			}
		}

		public class SetJunkCommand : VpnSocketCommand
		{
			public override string CommandText()
			{
				return "junkdata";
			}
		}
	}
}
