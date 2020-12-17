using MediaManager.Core;
using MediaManager.SABnzbd.SabCommands;

namespace MediaManager.SABnzbd
{
	public interface ISabManager : IManagerAdvanced
	{
		bool StopProcess();

		bool StartProcess();

		void QueueCommand(SabClientCommand command);

		SabManager.SabHttpData GetClientData();

		void KillProcess();

		void Restart();

		bool IsConnected();

		void ToggleEnabledState(bool enabled);

		SabManager.ClientPauseState PauseState { get; }
	}
}
