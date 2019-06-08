using System;
using System.Windows.Forms;

namespace MediaManager
{
    static class Program
    {

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
