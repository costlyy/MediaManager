using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
