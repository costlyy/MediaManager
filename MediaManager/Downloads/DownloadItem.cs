using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using MediaManager.Logging;
using MediaManager.SABnzbd;
using MediaManager.SABnzbd.JsonObjects;

namespace MediaManager.Downloads
{
	public class DownloadItem : IDownloadItem
	{
		public Panel PanelBackground { get; }
		public DownloadData Data { get; set; }

		private Panel PanelProgress { get; }

		private const int PROGRESS_PANEL_WIDTH = 277;
		private const float PROGRESS_PANEL_ONE_PERCENT = PROGRESS_PANEL_WIDTH / 100f;

		private const int GUI_TITLE_STRING = 1;
		private const int GUI_ITEM_TIME_REMAIN = 2;
		private const int GUI_SUB_STRING = 3;
		private const int GUI_BUTTON_FORCE = 4;
		private const int GUI_BUTTON_CANCEL = 5;

		public DownloadItem(JsonQueueSlot slotData)
		{
			Data = new DownloadData(slotData);
			LogWriter.Write($"DownloadItem[{Data.NzoID}] # Start constructing new download item.");

			PanelBackground = new Panel
			{
				BackColor = Color.FromArgb(32, 32, 32),
				Location = new Point(15, 0),
				Name = "panelDownloadItem" + Data.NzoID,
				Size = new Size(410, 64),
				TabIndex = 35,
				Visible = true
			};

			Label newLabel = new Label
			{
				AutoSize = true,
				Font =
					new Font("Liberation Sans", 14.25F, FontStyle.Regular,
						GraphicsUnit.Point, ((byte) (0))),
				ForeColor = SystemColors.Control,
				Location = new Point(7, 8),
				Name = "lblTitle",
				Size = new Size(144, 21),
				TabIndex = 30,
				Text = Data.Index + " - " + ParseFileName(Data.FileName),
				Tag = GUI_TITLE_STRING
			};
			newLabel.SendToBack();
			PanelBackground.Controls.Add(newLabel);

			newLabel = new Label
			{
				AutoSize = true,
				Font =
					new Font("Liberation Sans", 14.25F, FontStyle.Regular,
						GraphicsUnit.Point, ((byte) (0))),
				ForeColor = SystemColors.Control,
				Location = new Point(8, 34),
				Name = "lblSubTitle",
				Size = new Size(175, 21),
				TabIndex = 31,
				Text = "0.0 / 0.0",
				Tag = GUI_SUB_STRING,
			};
			PanelBackground.Controls.Add(newLabel);

			Button newButton = new Button
			{
				FlatStyle = FlatStyle.Flat,
				Font =
					new Font("Liberation Sans", 26.25F, FontStyle.Regular,
						GraphicsUnit.Point, ((byte) (0))),
				ForeColor = SystemColors.Control,
				Location = new Point(321, 3),
				Name = "btnForce",
				Size = new Size(42, 58),
				TabIndex = 37,
				Text = "↑",
				UseVisualStyleBackColor = true,
				Tag = GUI_BUTTON_FORCE,
			};
			newButton.Click += (s, e) => ForceDownloadNow(s, e, Data.NzoID);
			newButton.BringToFront();
			PanelBackground.Controls.Add(newButton);

			newButton = new Button
			{
				FlatStyle = FlatStyle.Flat,
				Font =
					new Font("Liberation Sans", 15.75F, FontStyle.Regular,
						GraphicsUnit.Point, ((byte) (0))),
				ForeColor = SystemColors.Control,
				Location = new Point(366, 3),
				Name = "btnCancel",
				Size = new Size(42, 58),
				TabIndex = 35,
				Text = "X",
				UseVisualStyleBackColor = true,
				Tag = GUI_BUTTON_CANCEL,
			};
			newButton.Click += (s, e) => CancelDownload(s, e, Data.NzoID);
			newButton.BringToFront();
			PanelBackground.Controls.Add(newButton);

			PanelProgress = new Panel
			{
				BackColor = Color.Transparent,
				Location = new Point(0, 0),
				Name = "panelProgress" + Data.NzoID,
				Size = new Size(PROGRESS_PANEL_WIDTH, 64),
				TabIndex = 35,
				Visible = true
			};
			PanelBackground.Controls.Add(PanelProgress);
			PanelProgress.SendToBack();

			LogWriter.Write($"DownloadItem[{Data.NzoID}] # Finished constructing new download item.");

			foreach (Control item in PanelBackground.Controls)
			{
				if (item == null || item.Tag == null) continue;

				int tag = 0;
				int.TryParse(item.Tag.ToString(), out tag);

				switch (tag)
				{
					case GUI_BUTTON_FORCE:
					case GUI_BUTTON_CANCEL:
						item.BringToFront();
						break;
				}
			}
		}

		private void ForceDownloadNow(object sender, EventArgs eventArgs, string ID)
		{
			if (SabManager.Instance == null)
			{
				LogWriter.Write($"DownloadItem[{ID}] # Cannot force download item, downloader not available.");
				return;
			}

			LogWriter.Write($"DownloadItem[{ID}] # Force downloading item...");
			SabManager.Instance.QueueCommand(new SABnzbd.SabCommands.SabManager.SetForceJob(ID));
		}

		private void CancelDownload(object sender, EventArgs eventArgs, string ID)
		{
			if (SabManager.Instance == null)
			{
				LogWriter.Write($"DownloadItem[{ID}] # Cannot force download item, downloader not available.");
				return;
			}

			LogWriter.Write($"DownloadItem[{ID}] # Canceling item...");
			SabManager.Instance.QueueCommand(new SABnzbd.SabCommands.SabManager.SetDeleteJob(ID));
		}

		private DownloadItem()
		{
			// Hide the default ctor.
		}

		public bool Delete()
		{
			LogWriter.Write($"DownloadItem[{Data.NzoID}] # Deleting...");
			PanelBackground.Dispose();
			return true;
		}

		public void Update()
		{
			UpdateText();
			UpdateProgressBar();
		}

		public override string ToString()
		{
			var returnString = new StringBuilder();
			returnString.Append($"[[ Download Item = {Data.NzoID} ]]");
			returnString.Append($"\n  ... FileName: {Data.FileName}");
			returnString.Append($"\n  ... Index: {Data.Index}");
			returnString.Append($"\n  ... SlotId: {Data.SlotId}");
			returnString.Append($"\n  ... Status: {Data.Status}");
			returnString.Append($"\n  ... Category: {Data.Category}");
			returnString.Append($"\n  ... EstimatedTime: {Data.EstimatedTime}");
			returnString.Append($"\n  ... AvergaeAge: {Data.AvergaeAge}");
			returnString.Append($"\n  ... HasRating: {Data.HasRating}");
			returnString.Append($"\n  ... Size: {Data.Size}");
			returnString.Append($"\n  ... MbLeft: {Data.MbLeft}");
			returnString.Append($"\n  ... Mb: {Data.Mb}");
			returnString.Append($"\n  ... SizeMbLeft: {Data.SizeMbLeft}");
			returnString.Append($"\n  ... Percentage: {Data.Percentage}");
			returnString.Append($"\n  ... TimeLeft: {Data.TimeLeft}");
			returnString.Append($"\n  ... Missing: {Data.Missing}");
			returnString.Append($"\n  ... Password: {Data.Password}");
			returnString.Append($"\n  ... UnpackOptions: {Data.UnpackOptions}");

			return returnString.ToString();
		}

		private string ParseFileName(string fileName)
		{
			char[] fileChars = fileName.ToCharArray();
			int totalSections = 0;
			var stringValue = new StringBuilder();

			for (int i = 0; i < fileChars.Length; i++)
			{
				if (fileChars[i].Equals('.'))
				{
					totalSections++;

					if (totalSections >= 5)
					{
						break;
					}
				}

				// name is too long to display, clip it.
				if (i > 35)
				{
					break;
				}

				stringValue.Append(fileChars[i]);
			}

			return stringValue.ToString();
		}

		private string ParseTimeRemaining(string timeRemaining)
		{
			char[] timeChars = timeRemaining.ToCharArray();
			int totalSections = 0;
			var stringValue = new StringBuilder();

			for (int i = 0; i < timeChars.Length; i++)
			{
				if (timeChars[i].Equals(':'))
				{
					totalSections++;

					if (totalSections >= 3)
					{
						break;
					}
				}

				stringValue.Append(timeChars[i]);
			}

			return stringValue.ToString();
		}

		private string GetSubStringDisplay()
		{
			string stringValue = "";
			float sizeMb = 0;

			try
			{
				sizeMb = float.Parse(Data.Mb, CultureInfo.InvariantCulture.NumberFormat);
			}
			catch (FormatException formatEx)
			{
			}

			float sizeLeftMb = 0;

			try
			{
				sizeLeftMb = float.Parse(Data.MbLeft, CultureInfo.InvariantCulture.NumberFormat);
			}
			catch (FormatException formatEx)
			{
			}

			float downloadedTotal = sizeMb - sizeLeftMb;
			var fixedTotal = new StringBuilder();
			char[] downloadChars = downloadedTotal.ToString(CultureInfo.InvariantCulture).ToCharArray();
			int counter = 0;

			for (int index = 0; index < downloadChars.Length; index++)
			{
				if (downloadChars[index].Equals('.') || downloadChars[index].Equals(','))
				{
					counter++;
				}

				// we only want 2 decimal places, clip the rest.
				if (counter > 3)
				{
					break;
				}

				fixedTotal.Append(downloadChars[index]);
			}

			stringValue = $"{fixedTotal}MB / {sizeMb}MB - {ParseTimeRemaining(Data.TimeLeft)}";
			return stringValue;
		}

		private void UpdateText()
		{
			foreach (Control item in PanelBackground.Controls)
			{
				if (item?.Tag == null) continue;

				int guiItemId = (int)item.Tag;
				Label castControl = null;

				switch (guiItemId)
				{
					case GUI_TITLE_STRING:
						castControl = item as Label;
						if (castControl == null) break;

						castControl.Text = Data.Index + " - " + ParseFileName(Data.FileName);
						continue;

					case GUI_SUB_STRING:
						castControl = item as Label;
						if (castControl == null) break;

						castControl.Text = GetSubStringDisplay();
						continue;
				}
			}
		}

		private void UpdateProgressBar()
		{
			float sizeMb = 0;

			try
			{
				sizeMb = float.Parse(Data.Mb, CultureInfo.InvariantCulture.NumberFormat);
			}
			catch (FormatException formatEx)
			{
			}

			float sizeLeftMb = 0;

			try
			{
				sizeLeftMb = float.Parse(Data.MbLeft, CultureInfo.InvariantCulture.NumberFormat);
			}
			catch (FormatException formatEx)
			{
			}

			if (sizeLeftMb <= 0.0f || sizeMb <= 0)
			{
				SetProgressBar(0f);
				return;
			}

			float percentage = 0;

			try
			{
				percentage = float.Parse(Data.Percentage, CultureInfo.InvariantCulture.NumberFormat);
			}
			catch (FormatException formatEx)
			{
			}

			SetProgressBar(percentage);
		}

		private void SetProgressBar(float percent)
		{
			if (PanelProgress == null)
			{
				LogWriter.Write($"DownloadItem[{Data.NzoID}] # Progress panel was null, could not update percent: {percent}");
				return;
			}

			if (percent <= 0)
			{
				PanelProgress.BackColor = Color.Transparent;
				return;
			}

			if (PanelProgress.BackColor != Color.DarkGreen)
			{
				PanelProgress.BackColor = Color.DarkGreen;
			}

			float result = percent * PROGRESS_PANEL_ONE_PERCENT;

			PanelProgress.Width = (int) result;
		}
	}
}
