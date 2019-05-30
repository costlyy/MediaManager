using System;
using System.Windows.Forms;
using MediaManager.GUI;

namespace MediaManager
{
    static class Program
    {
		public const string CURRENT_VERSION = "v1.0.0.0a";
		
	    private static Bootstrapper _mainBoot;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			_mainBoot = new Bootstrapper();
			_mainBoot.Initialise(args);
		}
    }
}
