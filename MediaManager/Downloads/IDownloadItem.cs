using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MediaManager.SABnzbd.JsonObjects;

namespace MediaManager.Downloads
{
	public interface IDownloadItem
	{
		void Update();
		Panel PanelBackground { get; }
		DownloadData Data { get; set; }
		bool Delete();
	}
}
