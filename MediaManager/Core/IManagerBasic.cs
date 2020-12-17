using System;

namespace MediaManager.Core
{
	public interface IManagerBasic
	{
		bool Start();

		void Update();

		int State { get; }

		bool GetError(ref string errorString);

		DateTime StartTime { get; }
	}
}
