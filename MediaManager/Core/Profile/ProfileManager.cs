using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using MediaManager.Logging;
using Newtonsoft.Json;

namespace MediaManager.Core.Profile
{
	public class ProfileManager : IManagerBasic
	{
		public enum ProfileManagerState
		{
			Idle,
			Loading,
			Running,
			Error
		}

		private ProfileManagerState _state;
		public int State => (int)_state;

		public DateTime StartTime { get; set; }

		private string _errorString = "";
		private string _errorStringDetailed = "";
		private string _profilePath = "";
		private string _basePath = "";

		#region Singleton
		private static ProfileManager _instance;
		public static ProfileManager Instance => _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				LogWriter.Write("ProfileManager # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new ProfileManager();
		}
		#endregion

		public bool Start()
		{
			switch (_state)
			{
				case ProfileManagerState.Loading:
					LogWriter.Write($"ProfileManager # Profile manager is already loading, cannot start.");
					LogWriter.PrintCallstack();
					return false;
				case ProfileManagerState.Error:
					LogWriter.Write($"ProfileManager # Failed to start ConfigManager due to error.");
					LogWriter.PrintCallstack();
					return false;
			}
			
			LogWriter.Write($"ProfileManager # Starting ProfileManager.");

			SetState(ProfileManagerState.Loading);
			StartTime = DateTime.Now;
			return true;
		}

		public void Update()
		{
			try
			{
				switch (_state)
				{
					case ProfileManagerState.Idle:
						break;
					case ProfileManagerState.Loading:
						ProcessLoadingUpdate();
						break;
					case ProfileManagerState.Running:
						ProcessRunningUpdate();
						break;
					case ProfileManagerState.Error:
						break;
				}
			}
			catch (Exception generalException)
			{
				_errorString = "There was a general exception in ProfileManager.";
				_errorStringDetailed = generalException.ToString();
				SetState(ProfileManagerState.Error);
			}
		}

		public bool GetError(ref string errorString)
		{
			if (_state != ProfileManagerState.Error) return false;

			SetState(ProfileManagerState.Idle);

			LogWriter.Write($"ProfileManager # Error dump: {_errorStringDetailed}");
			errorString = _errorString;
			return true;
		}

		public ProfileData<T> ImportData<T>()
		{
			string path = GetProfilePath();
			var data = Activator.CreateInstance<ProfileData<T>>();
			data.Import();
			return data;
		}

		public string GetProfilePath()
		{
			return _profilePath;
		}

		public bool DoFactoryReset()
		{
			if (_state == ProfileManagerState.Loading)
			{
				LogWriter.Write($"ProfileManager # DoFactoryReset waiting for manager to load.");
				return false;
			}
			
			LogWriter.Initialize();

			try
			{
				Directory.Delete(_profilePath, true);
				Directory.Delete(_basePath, true);
			}
			catch (Exception ex)
			{
				LogWriter.Write($"ProfileManager # DoFactoryReset caught general exception on reset.\n\n{ex}");
			}

			SetState(ProfileManagerState.Loading);

			LogWriter.Write($"ProfileManager # DoFactoryReset complete.");
			return true;
		}

		private void SetState(ProfileManagerState newState)
		{
			LogWriter.Write($"ProfileManager # SetState to new state: {newState} from {_state}.");
			_state = newState;
		}

		private void CreateDefaultProfile(string path)
		{
			LogWriter.Write($"ProfileManager # Could not find a default profile, creating a new one.");
			var newCoreData = new CoreProfileData(path);
			newCoreData.Export();
		}

		private void LoadDefaultProfile(string path)
		{
			LogWriter.Write($"ProfileManager # Found existing default profile");

			var newCoreData = new CoreProfileData(path);

			newCoreData.Import();
		}

		private void ProcessLoadingUpdate()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			if (!Directory.Exists(path))
			{
				_errorString = "Failed to locate 'my documents' folder";
				SetState(ProfileManagerState.Error);
				return;
			}

			path += @"\MediaManager";

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			_basePath = path;

			path += @"\Profile";

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);

				CreateDefaultProfile(path);
			}
			else
			{
				LoadDefaultProfile(path);
				
			}

			_profilePath = path;

			LogWriter.Write($"ProfileManager # Located profile at base path: {_basePath}");

			SetState(ProfileManagerState.Running);
		}

		private void ProcessRunningUpdate()
		{
		}
	}
}
