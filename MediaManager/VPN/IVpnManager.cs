using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaManager.Core;
using MediaManager.VPN.SocketCommands;

namespace MediaManager.VPN
{
	public interface IVpnManager : IManager
	{
		bool Disconnect(bool killProcess = false);

		void Destroy();

		bool Connect();

		void QueueCommand(VpnSocketCommand command);

		List<VpnSocketResponse> GetCommandResponses();
	}
}
