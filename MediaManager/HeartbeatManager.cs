using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaManager.Logging;

namespace MediaManager
{
	public interface IHeartbeatManager
	{
		void AddUpdateCall(Action method);

		void Start(HeartbeatManager.OperatingMode mode);

		void Stop();

		void KeepAlive();

		int ExternalUpdateTimeSeconds { get; }

		int State { get; }
	}

	public class HeartbeatManager : IHeartbeatManager
	{
		#region Singleton
		private static HeartbeatManager _instance;
		public static IHeartbeatManager Instance => _instance;

		public static void Initialize()
		{
			if (_instance != null)
			{
				LogWriter.Write("Heartbeat # Failed to Initialize, already an instance active.");
				return;
			}

			_instance = new HeartbeatManager();
		}
		#endregion

		public int State => (int)_mode;

		private List<Action> _mainUpdateItems;
		private Timer _mainUpdateTimer;
		private Timer _heartbeatTimer;
		private OperatingMode _mode;
		private DateTime _aliveTime;

		public enum OperatingMode
		{
			None,
			Active,
			Idle
		}

		public HeartbeatManager()
		{
			LogWriter.Write("Heartbeat # Constructed new Heartbeat Manager.");
			_mainUpdateItems = new List<Action>();
		}

		public int ExternalUpdateTimeSeconds
		{
			get
			{
				switch (_mode)
				{
					default:
						return Config.MAIN_IDLE_EXTERNAL_TICK_S;
					case OperatingMode.Active:
						return Config.MAIN_ACTIVE_EXTERNAL_TICK_S;
				}
			}
		}

		public void KeepAlive()
		{
			//LogWriter.Write($"Heartbeat # KeepAlive");

			_aliveTime = DateTime.Now;

			if (_mode != OperatingMode.Idle) return;

			LogWriter.Write($"Heartbeat # KeepAlive called, moving to Active.");
			LogWriter.PrintCallstack();
			ChangeMode(OperatingMode.Active);
		}

		/// <summary>
		/// Stop the main update timer (if running) and add a new call to invoke on tick
		/// </summary>
		/// <param name="method"></param>
		public void AddUpdateCall(Action method)
		{
			if (_mainUpdateItems == null)
			{
				_mainUpdateItems = new List<Action>();
				LogWriter.Write("Heartbeat # mainUpdateItems was null while adding calls - how did this happen? Resetting...");
			}

			if (method == null)
			{
				LogWriter.Write("Heartbeat # Error! Passed a NULL object to add update.");
				return;
			}

			_mainUpdateItems.Add(method);

			LogWriter.Write($"Heartbeat # Added new call to heartbeat manager: {method.Method.DeclaringType}.{method.Method.Name}()");

			if (_mainUpdateTimer == null) return;

			Start(_mode);
		}

		/// <summary>
		/// Start the main update timer
		/// </summary>
		/// <param name="mode"></param>
		public void Start(OperatingMode mode)
		{
			int updateTickMs = 100;

			switch (mode)
			{
				default:
					return;
				case OperatingMode.Active:
					updateTickMs = Config.MAIN_ACTIVE_UPDATE_TICK_MS;
					break;
				case OperatingMode.Idle:
					updateTickMs = Config.MAIN_IDLE_UPDATE_TICK_MS;
					break;
			}

			_mainUpdateTimer?.Stop();
			_mainUpdateTimer = null;
			_mainUpdateTimer = new Timer();

			_mode = mode;
			_mainUpdateTimer.Interval = updateTickMs;

			foreach (var item in _mainUpdateItems.Where(i => i != null))
			{
				_mainUpdateTimer.Tick += (e, s) => item();
			}

			_heartbeatTimer = new Timer
			{
				Interval = 500
			};

			_heartbeatTimer.Tick += (o, args) => UpdateAliveTime();
			_heartbeatTimer.Start();

			_mainUpdateTimer.Start();

			_aliveTime = DateTime.Now;

			LogWriter.Write($"Heartbeat # Starting heartbeat manager with interval: {updateTickMs} (at {_mainUpdateItems.Count} items per tick).");
		}

		/// <summary>
		/// Start the main update timer
		/// </summary>
		/// <param name="mode"></param>
		public void Stop()
		{
			_mode = OperatingMode.None;
			_mainUpdateTimer?.Stop();
			_mainUpdateTimer = null;

			LogWriter.Write($"Heartbeat # Stopped heartbeat manager.");
		}

		private void ChangeMode(OperatingMode mode)
		{
			if (mode == OperatingMode.None)
			{
				Stop();
				return;
			}

			if (_heartbeatTimer == null)
				return;

			LogWriter.Write($"Heartbeat # Changed update mode to {mode}.");

			Start(mode);

			_mode = mode;
		}

		private void UpdateAliveTime()
		{
			if (_mode != OperatingMode.Active) return;

			if (DateTime.Now <= _aliveTime + TimeSpan.FromSeconds(Config.MAIN_ACTIVE_TIMEOUT_S)) return;
			LogWriter.Write($"Heartbeat # Active mode timed out after {Config.MAIN_ACTIVE_TIMEOUT_S} seconds, returning to idle.");
			ChangeMode(OperatingMode.Idle);
		}
	}
}
