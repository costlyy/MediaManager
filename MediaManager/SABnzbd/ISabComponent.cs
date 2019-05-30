using MediaManager.SABnzbd.SabCommands;

namespace MediaManager.SABnzbd
{
	public interface ISabComponent
	{
		/// <summary>
		/// Current component state. Needs to be cast to the specific component's state enum and vice-versa.
		/// </summary>
		int State { get; }

		/// <summary>
		/// Component Id.
		/// </summary>
		/// <returns></returns>
		int Id { get; }

		/// <summary>
		/// Start the component moving it into the init stages. Returns false on error, must use "GetError" before attempting 
		/// to start again.
		/// </summary>
		/// <returns></returns>
		bool Start();

		/// <summary>
		/// Start the component moving it into the init stages. Returns false on error, must use "GetError" before attempting 
		/// to start again.
		/// </summary>
		/// <returns></returns>
		bool Stop();

		/// <summary>
		/// Actively maintain the component.
		/// </summary>
		void Update();

		/// <summary>
		/// Returns a literal string with an error code. Optional ref error details available (callstack etc). 
		/// Usually required to be called after a general exception before starting the component again.
		/// </summary>
		/// <returns></returns>
		string GetError(ref string errorDetail);
	}

	public interface ISabHttpClient : ISabComponent
	{
		bool Disconnect();

		void SendCommand(SabClientCommand command);

		SabManager.SabHttpData GetClientData();
	}
}
