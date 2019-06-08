using System;
using System.Runtime.InteropServices;

namespace MediaManager.Core
{
    class Helpers
    {
		#region Public facing helpers

		public static class GeneralErrorCode
		{
			public static string GENERAL_EXCEPTION = "There was a general exception";
			public static string ARGS_EXCEPTION = "There was a InvalidArguement exception";
			public static string NO_BINARY_EXCEPTION = "There was no binary path provided";
			public static string INVALID_BINARY_EXCEPTION = "Failed to validate binary path. Check that the path is defined correctly in the settings";
			public static string NO_FILE_EXTENSION = "Extension was not found for file, unable to verify type";
			public static string UNKNOWN_FILE_EXTENSION = "Extension was not of expected type";
			public static string INVALID_PATH_FORMAT_DRIVE = "Unexpected file path format, no drive indicator found";
			public static string INVALID_PATH_FORMAT = "Unexpected file path format";
		}

		public enum PathTypes
	    {
			Executable,
			Config
	    }

        public static void ExitWindows(int flg)
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref
            htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero,
            IntPtr.Zero);
            ok = ExitWindowsEx(flg, 0);
        }

	    public static bool SanitisePath(PathTypes type, ref string path, ref string error)
	    {
		    if (string.IsNullOrWhiteSpace(path))
		    {
			    error = GeneralErrorCode.NO_BINARY_EXCEPTION;
				return false;
		    }

		    var ext = "";

			switch (type)
			{
				case PathTypes.Executable:
					ext = "exe";
					break;
				case PathTypes.Config:
					ext = "conf";
					break;
			}

		    char[] splitPath = path.ToCharArray();
		    int extPointer = 0;
		    int drivePointer = 0;

			for (var index = 0; index < splitPath.Length; index++)
		    {
			    char c = splitPath[index];

			    if (c.Equals('.'))
			    {
				    extPointer = index;
			    }

			    if (c.Equals(':'))
			    {
				    drivePointer = index;
			    }
			}

		    if (drivePointer == 0)
		    {
			    error = GeneralErrorCode.INVALID_PATH_FORMAT_DRIVE;
			    return false;
			}

		    if (drivePointer > 2 || splitPath.Length > 255)
		    {
				error = GeneralErrorCode.INVALID_PATH_FORMAT;
			    return false;
			}

		    if (extPointer == 0)
		    {
			    error = GeneralErrorCode.NO_FILE_EXTENSION;
			    return false;
			}

		    var newExt = "";

			// +1 on extPointer start point to avoid getting the dot (e.g .exe instead of exe).
			for (var index = extPointer + 1; index < splitPath.Length; index++)
			{
				newExt += splitPath[index];
			}

		    if (!string.Equals(newExt, ext, StringComparison.InvariantCultureIgnoreCase))
		    {
			    error = GeneralErrorCode.UNKNOWN_FILE_EXTENSION + $" (actual: {newExt}, expected: {ext})";
			    return false;
			}

		    error = "";
		    return true;
	    }

		#endregion /Public facing helpers

		#region EWX Windows Shutdown Logic

		private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const int TOKEN_QUERY = 0x00000008;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public const int EWX_LOGOFF = 0x00000000;
        public const int EWX_SHUTDOWN = 0x00000001;
        public const int EWX_REBOOT = 0x00000002;
        public const int EWX_FORCE = 0x00000004;
        public const int EWX_POWEROFF = 0x00000008;
        public const int EWX_FORCEIFHUNG = 0x00000010;


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr
        phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name,
        ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool ExitWindowsEx(int flg, int rea);

		#endregion /EWX Windows Shutdown Logic
	}
}
