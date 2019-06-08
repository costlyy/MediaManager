using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MediaManager.Core;
using MediaManager.Logging;

namespace MediaManager.VPN
{
	public partial class VpnManager
	{
		private class VpnProcess : IVpnProcess
		{
			public enum ProcessState
			{
				Idle,
				Start,
				KillExisting,
				LoadNew,
				Running,
				Unresponsive,
				Error,
				Stopped,
				Verify,
				Destroy
			}

			private const string EVENT_NAME = "MyOpenVpnEvent";
			private const int RESPONSE_TIMEOUT = 30 * 1000;
			private const int BOOT_TIME_GRACE_SECONDS = 4;

			private ProcessState _state;
			public int State => (int) _state;
			public int Id { get; }

			private Process _vpnProcess;
			private readonly SettingsData _userSettings;
			private TimeSpan _processFreezeTotal;
			private DateTime _processFreezeStart;

			private DateTime _bootTimer;
			private bool _startedResponseTimer;

			private string _activeError;
			private string _activeErrorDetail;

			//private EventHandler<VpnMessage> _messageHandler;

			#region CTOR & Public API

			public VpnProcess(SettingsData data, int componentId = -1)
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
					case ProcessState.Start:
						MaintainStart();
						break;
					case ProcessState.KillExisting:
						MaintainKillExisting();
						break;
					case ProcessState.LoadNew:
						MaintainLoadNew();
						break;
					case ProcessState.Verify:
						MaintainVerify();
						break;
					case ProcessState.Running:
						MaintainRunning();
						break;
					case ProcessState.Unresponsive:
						MaintainUnresponsive();
						break;
					case ProcessState.Destroy:
						MaintainDestroy();
						break;
					// TODO: Why does stopped go straight back to idle? what's the point in Stopped? 03-06-18
					case ProcessState.Stopped:
						CleanupVpn();
						break;
				}
			}

			public bool Start()
			{
				if (_state == ProcessState.Error)
				{
					LogWriter.Write("VpnProcess # Cannot launch process until error is handled.");
					return false;
				}

				SetState(ProcessState.Start);
				LogWriter.Write("VpnProcess # Started process launch request.");

				//_messageHandler = new EventHandler<VpnMessage>();

				return true;
			}

			public string GetError(ref string errorDetail)
			{
				errorDetail = _activeErrorDetail;
				return _activeError;
			}

			public bool Destroy()
			{
				switch (_state)
				{
					case ProcessState.Idle:
						LogWriter.Write("VpnProcess # VPN process has been destroyed.");
						_vpnProcess = null;
						return true;

					case ProcessState.Start:
					case ProcessState.KillExisting:
					case ProcessState.LoadNew:
					case ProcessState.Running:
					case ProcessState.Unresponsive:
					case ProcessState.Verify:
						LogWriter.Write("VpnProcess # Attempting to destroy VPN process.");
						SetState(ProcessState.Destroy);
						break;
				
					case ProcessState.Error:
						LogWriter.Write("VpnProcess # Process encountered and error on destroy.");
						break;
				}

				return false;
			}

			#endregion

			#region State Wrappers

			private void SetState(ProcessState newState)
			{
				LogWriter.Write($"VpnProcess # SetState from: {_state} to new state: {newState}.");
				_state = newState;
				HeartbeatManager.Instance.KeepAlive();
			}

			#endregion

			#region VPN Process start

			private void MaintainStart()
			{
				LogWriter.Write($"VpnProcess # Starting new VPN process.");
				SetState(ProcessState.KillExisting);
			}

			#endregion

			#region VPN Process kill existing instances

			private void MaintainKillExisting()
			{
				var myProcesses = new List<Process>(Process.GetProcesses());

				bool killedAll = true;

				foreach (Process item in myProcesses.Where(item => item != null))
				{
					if (!string.Equals(item.ProcessName, "openvpn", StringComparison.InvariantCultureIgnoreCase)) continue;

					try
					{
						LogWriter.Write($"VpnProcess # Killing existing VPN process: {item.Id}.");
						item.Kill();
						killedAll = false;
					}
					catch (Exception) // We don't actually care if this fails as it might have crashed.
					{
					}
				}

				if (killedAll)
				{
					LogWriter.Write($"VpnProcess # Sanitised environment, ready to launch new process instance.");
					_vpnProcess = null; // make sure that we creaate a new process next.
					SetState(ProcessState.LoadNew);
				}
				else
				{
					LogWriter.Write($"VpnProcess # Sanitising environment...");
				}
			}

			#endregion

			#region VPN Process load new instance

			private void MaintainLoadNew()
			{
				if (_vpnProcess == null)
				{
					_vpnProcess = new Process();
				}

				var binaryPath = _userSettings.VpnBinaryPath;
				var error = "";

				if (!Helpers.SanitisePath(Helpers.PathTypes.Executable, ref binaryPath, ref error))
				{
					LogWriter.Write($"VpnProcess # LoadSabProcess - Error! Failed to validate/sanitise VPN binary path: {_userSettings.VpnBinaryPath} (error: {error}).");
					SetState(ProcessState.Error);
					_activeError = error;
					return;
				}

				try
				{
					_vpnProcess.StartInfo.CreateNoWindow = false;
					_vpnProcess.StartInfo.Arguments = $"--config \"{_userSettings.ActiveVpnRootConfg}\" --service \"{EVENT_NAME}\" 0";
					_vpnProcess.StartInfo.FileName = binaryPath;
					_vpnProcess.EnableRaisingEvents = true;

					//_vpnProcess.StartInfo.Arguments = $"--config \"{_userSettings --service \"{EVENT_NAME}\" 0";

					_vpnProcess.Start();
					LogWriter.Write("VpnProcess # Starting new VPN process.");
				}
				catch (Exception exception)
				{
					LogWriter.Write("VpnProcess # Error! Caught a general exception while starting VPN process.");
					LogWriter.Write(exception.ToString());

					SetState(ProcessState.Error);
					_activeError = VpnErrorCodes.GENERAL_EXCEPTION;
					_activeErrorDetail = exception.ToString();
				}

				try
				{
					if (!_vpnProcess.Responding) return;

					LogWriter.Write("VpnProcess # Process launched, move state to Verify.");
					_bootTimer = DateTime.Now;
					SetState(ProcessState.Verify);
				}
				catch (InvalidOperationException)
				{
					// Process hasn't loaded yet, don't worry about these exceptions.
				}
			}

			#endregion

			#region VPN Process Load Validation

			private void MaintainVerify()
			{
				if (DateTime.Now <= _bootTimer + TimeSpan.FromSeconds(BOOT_TIME_GRACE_SECONDS)) return;

				if (_vpnProcess != null)
				{
					try
					{
						if (!_vpnProcess.HasExited)
						{
							LogWriter.Write($"VpnProcess # Process verified, move state to Running. PID: {_vpnProcess.Id}");
							SetState(ProcessState.Running);
						}
						else
						{
							LogWriter.Write($"VpnProcess # Process NOT verified - process has exited with code {_vpnProcess.ExitCode}.");
							SetState(ProcessState.Error);
							_activeError = $"Process NOT verified - process has exited with code {_vpnProcess.ExitCode}.";
							_activeErrorDetail = $"Details = ExitCode: {_vpnProcess.ExitCode}, ExitTime: {_vpnProcess.ExitTime}, StartInfo.Args: {_vpnProcess.StartInfo.Arguments}, StartInfo.FileName: {_vpnProcess.StartInfo.FileName}";
						}
					}
					catch (Exception ex)
					{
						LogWriter.Write($"VpnProcess # Process NOT verified, exception while verifying existence.");
						SetState(ProcessState.Error);
						_activeError = "Process NOT verified, exception starting.";
						_activeErrorDetail = $"Details = StartInfo.Args: {_vpnProcess.StartInfo.Arguments}, StartInfo.FileName: {_vpnProcess.StartInfo.FileName}, HasExited {_vpnProcess.HasExited}, Exception: {ex}";
					}
				}
				else
				{
					LogWriter.Write($"VpnProcess # Process NOT verified, error starting.");
					SetState(ProcessState.Error);
					_activeError = "Process NOT verified, error starting.";
					_activeErrorDetail = $"Details = ExitCode: {_vpnProcess.ExitCode}, ExitTime: {_vpnProcess.ExitTime}, StartInfo.Args: {_vpnProcess.StartInfo.Arguments}, StartInfo.FileName: {_vpnProcess.StartInfo.FileName}";
				}
				
			}

			#endregion

			#region VPN Process Maintain

			private void MaintainRunning()
			{
				if (!_vpnProcess.Responding)
				{
					LogWriter.Write("VpnProcess # Process not responding, move to Unresponsive state.");
					SetState(ProcessState.Unresponsive);
				}
			}

			#endregion

			#region VPN Process Destroy

			private void MaintainDestroy()
			{
				CleanupVpn();
				LogWriter.Write("VpnProcess # Waiting for destroy to complete.");
			}

			#endregion

			#region VPN Process Unresponsive

			private void MaintainUnresponsive()
			{
				if (!_startedResponseTimer)
				{
					_processFreezeStart = DateTime.Now;
					_startedResponseTimer = true;
				}

				_processFreezeTotal = DateTime.Now - _processFreezeStart;

				if (_processFreezeTotal.TotalSeconds >= RESPONSE_TIMEOUT)
				{
					SetState(ProcessState.Start);
					_startedResponseTimer = false;
					_processFreezeTotal = TimeSpan.Zero;
					LogWriter.Write($"VpnProcess # Re-launching process after {RESPONSE_TIMEOUT}ms timeout.");
				}
			}

			#endregion

			private void CleanupVpn()
			{
				if (_vpnProcess == null)
				{
					_startedResponseTimer = false;
					LogWriter.Write("VpnProcess # Process cleaned up, moving to idle.");
					SetState(ProcessState.Idle);
					return;
				}

				try
				{
					if (!_vpnProcess.HasExited)
					{
						LogWriter.Write($"VpnProcess # Closing VPN process.");
						_vpnProcess.Kill();
						return;
					}
				}
				catch (Exception ex)
				{
					LogWriter.Write($"VpnProcess # Caught exception while closing VPN process:\n{ex}");

					// This is bad, the process handle wasn't responding to our kill request, so we need to fall back onto
					// system level management. This will ensure that no processes may have slipped through the net or been
					// dereferenced.

					var myProcesses = new List<Process>(Process.GetProcesses());

					foreach (Process item in myProcesses.Where(item => item != null))
					{
						if (!string.Equals(item.ProcessName, "openvpn", StringComparison.InvariantCultureIgnoreCase)) continue;

						try
						{
							item.Kill();
						}
						catch (Exception) // We don't actually care if this fails as it might have crashed.
						{
						}
					}
				}

				_vpnProcess = null;
				_startedResponseTimer = false;

				LogWriter.Write("VpnProcess # Process cleaned up, moving to idle.");
				SetState(ProcessState.Idle);
			}
		}
	}
}