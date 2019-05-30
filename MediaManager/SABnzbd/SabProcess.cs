using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MediaManager.Core;
using MediaManager.Logging;
using MediaManager.VPN;

namespace MediaManager.SABnzbd
{
	public partial class SabManager
	{
		private class SabProcess : ISabComponent
		{
			public enum ProcessState
			{
				Idle,
				KillExisting,
				Loading,
				Running,
				Unresponsive,
				Error,
				Stopping,
			}

			private const string EVENT_NAME = "MySabEvent";
			private const int RESPONSE_TIMEOUT = 30 * 1000;

			private ProcessState _state;
			public int State => (int)_state;
			public int Id { get; }

			private Process _sabProcess;
			private readonly SettingsData _userSettings;
			private TimeSpan _processFreezeTotal;
			private DateTime _processFreezeStart;

			private int processStopFailCounter = 0;
			private int _unresponsiveCounter = 0;

			private string _activeError;
			private string _activeErrorDetail;

			private bool _startedResponseTimer;

			//private EventHandler<SabMessage> _messageHandler;

			#region CTOR & Public API

			public SabProcess(SettingsData data, int componentId = -1)
			{
				_userSettings = data;
				Id = componentId;
			}

			public void Update()
			{
				switch (_state)
				{
					case ProcessState.Idle: // Do nothing for now.
						break;
					case ProcessState.KillExisting:
						ProcessKillExisting();
						break;
					case ProcessState.Loading:
						LoadSabProcess();
						break;
					case ProcessState.Running:
						MaintainSabProcess();
						break;
					case ProcessState.Unresponsive:
						MaintainSabUnresponsive();
						break;
					case ProcessState.Stopping:
						CleanupSab();
						break;
				    case ProcessState.Error:
				        ProcessError();
				        break;
                }
			}

			public bool Start()
			{
				if (_state == ProcessState.Error)
				{
					LogWriter.Write("SabProcess # Cannot launch process until error is handled.");
					return false;
				}

				SetState(ProcessState.Loading);
				LogWriter.Write("SabProcess # Started process launch request.");

				//_messageHandler = new EventHandler<SabMessage>();

				return true;
			}

			public bool Stop()
			{
				if (_state != ProcessState.Idle && _state != ProcessState.Stopping)
				{
					SetState(ProcessState.Stopping);
				}

				return _state == ProcessState.Idle;
			}

			public string GetError(ref string errorDetail)
			{
				errorDetail = _activeErrorDetail;
				return _activeError;
			}

		    public override string ToString()
		    {
		        return "SabNZBD Process";
		    }

		    #endregion

			private void SetState(ProcessState newState)
			{
				LogWriter.Write($"SabProcess # Setting state from: {_state} to {newState}.");
				_state = newState;
				HeartbeatManager.Instance.KeepAlive();
			}

			#region FSM - Kill existing SABnzbd processes

			private void ProcessKillExisting()
			{
				List<Process> myProcesses = new List<Process>(Process.GetProcesses());

				foreach (Process item in myProcesses.Where(item => item != null))
				{
					if (string.Equals(item.ProcessName, "sabnzbd", StringComparison.InvariantCultureIgnoreCase))
					{
						LogWriter.Write($"SabProcess # Killing prior SABnzbd process: {item.Id}");
						item.Kill();
					}
				}
			}

			#endregion

			#region FSM - Process Load

			private void LoadSabProcess()
			{
				var binaryPath = _userSettings.SabBinaryPath;
				var error = "";

				if (!Helpers.SanitisePath(Helpers.PathTypes.Executable, ref binaryPath, ref error))
				{
					LogWriter.Write($"SabProcess # LoadSabProcess - Error! Failed to validate/sanitise SABnzbd binary path: {_userSettings.SabBinaryPath} (error: {error}).");
					SetState(ProcessState.Error);
					_activeError = error;
					return;
				}

				_sabProcess = new Process();

				try
				{
					_sabProcess.StartInfo.CreateNoWindow = false;
					_sabProcess.EnableRaisingEvents = true;
					_sabProcess.StartInfo.Arguments = $"--browser 0";
					_sabProcess.StartInfo.FileName = _userSettings.SabBinaryPath;
					_sabProcess.Start();

					if (_sabProcess.Responding)
					{
						LogWriter.Write("SabProcess # Process launched, move state to Running.");
						SetState(ProcessState.Running);
					}
				}
				catch (Exception exception)
				{
					LogWriter.Write("SabProcess # Error! Caught a general exception while starting SAB process.");
					LogWriter.Write(exception.ToString(), DebugPriority.Medium, true);

					SetState(ProcessState.Error);
					_activeError = Helpers.GeneralErrorCode.GENERAL_EXCEPTION;
					_activeErrorDetail = exception.ToString();
				}
			}

			#endregion

			#region FSM - Process maintain every tick

			private void MaintainSabProcess()
			{
				if (!_sabProcess.Responding)
				{
					if (_unresponsiveCounter > 2)
					{
						LogWriter.Write("SabProcess # Process not responding, move to Unresponsive state.");
						SetState(ProcessState.Unresponsive);
						_unresponsiveCounter = 0;
					}
					else
					{
						_unresponsiveCounter++;
					}
				}
			}

			#endregion

			#region FSM - Process unresponsive

			private void MaintainSabUnresponsive()
			{
				if (!_startedResponseTimer)
				{
					_processFreezeStart = DateTime.Now;
					_startedResponseTimer = true;
				}

				_processFreezeTotal = DateTime.Now - _processFreezeStart;

				if (_processFreezeTotal.TotalSeconds >= RESPONSE_TIMEOUT)
				{
					SetState(ProcessState.Loading);
					_startedResponseTimer = false;
					_processFreezeTotal = TimeSpan.Zero;
					LogWriter.Write($"SabProcess # Re-launching process after {RESPONSE_TIMEOUT}ms timeout.");
				}
			}

			#endregion

			#region FSM - Cleanup process

			private void CleanupSab()
			{
				if (processStopFailCounter < 10)
				{
					try
					{
						_sabProcess?.Kill();
					}
					catch (Exception ex)
					{
						LogWriter.Write($"SabProcess # Failed to stop process object. Exception: \n\n{ex}");
						processStopFailCounter++;
						return;
					}
				}
				else
				{
					processStopFailCounter = 0;
					LogWriter.Write("SabProcess # Failed to kill SABnzbd process after 10 attempts.", DebugPriority.High, true);
				}

				_sabProcess = null;
				_startedResponseTimer = false;

				// Check for errant sab processes that may have been loaded from elsewhere and kill those too. Don't want anything hanging around.
				List<Process> myProcesses = new List<Process>(Process.GetProcesses());
				foreach (Process item in myProcesses.Where(item => item != null))
				{
					if (string.Equals(item.ProcessName, "sabnzbd", StringComparison.InvariantCultureIgnoreCase))
					{
						try
						{
							LogWriter.Write($"SabProcess # Killing additional SABnzbd process: {item.Id}");
							item.Kill();
						}
						catch (Exception ex)
						{
							LogWriter.Write($"SabProcess # Attempted to skill additional SABnzbd process {item.Id} but encountered an error: \n\n{ex}");
						}
					}
				}

				LogWriter.Write("SabProcess # Process cleaned up, moving to idle.");
				SetState(ProcessState.Idle);
			}

            #endregion

		    #region FSM - Error

		    private void ProcessError()
		    {
		        LogWriter.Write("SabProcess # Stuck in error state.");
		    }

		    #endregion
        }
    }
}