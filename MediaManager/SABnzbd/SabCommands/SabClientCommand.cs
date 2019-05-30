using MediaManager.SABnzbd.JsonObjects;
using Newtonsoft.Json.Serialization;

namespace MediaManager.SABnzbd.SabCommands
{
	public abstract class SabClientCommand
	{
		public const string COMMAND_UNKNOWN = "UnknownCommand";

		public ITraceWriter TraceWriter = new MemoryTraceWriter();

		protected SabClientCommand()
		{
		}

		public virtual string CommandText()
		{
			return COMMAND_UNKNOWN;
		}

		public virtual IJsonBase Parse(string rawJSON)
		{
			return null;
		}
	}
}
