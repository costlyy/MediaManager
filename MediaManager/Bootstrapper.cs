using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using MediaManager.GUI;
using MediaManager.Logging;

namespace MediaManager
{
	public class Bootstrapper
	{
		private MainForm _main;

		public void Initialise(string[] args)
		{
			LogWriter.Initialize();
			HeartbeatManager.Initialize();
			
			_main = new MainForm(args);

			HeartbeatManager.Instance.AddUpdateCall(_main.MainUpdate);
			HeartbeatManager.Instance.Start(HeartbeatManager.OperatingMode.Active);

			if (!Config.CORE_LEAK_EXCEPTIONS)
		    {
		        try
		        {
		            Application.Run(_main);
		        }
		        catch (Exception ex)
		        {
		            PrintException(ex);
		            _main.ForceCleanup();
		        }
            }
		    else
		    {
		        Application.Run(_main);
		    }
		}

		private void PrintException(Exception ex)
		{
			if (LogWriter.Exists())
			{
				LogWriter.Write("   ---   GENERAL THREAD EXCEPTION CAUGHT   ---   ");
				LogWriter.Write("");
				LogWriter.Write("");
				LogWriter.Write("   ---   START GENERAL THREAD EXCEPTION   ---   ");
				LogWriter.Write("");
				LogWriter.Write(ex.ToString());
				LogWriter.Write(" -- SYSTEM INFORMATION -- ");
				LogWriter.Write(DateTime.Now.ToString(CultureInfo.InvariantCulture));
				LogWriter.Write(Environment.MachineName);
				LogWriter.Write(Environment.CommandLine);
				LogWriter.Write(Environment.OSVersion.ToString());
				LogWriter.Write(Environment.TickCount.ToString());
				LogWriter.Write(Environment.UserName);
				LogWriter.Write(Environment.CurrentDirectory);
				LogWriter.Write("");
				LogWriter.Write("");
				LogWriter.Write("   ---   END GENERAL THREAD EXCEPTION   ---   ");

				return;
			}

			Debug.WriteLine("");
			Debug.WriteLine("   ---   START GENERAL THREAD EXCEPTION   ---   ");
			Debug.WriteLine("");
			Debug.WriteLine("");
			Debug.WriteLine(ex);
			Debug.WriteLine("\nSystem:\n");
			Debug.WriteLine(DateTime.Now);
			Debug.WriteLine(Environment.MachineName);
			Debug.WriteLine(Environment.CommandLine);
			Debug.WriteLine(Environment.OSVersion);
			Debug.WriteLine(Environment.TickCount);
			Debug.WriteLine(Environment.UserName);
			Debug.WriteLine(Environment.CurrentDirectory);
			Debug.WriteLine("");
			Debug.WriteLine("");
			Debug.WriteLine("   ---   END GENERAL THREAD EXCEPTION   ---   ");
		}
	}
}
