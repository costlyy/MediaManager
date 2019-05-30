using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MediaManager.Core;
using MediaManager.Logging;
using MediaManager.SABnzbd.JsonObjects;
using MediaManager.SABnzbd.SabCommands;

namespace MediaManager.SABnzbd
{
	public partial class SabManager
	{
		public class SabHttpClient : IDisposable, ISabHttpClient
		{
			public enum ClientState
			{
				Idle,
				Connect,
				Verify,
				WaitingForCommands,
				Cleanup,
				Error,
				Reboot
			}

			public enum ClientSubState
			{
				None,
				GetVersion,
				GetStatus,
				Finished,
				Error,
			}

			public SabHttpData Data { get; }

			private ClientState _state;
			public int State => (int)_state;

			private ClientSubState _subState;

			public int Id { get; }

			private readonly SettingsData _settings;
			private HttpClient _client;

			private DateTime _bootStartTime;
			private bool _booting;

			private List<SabClientCommand> _pendingCommands;

			private bool _waitForStatus = false;
			private bool _waitForQueue = false;
			private bool _runStatusCheck = true;
			private DateTime _statusTime;

			private string _activeError;
			private string _activeErrorDetail;

			private const int BOOT_TIMER_SECONDS = 4;

			#region CTOR & Public Interface

			public bool Start()
			{
				SetState(ClientState.Connect);
				return true;
			}

			public bool Stop()
			{
				SetState(ClientState.Idle);
				return true;
			}

			public SabHttpClient(int Id, SettingsData settingData)
			{
				this.Id = Id;
				_settings = settingData;
				Data = new SabHttpData();
				LogWriter.Write($"SabClient # Constructed new SAB HttpClient ID: {Id}.");
			}

			public string GetError(ref string errorDetail)
			{
				LogWriter.Write($"SabClient # Returning error codes to caller system.");
				ProcessCleanup();
				SetState(ClientState.Idle);
				errorDetail = _activeErrorDetail;
				return _activeError;
			}

			public bool Disconnect()
			{
				if (_state != ClientState.Idle && _state != ClientState.Cleanup)
				{
					SetSubState(ClientSubState.None);
					SetState(ClientState.Cleanup);
				}

				return _state == ClientState.Idle;
			}

			public void SendCommand(SabClientCommand command)
			{
				if (_state != ClientState.WaitingForCommands)
				{
					LogWriter.Write($"SabClient # Cannot send client command yet, client not yet read. State: {_state}");
					return;
				}

				if (command == null)
				{
					LogWriter.Write($"SabClient # Cannot send client command, object null.");
					return;
				}

				if (_pendingCommands == null)
				{
					_pendingCommands = new List<SabClientCommand>();
				}
#if DEBUG
				else
				{
					if (_pendingCommands.Contains(command))
					{
						int count = _pendingCommands.Count(item => item.Equals(command));
						LogWriter.Write($"SabClient # SendCommand stack: {command.CommandText()}, {count+1} total.", DebugPriority.Low);
					}
				}
#endif

				LogWriter.Write($"SabClient # Send client command: {command.CommandText()}.");
				_pendingCommands.Add(command);
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public SabHttpData GetClientData()
			{
				return Data;
			}

		    public override string ToString()
		    {
		        return "SabNZBD HTTP Client";
		    }

		    #endregion

			public void Update()
			{
				try
				{
					switch (_state)
					{
						case ClientState.Idle:
							break;
						case ClientState.Connect:
							ProcessConnect();
							break;
						case ClientState.Verify:
							ProcessVerifyConnection();
							break;
						case ClientState.WaitingForCommands:
							ProcessWaitForCommands();
							break;
						case ClientState.Cleanup:
							ProcessCleanup();
							break;
						case ClientState.Error:
							ProcessError();
							break;
						case ClientState.Reboot:
							ProcessReboot();
							break;
					}
				}
				catch (Exception exception)
				{
					LogWriter.Write($"SabClient # Caught a general exception. \n\n{exception}", DebugPriority.Medium, true);
					_activeError = "Caught a general exception in SabSocket.";
					_activeErrorDetail = exception.ToString();
					SetState(ClientState.Error);
				}
			}

			private void SetState(ClientState newState)
			{
				LogWriter.Write($"SabClient # Setting state from {_state} to {newState}.");
				_state = newState;
				HeartbeatManager.Instance.KeepAlive();
			}

			private void SetSubState(ClientSubState newState)
			{
				LogWriter.Write($"SabClient # Setting sub-state from {_subState} to {newState}.");
				_subState = newState;
			}

			#region FSM - Initialise connection

			private void ProcessConnect()
			{
				if (string.IsNullOrEmpty(_settings.SabAPI))
				{
					LogWriter.Write($"SabClient # No API key specified, cannot connect to SABnzbd.", DebugPriority.High, true);
					SetState(ClientState.Error);
					return;
				}

				_client = new HttpClient {
					BaseAddress = new Uri($"http://{_settings.SabIP}:{_settings.SabPort}/sabnzbd/")
				};

				_client.DefaultRequestHeaders.Accept.Clear();
				_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				LogWriter.Write($"SabClient # Created HttpClient and initialised to: {_settings.SabIP}:{_settings.SabPort}/sabnzbd/ .");

				_bootStartTime = DateTime.Now;
				SetSubState(ClientSubState.None);

				SetState(ClientState.Verify);
			}

			#endregion FSM - StartProcess

			#region FSM - Verify Connection

			private void ProcessVerifyConnection()
			{
				if (_client == null)
				{
					LogWriter.Write($"SabClient # Attempted to verify a null connection. Retrying...", DebugPriority.High, true);
					SetState(ClientState.Connect);
					return;
				}

				switch (_subState)
				{
					case ClientSubState.None:
						LogWriter.Write($"SabClient # Initialising sub-state client connection verification engine.");
						SetSubState(ClientSubState.GetVersion);
						break;

					case ClientSubState.GetVersion:
						ProcessGetVersion();
						break;
					case ClientSubState.GetStatus:
						ProcessGetStatus();
						break;
					case ClientSubState.Finished:
						LogWriter.Write($"SabClient # Verified connection to SABnzbd via HttpClient, awaiting commands.");

						_runStatusCheck = true;
						_statusTime = DateTime.Now;

						SetState(ClientState.WaitingForCommands);
						break;

					case ClientSubState.Error:
						LogWriter.Write($"SabClient # Entered error state while verifying HTTP connection, exiting.");
						SetState(ClientState.Error);
						break;
				}
			}

			private async void ProcessGetVersion()
			{
				string version = "N/A";

			    try
			    {
			        version = await GetVersionAsync();
			    }
			    catch (TaskCanceledException cancelException)
			    {
			        LogWriter.Write($"SabClient # Caught TaskCanceledException while awaiting version: \n{cancelException}",
			            DebugPriority.High, true);
				    SetSubState(ClientSubState.Error);
			        return;
			    }
			    catch (HttpRequestException httpException)
			    {
			        LogWriter.Write($"SabClient # Caught HttpRequestException while awaiting version: \n{httpException}",
			            DebugPriority.High, true);
				    SetSubState(ClientSubState.Error);
					return;
			    }

			    // Delayed async calls can be thrown away.
				if (_subState == ClientSubState.GetStatus) return;

				if (JsonParser.IsValid(version))
				{
					Data.Version = new SabCommands.SabManager.GetVersion().Parse(version) as JsonVersion;
					SetSubState(ClientSubState.GetStatus);
				}
				else
				{
					LogWriter.Write($"SabClient # Failed to verify server version.");
					SetSubState(ClientSubState.Error);
				}				
			}

			private async Task<string> GetVersionAsync()
			{
				string returnValue = "";

				var cmd = new SabCommands.SabManager.GetVersion();
				HttpResponseMessage response = await _client.GetAsync($"{JsonParser.Format()}{_settings.SabAPI}{cmd.CommandText()}");

				try
				{
					response.EnsureSuccessStatusCode();
				}
				catch (HttpRequestException ex)
				{
					LogWriter.Write($"SabClient # Encountered an error when verifying GET success. Query: \n\n {JsonParser.Format()}{_settings.SabAPI}{cmd.CommandText()} \n\nError: \n\n{ex}");
				}
				
				if (response.IsSuccessStatusCode)
				{
					returnValue = await response.Content.ReadAsStringAsync();
				}

				return returnValue;
			}

			private async void ProcessGetStatus()
			{
				string status = "N/A";

				try
				{
					status = await GetStatusAsync();
				}
				catch (TaskCanceledException cancelException)
				{
					LogWriter.Write($"SabClient # Caught TaskCanceledException while awaiting status: \n{cancelException}");
					SetSubState(ClientSubState.Error);
					return;
				}
				catch (ObjectDisposedException disposedEx)
				{
					LogWriter.Write($"SabClient # Encountered an ObjectDisposedException in STATUS. Ex:\n{disposedEx}");
					SetSubState(ClientSubState.Error);
					return;
				}

				// Delayed async calls can be thrown away.
				if (_subState == ClientSubState.Finished) return;

				if (JsonParser.IsValid(status))
				{
					Data.StatusData = new SabCommands.SabManager.GetStatus().Parse(status) as JsonStatus;

					if (DateTime.Now <= _bootStartTime + TimeSpan.FromSeconds(BOOT_TIMER_SECONDS)) return;
					_bootStartTime = DateTime.Now;

					if (Data.StatusData == null || Data.StatusData.Data.Paused) return;

					LogWriter.Write($"SabClient # Connection verification complete.");
					SetSubState(ClientSubState.Finished);
				}
				else
				{
					LogWriter.Write($"SabClient # Failed to verify server status.");
				}
			}

			private async Task<string> GetStatusAsync()
			{
				string returnValue = "";

				var cmd = new SabCommands.SabManager.GetStatus();
				HttpResponseMessage response = await _client.GetAsync($"{JsonParser.Format()}{_settings.SabAPI}{cmd.CommandText()}");

				try
				{
					response.EnsureSuccessStatusCode();
				}
				catch (HttpRequestException ex)
				{
					LogWriter.Write($"SabClient # Encountered an error when verifying GET success. Query: \n\n {JsonParser.Format()}{_settings.SabAPI}{cmd.CommandText()} \n\nError: \n\n{ex}");
				}

				if (response.IsSuccessStatusCode)
				{
					returnValue = await response.Content.ReadAsStringAsync();
				}

				return returnValue;
			}

			#endregion FSM - Verify Connection

			#region FSM - Wait For Commands

			private async void ProcessWaitForCommands()
			{
				MaintainStatus();

				// Check for any commands to process.
				if (_pendingCommands == null || _pendingCommands.Count <= 0) return;

				var commands = _pendingCommands;
				SabClientCommand item = null;

				for (var index = 0; index < commands.Count; index++)
				{
					if (commands[index] == null) continue;
					item = commands[index];
					_pendingCommands.RemoveAt(index);
					break;
				}

				if (item == null) return;

				await SentCommandToClient(item);
			}

			private async void MaintainStatus()
			{
				if (!_runStatusCheck || _state != ClientState.WaitingForCommands) return;

				if (DateTime.Now <= _statusTime + TimeSpan.FromSeconds(HeartbeatManager.Instance.ExternalUpdateTimeSeconds)) return;

				_statusTime = DateTime.Now;

				LogWriter.Write($"SabClient # Started requesting SABnzbd status update.");

				string status = "N/A";

				try
				{
					status = await GetStatusAsync();
				}
				catch (TaskCanceledException taskEx)
				{
					LogWriter.Write($"SabClient # Caught TaskCanceledException while awaiting regular status: \n{taskEx}");

					if (_state != ClientState.WaitingForCommands) return;

					LogWriter.Write($"SabClient # Jumping to reboot as back-end is still running.");
					SetState(ClientState.Reboot);
				}
				catch (ObjectDisposedException disposedEx)
				{
					LogWriter.Write($"SabClient # Encountered an ObjectDisposedException in STATUS. Ex:\n{disposedEx}");
					return;
				}

				if (JsonParser.IsValid(status))
				{
					Data.StatusData = new SabCommands.SabManager.GetStatus().Parse(status) as JsonStatus;
				}

				string queue = "N/A";
				try
				{
					queue = await GetQueueAsync();
				}
				catch (TaskCanceledException taskEx)
				{
					LogWriter.Write(
						$"SabClient # Encountered a task canceled exception in QUEUE, moving to reboot SABnzbd HTTP Client. Ex:\n{taskEx}");

					if (_state != ClientState.WaitingForCommands) return;

					LogWriter.Write($"SabClient # Jumping to reboot as back-end is still running.");
					SetState(ClientState.Reboot);
				}
				catch (ObjectDisposedException disposedEx)
				{
					LogWriter.Write($"SabClient # Encountered an ObjectDisposedException in QEUEUE. Ex:\n{disposedEx}");
					return;
				}

				if (JsonParser.IsValid(queue))
				{
					Data.QueueData = new SabCommands.SabManager.GetQueue().Parse(queue) as JsonQueue;
				}

				MaintainCommonData();

				LogWriter.Write($"SabClient # Finished requesting SABnzbd status update.");
			}

			private void MaintainCommonData()
			{
				if (Data.QueueData?.Data != null)
				{
					Data.Paused = Data.QueueData.Data.Paused;
				}
				else
				{
					LogWriter.Write($"SabClient # Caught null Data.QueueData object.", DebugPriority.Low);
				}

				if (Data.StatusData?.Data != null)
				{
					Data.UpTime = Data.StatusData.Data.UpTime;
				}
				else
				{
					LogWriter.Write($"SabClient # Caught null Data.StatusData object.", DebugPriority.Low);
				}
			}

			private async Task<string> GetQueueAsync()
			{
				string returnValue = "";

				var cmd = new SabCommands.SabManager.GetQueue();
				HttpResponseMessage response = await _client.GetAsync($"{JsonParser.Format()}{_settings.SabAPI}{cmd.CommandText()}");

				try
				{
					response.EnsureSuccessStatusCode();
				}
				catch (HttpRequestException ex)
				{
					LogWriter.Write($"SabClient # Encountered an error when verifying GET success. Query: \n\n {JsonParser.Format()}{_settings.SabAPI}{cmd.CommandText()} \n\nError: \n\n{ex}");
				}

				if (response.IsSuccessStatusCode)
				{
					returnValue = await response.Content.ReadAsStringAsync();
				}

				return returnValue;
			}

			private async Task SentCommandToClient(SabClientCommand command)
			{
				HttpResponseMessage response = await _client.GetAsync($"{JsonParser.Format()}{_settings.SabAPI}{command.CommandText()}");

				try
				{
					response.EnsureSuccessStatusCode();
				}
				catch (HttpRequestException ex)
				{
					LogWriter.Write($"SabClient # Encountered an error when verifying POST success. Query: \n\n {JsonParser.Format()}{_settings.SabAPI}{command.CommandText()} \n\nError: \n\n{ex}");
				}

				if (response.IsSuccessStatusCode)
				{
					LogWriter.Write($"SabClient # Finished processing command: '{command.CommandText()}'.");
				}
			}

			#endregion FSM - Wait For Commands

			#region FSM - Process Cleanup

			private void ProcessCleanup()
			{
				_client.Dispose();
				_waitForStatus = false;
				_runStatusCheck = false;
				_client = null;

				LogWriter.Write($"SabClient # Finished cleaning up HTTP client.");
				SetState(ClientState.Idle);
			}

			#endregion FSM - Process Cleanup
			
			#region FSM - Process Reboot

			private void ProcessReboot()
			{
				_client.Dispose();
				_waitForStatus = false;
				_runStatusCheck = true;
				_client = null;

				LogWriter.Write($"SabClient # Started re-booting HTTP client.");
				SetState(ClientState.Connect);
			}

			#endregion

			#region FSM - Process Error

			public void ProcessError()
			{
				LogWriter.Write($"SabClient # Stuck in error state.");
			}

			#endregion FSM - Process Error
		}
	}
}
