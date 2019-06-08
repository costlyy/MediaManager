using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace MediaManager.Logging
{
	public enum DebugPriority
	{
		High,
		Medium,
		Low,
	}

	public interface ILogWriter
	{
	}

	public class LogWriter : ILogWriter
	{
		#region Singleton

		private static ILogWriter Instance => _instance;
		private static LogWriter _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				Write("LogWriter # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new LogWriter();
		}

		#endregion

		private static StreamWriter _logStream;
		private static string _logPath;

		private LogWriter()
		{
			if (ValidateLogDirectory())
			{
				WriteInitialLogData();
			}
		}

		private bool ValidateLogDirectory()
		{
			try
			{
				string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				if (!Directory.Exists(myDocs))
				{
					Debug.WriteLine("LogWriter # Failed to locate my documents, cannot start log writer.");
					return false;
				}

				myDocs += @"\MediaManager";

				if (!Directory.Exists(myDocs))
				{
					Directory.CreateDirectory(myDocs);
				}

				string time = "";

				foreach (char c in DateTime.Now.ToLongTimeString())
				{
					if (c.Equals(':'))
					{
						time += "-";
						continue;
					}

					time += c;
				}

				string date = "";

				foreach (char c in DateTime.Now.ToShortDateString())
				{
					if (c.Equals('\\') || c.Equals('/'))
					{
						date += "-";
						continue;
					}

					date += c;
				}

				myDocs += @"\MediaMgr_" + date + "_" + time + ".log";

				_logPath = myDocs;

				return true;

			}
			catch (Exception ex)
			{
				Debug.WriteLine($"LogWriter # Encountered an exception while creating log file. \n\n{ex}");
				return false;
			}
		}

		private void WriteInitialLogData()
		{
			_logStream = File.CreateText(_logPath);
			_logStream.AutoFlush = true;

			_logStream.WriteLine(
				$"{GetTimestamp()} - {FormatPriority(DebugPriority.High)} - Started Media Manager ({Application.ProductVersion}).");

		}

		private static string GetTimestamp()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}

		private static string FormatPriority(DebugPriority priority)
		{
			switch (priority)
			{
				case DebugPriority.High:
					return "[1]";
				case DebugPriority.Medium:
					return "[2]";
				case DebugPriority.Low:
					return "[3]";
			}

			return "[0] - ";
		}

		public static bool Exists()
		{
			return _instance != null;
		}

		public static void Write(string value, DebugPriority priority = DebugPriority.Medium, bool assert = false)
		{
			try
			{
				string printValue = $"{GetTimestamp()} - {FormatPriority(priority)} - {value}";
				_logStream.WriteLine(printValue);
#if DEBUG
				Debug.WriteLine(printValue);
#endif
#if !DEBUG
				assert = false;
#endif

				if (assert)
				{
					HeartbeatManager.Instance.Stop();

					if (MessageBox.Show($"LogWriter.Write: \n\n{printValue}") == DialogResult.OK)
					{
						HeartbeatManager.Instance.Start(HeartbeatManager.OperatingMode.Active);
					}
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show($"There was an error while writing to the log. Value: \r\r{ex}", "Log Writer Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static void PrintCallstack(DebugPriority priority = DebugPriority.Medium)
		{
			try
			{
				string printValue = $"{GetTimestamp()} - {FormatPriority(priority)} - {Environment.StackTrace}";
				_logStream.WriteLine(printValue);

#if DEBUG
				Debug.WriteLine(printValue);
#endif
			}
			catch (Exception ex)
			{
				MessageBox.Show($"There was an error while writing to the log (callstack). Value: \r\r{ex}", "Log Writer Error",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
