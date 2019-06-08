using MediaManager.SABnzbd.JsonObjects;

namespace MediaManager.Downloads
{
	public class DownloadData
	{
		public DownloadData(JsonQueueSlot slotData)
		{
			Status = slotData.Status;
			Index = slotData.Index;
			Password = slotData.Password;
			Missing = slotData.Missing;
			AvergaeAge = slotData.AvergaeAge;
			Script = slotData.Script;
			HasRating = slotData.HasRating;
			FileName = slotData.FileName;
			Priority = slotData.Priority;
			Category = slotData.Category;
			MbLeft = slotData.MbLeft;
			EstimatedTime = slotData.EstimatedTime;
			TimeLeft = slotData.TimeLeft;
			Percentage = slotData.Percentage;
			NzoID = slotData.NzoID;
			UnpackOptions = slotData.UnpackOptions;
			Size = slotData.Size;
			Mb = slotData.Mb;
			//SizeMbLeft = slotData.SizeMbLeft;
		}

		public int SlotId { get; set; }

		public string Status { get; set; }

		public int Index { get; set; }

		public string Password { get; set; }

		public int Missing { get; set; }

		public string AvergaeAge { get; set; }

		public string Script { get; set; }

		public bool HasRating { get; set; }

		public string Mb { get; set; }

		public float SizeMbLeft { get; set; }

		public string FileName { get; set; }

		public string Priority { get; set; }

		public string Category { get; set; }

		public string MbLeft { get; set; }

		public string EstimatedTime { get; set; }

		public string TimeLeft { get; set; }

		public string Percentage { get; set; }

		public string NzoID { get; set; }

		public string UnpackOptions { get; set; }

		public string Size { get; set; }
	}
}
