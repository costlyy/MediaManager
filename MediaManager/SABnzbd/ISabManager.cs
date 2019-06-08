using MediaManager.Core;
using MediaManager.SABnzbd.SabCommands;

namespace MediaManager.SABnzbd
{
	public interface ISabManager : IManager
	{
		bool StopProcess();

		bool StartProcess();

		void QueueCommand(SabClientCommand command);

		SabManager.SabHttpData GetClientData();

		void Destroy();

		void Restart();
	}
}
