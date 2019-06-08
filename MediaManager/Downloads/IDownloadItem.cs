using System.Windows.Forms;

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
