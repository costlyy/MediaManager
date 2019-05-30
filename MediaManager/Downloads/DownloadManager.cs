using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MediaManager.Logging;
using MediaManager.SABnzbd;
using MediaManager.SABnzbd.JsonObjects;
using Newtonsoft.Json;

namespace MediaManager.Downloads
{
	public interface IDownloadManager
	{
		void Update();

		void SetData(SabManager.SabHttpData data);

		void AddItemManually(IDownloadItem item);

		void ClearAll();

		void AddParentPanel(Panel parentPanel);
	}

	public class DownloadManager : IDownloadManager
	{
		#region Singleton
		public static IDownloadManager Instance => _instance;
		private static DownloadManager _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				LogWriter.Write("DownloadManager # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new DownloadManager();
		}
		#endregion  Singleton

		private Dictionary<int, IDownloadItem> _downloadItems;
		private Panel _parentPanel;
		private int _totalDownloads = 0;

		public const int DOWNLOAD_ITEM_Y_SPACING = 89;
		public const int DOWNLOAD_ITEM_Y_DEFAULT = 152;

		private SabManager.SabHttpData _data;

		private DownloadManager()
		{
			_downloadItems = new Dictionary<int, IDownloadItem>();
		}

		public void AddItemManually(IDownloadItem item)
		{
			var newItem = item as DownloadItem;

			if (newItem == null) return;

			_parentPanel.Controls.Add(newItem.PanelBackground);
			AddDownloadItem(newItem);
		}

		public void SetData(SabManager.SabHttpData data)
		{
			if (data == null) return;
			_data = data;
		}

		public void Update()
		{
			if (_data?.QueueData?.Data?.Slots == null) return;

			var updated = false;
			var counter = 0;

			foreach (JsonQueueSlot item in _data.QueueData.Data.Slots)
			{
				foreach (KeyValuePair<int, IDownloadItem> download in _downloadItems)
				{
					if (download.Value.Data?.NzoID != item.NzoID)
					{
						continue;
					}

					download.Value.Data = new DownloadData(item);
					updated = true;
					break;
				}

				if (updated) continue;

			    foreach (Control parentItem in _parentPanel.Controls)
			    {
			        if (!string.Equals(parentItem.Tag.ToString(), "warning", StringComparison.InvariantCultureIgnoreCase)) continue;

			        parentItem.Visible = false;
			        break;
			    }

				var newItem = new DownloadItem(item);
				_parentPanel.Controls.Add(newItem.PanelBackground);
				AddDownloadItem(newItem);
				LogWriter.Write($"DownloadManager # Added new download from queue data: {item.NzoID} ({item.FileName}).");
			}

			if (_downloadItems == null) return;

			foreach (var downloadItem in _downloadItems.Where(item => item.Value != null))
			{
				downloadItem.Value.Update();
				downloadItem.Value.Data.SlotId = counter;
				counter++;
			}

			bool validEntry = false;
			var itemsToRemove = new List<KeyValuePair<int, IDownloadItem>>();

			foreach (var downloadItem in _downloadItems.Where(item => item.Value != null))
			{
				foreach (JsonQueueSlot item in _data.QueueData.Data.Slots)
				{
					if (downloadItem.Value.Data.NzoID != item.NzoID) continue;

					validEntry = true;
					break;
				}

				if (validEntry) continue;

				itemsToRemove.Add(downloadItem);
				LogWriter.Write($"DownloadManager # Adding item: {downloadItem.Value.Data.NzoID} ({downloadItem.Value.Data.FileName}) to removal queue.");
				LogWriter.Write($"  ...  FULL_DATA : {downloadItem.Value.ToString()}", DebugPriority.Low);
			}

			if (itemsToRemove.Count <= 0) return;

			foreach (var item in itemsToRemove)
			{
				if (!_downloadItems.ContainsKey(item.Key)) continue;

				if (_downloadItems.TryGetValue(item.Key, out var refItem))
				{
					refItem.Delete();
				}

				if (_downloadItems.Remove(item.Key))
				{
					LogWriter.Write($"DownloadManager # Processed removal of: {item.Value.Data.NzoID} ({item.Value.Data.FileName}).");
					ReshuffleQueue();
				}
			}

		}

		public void ClearAll()
		{
			foreach (var item in _downloadItems)
			{
				LogWriter.Write($"DownloadManager # Clear All - Removing: {item.Value.Data.NzoID} ({item.Value.Data.FileName}).");
				LogWriter.Write($"  ...  FULL_DATA : {item.Value.ToString()}", DebugPriority.Low);
				item.Value.Delete();
			}

			LogWriter.Write($"DownloadManager # Cleared {_downloadItems?.Count} items from manager.");
			_downloadItems?.Clear();
			ReshuffleQueue();
		}

		public void AddParentPanel(Panel parentPanel)
		{
			if (parentPanel == null) return;
			_parentPanel = parentPanel;
		}

		private void ReshuffleQueue()
		{
			if (_downloadItems.Count <= 0)
			{
				return;
			}

			LogWriter.Write($"DownloadManager # Re-shuffling download list/queue.");

			int position = 1; // 1 indexed NOT 0

			foreach (var kvpItem in _downloadItems)
			{
				kvpItem.Value.PanelBackground.Location =
					new Point(kvpItem.Value.PanelBackground.Location.X, GetSpacingForItem(position));
				position++;
			}
		}

		private int GetSpacingForItem(int itemNumber)
		{
			if (itemNumber <= 1) return DOWNLOAD_ITEM_Y_DEFAULT;

			return DOWNLOAD_ITEM_Y_DEFAULT + ((itemNumber - 1) * DOWNLOAD_ITEM_Y_SPACING);
		}

		private void AddDownloadItem(DownloadItem item)
		{
			if (_downloadItems == null || item == null) return;

			if (_downloadItems.Any(download => download.Value.Data.NzoID == item.Data.NzoID))
			{
				return;
			}

			item.Data.SlotId = _downloadItems.Count;
			_downloadItems.Add(_totalDownloads, item);
			_totalDownloads++;

			item.PanelBackground.Location = new Point(item.PanelBackground.Location.X, GetSpacingForItem(_downloadItems.Count));

			LogWriter.Write($"DownloadManager # Adding new download: {item.Data.NzoID} (total: {_totalDownloads}, Name: {item.Data.FileName}).");
			LogWriter.Write($"  ...  FULL_DATA : {item.ToString()}", DebugPriority.Low);
		}
	}
}
