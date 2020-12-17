using System.Collections.Generic;
using System.Windows.Forms;

namespace MediaManager.Core
{
	public interface IManagerAdvanced : IManagerBasic
	{
		void SetControls(List<Control> controls);

		string GetUpTime();

		void SaveData();
	}
}
