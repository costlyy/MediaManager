using System.Collections.Generic;
using System.Windows.Forms;

namespace MediaManager.Core
{
	public interface IManager
	{
		bool Start();

		void Update();

		void SetControls(List<Control> controls);

		void SetSettings(ref SettingsData settings);

		string GetError(ref string detailedError);

		string GetUpTime();

		int State { get; }
	}
}
