using System.Collections.Generic;
using MediaManager.Core;
using MediaManager.VPN.SocketCommands;

namespace MediaManager.VPN
{
	public interface IVpnManager : IManager
	{
		bool Disconnect(bool killProcess = false);

		void Destroy();

		bool Connect();

		void Restart();

		void QueueCommand(VpnSocketCommand command);

		List<VpnSocketResponse> GetCommandResponses();
	}
}
