using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaManager.Core;
using MediaManager.Core.Profile;
using MediaManager.Logging;

namespace MediaManager.VPN
{
	public partial class VpnManager
	{
		private class ConfigManager : IManagerAdvanced
		{
			private const int CONTROL_ID_CONFIG_0 = 10;
			private const int CONTROL_ID_CONFIG_1 = 11;
			private const int CONTROL_ID_CONFIG_2 = 12;
			private const int CONTROL_ID_CONFIG_3 = 13;
			private const int CONTROL_ID_CONFIG_4 = 14;
			private const int CONTROL_ID_CONFIG_5 = 15;
			private const int CONTROL_ID_BROWSE_0 = 20;
			private const int CONTROL_ID_BROWSE_1 = 21;
			private const int CONTROL_ID_BROWSE_2 = 22;
			private const int CONTROL_ID_BROWSE_3 = 23;
			private const int CONTROL_ID_BROWSE_4 = 24;
			private const int CONTROL_ID_BROWSE_5 = 25;
			private const int CONTROL_ID_PRIORITY_0 = 30;
			private const int CONTROL_ID_PRIORITY_1 = 31;
			private const int CONTROL_ID_PRIORITY_2 = 32;
			private const int CONTROL_ID_PRIORITY_3 = 33;
			private const int CONTROL_ID_PRIORITY_4 = 34;
			private const int CONTROL_ID_PRIORITY_5 = 35;

			private const int LOWEST_PRIORITY = 6;
			private const int HIGHEST_PRIORITY = 1;

			public const int LOCAL_PORT = 8888;
			public const string LOCAL_HOST = "127.0.0.1";

			public enum ConfigManagerState
			{
				Idle,
				InitControls,
				CheckForConfigs,
				ValidateConfigs,
				MaintainConfigs,
				Error,
			}

			private ConfigManagerState _state;
			public int State => (int) _state;

			public DateTime StartTime { get; }

			private string _errorString;
			private string _detailedErrorString;

			private List<Control> _controls;

			private KeyValuePair<int, string> _kvpBestConfig;
			private List<KeyValuePair<int, string>> _orderedConfigs;

			private VpnProfileData _profileData;

			private List<TextBox> _configsBoxes;
			private List<Button> _configsPriorityButtons;

			#region CTOR & Public Interface

			public ConfigManager(ProfileData<VpnProfileData> profileData)
			{
				LogWriter.Write($"ConfigManager # Constructed new ConfigManager.");
				StartTime = DateTime.Now;
				_profileData = profileData as VpnProfileData;

				_configsBoxes = new List<TextBox>();
				_configsPriorityButtons = new List<Button>();
			}

			public bool Start()
			{
				if (_state == ConfigManagerState.Error)
				{
					LogWriter.Write($"ConfigManager # Failed to start ConfigManager due to error.");
					LogWriter.PrintCallstack();
					return false;
				}

				LogWriter.Write($"ConfigManager # Starting ConfigManager.");

				SetState(ConfigManagerState.InitControls);
				return true;
			}

			public void Update()
			{
				try
				{
					switch (_state)
					{
						case ConfigManagerState.Idle:
							break;
						case ConfigManagerState.InitControls:
							ProcessInitControls();
							break;
						case ConfigManagerState.CheckForConfigs:
							ProcessCheckForConfigs();
							break;
						case ConfigManagerState.ValidateConfigs:
							ProcessValidateConfigs();
							break;
						case ConfigManagerState.MaintainConfigs:
							ProcessMaintainConfigs();
							break;
						case ConfigManagerState.Error:
							break;
					}
				}
				catch (Exception generalException)
				{
					_errorString = "There was a general exception in ConfigManager.";
					_detailedErrorString = generalException.ToString();
					SetState(ConfigManagerState.Error);
				}
				
			}

			public void SetControls(List<Control> controls)
			{
				if (controls == null)
				{
					// TODO: Add error case.
					return;
				}

				_controls = controls;
			}

			public bool GetError(ref string detailedError)
			{
				if (_state != ConfigManagerState.Error) return false;

				SetState(ConfigManagerState.Idle);

				LogWriter.Write($"ConfigManager # Error dump: {_detailedErrorString}");
				detailedError = _errorString;
				return true;
			}

			public string GetUpTime()
			{
				return "00:00:00";
			}

			public void SaveData()
			{
				// TODO: save data
			}

			#endregion Public Interface

			private void SetState(ConfigManagerState newState)
			{
				LogWriter.Write($"ConfigManager # SetState from {_state} to new state: {newState}.");
				_state = newState;
			}

			private void UpdateConfigControls()
			{
				if (_configsBoxes != null)
				{
					foreach (TextBox item in _configsBoxes)
					{
						int.TryParse(item.Tag.ToString(), out var itemTag);

						string[] configSplit;

						switch (itemTag)
						{
							case CONTROL_ID_CONFIG_0:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[0].Path))
								{
									configSplit = _profileData.VpnConfig[0].Path.Split('.');
									item.Text = configSplit[0];
								}
								break;

							case CONTROL_ID_CONFIG_1:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[1].Path))
								{
									configSplit = _profileData.VpnConfig[1].Path.Split('.');
									item.Text = configSplit[0];
								}
								break;

							case CONTROL_ID_CONFIG_2:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[2].Path))
								{
									configSplit = _profileData.VpnConfig[2].Path.Split('.');
									item.Text = configSplit[0];
								}
								break;

							case CONTROL_ID_CONFIG_3:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[3].Path))
								{
									configSplit = _profileData.VpnConfig[3].Path.Split('.');
									item.Text = configSplit[0];
								}
								break;

							case CONTROL_ID_CONFIG_4:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[4].Path))
								{
									configSplit = _profileData.VpnConfig[4].Path.Split('.');
									item.Text = configSplit[0];
								}
								break;

							case CONTROL_ID_CONFIG_5:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[5].Path))
								{
									configSplit = _profileData.VpnConfig[5].Path.Split('.');
									item.Text = configSplit[0];
								}
								break;
						}
					}
				}

				if (_configsPriorityButtons == null) return;

				foreach (Button item in _configsPriorityButtons)
				{
					int.TryParse(item.Tag.ToString(), out var itemTag);

					switch (itemTag)
					{
						case CONTROL_ID_PRIORITY_0:
							item.Text = _profileData.VpnConfig[0].Path;
							break;

						case CONTROL_ID_PRIORITY_1:
							item.Text = _profileData.VpnConfig[1].Path;
							break;

						case CONTROL_ID_PRIORITY_2:
							item.Text = _profileData.VpnConfig[2].Path;
							break;

						case CONTROL_ID_PRIORITY_3:
							item.Text = _profileData.VpnConfig[3].Path;
							break;

						case CONTROL_ID_PRIORITY_4:
							item.Text = _profileData.VpnConfig[4].Path;
							break;

						case CONTROL_ID_PRIORITY_5:
							item.Text = _profileData.VpnConfig[5].Path;
							break;
					}
				}
			
			}

			#region FSM Methods

			private void ProcessInitControls()
			{
				if (_controls == null || _controls.Count <= 0)
				{
					LogWriter.Write($"ConfigManager # Warning! Cannot configure controls as they are empty/null.");
					SetState(ConfigManagerState.CheckForConfigs);
					return;
				}

				try
				{
					foreach (Control item in _controls)
					{
						if (item.Tag == null) continue;

						int.TryParse(item.Tag.ToString(), out var buttonTag);

						string[] configSplit;

						switch (buttonTag)
						{
							case CONTROL_ID_CONFIG_0:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[0].Path))
								{
									configSplit = _profileData.VpnConfig[0].Path.Split('.');
									item.Text = configSplit[0];
								}

								_configsBoxes.Add((TextBox)item);
								continue;

							case CONTROL_ID_CONFIG_1:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[1].Path))
								{
									configSplit = _profileData.VpnConfig[1].Path.Split('.');
									item.Text = configSplit[0];
								}
								_configsBoxes.Add((TextBox)item);
								continue;

							case CONTROL_ID_CONFIG_2:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[2].Path))
								{
									configSplit = _profileData.VpnConfig[2].Path.Split('.');
									item.Text = configSplit[0];
								}
								_configsBoxes.Add((TextBox)item);
								continue;

							case CONTROL_ID_CONFIG_3:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[3].Path))
								{
									configSplit = _profileData.VpnConfig[3].Path.Split('.');
									item.Text = configSplit[0];
								}

								_configsBoxes.Add((TextBox)item);
								continue;

							case CONTROL_ID_CONFIG_4:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[4].Path))
								{
									configSplit = _profileData.VpnConfig[4].Path.Split('.');
									item.Text = configSplit[0];
								}
								_configsBoxes.Add((TextBox)item);
								continue;

							case CONTROL_ID_CONFIG_5:
								if (!string.IsNullOrWhiteSpace(_profileData.VpnConfig[5].Path))
								{
									configSplit = _profileData.VpnConfig[5].Path.Split('.');
									item.Text = configSplit[0];
								}
								_configsBoxes.Add((TextBox)item);
								continue;

							case CONTROL_ID_BROWSE_0:
							case CONTROL_ID_BROWSE_1:
							case CONTROL_ID_BROWSE_2:
							case CONTROL_ID_BROWSE_3:
							case CONTROL_ID_BROWSE_4:
							case CONTROL_ID_BROWSE_5:
								item.Click += (sender, args) => ConfigButtonClick((Button)sender, args);
								continue;

							case CONTROL_ID_PRIORITY_0:
							case CONTROL_ID_PRIORITY_1:
							case CONTROL_ID_PRIORITY_2:
							case CONTROL_ID_PRIORITY_3:
							case CONTROL_ID_PRIORITY_4:
							case CONTROL_ID_PRIORITY_5:
								item.MouseDown += (sender, args) => PriorityButtonClick((Button)sender, args);
								_configsPriorityButtons.Add((Button) item);

								switch (buttonTag)
								{
									case CONTROL_ID_PRIORITY_0:
										item.Text = _profileData.VpnConfig[0].Priority.ToString();
										continue;
									case CONTROL_ID_PRIORITY_1:
										item.Text = _profileData.VpnConfig[1].Priority.ToString();
										continue;
									case CONTROL_ID_PRIORITY_2:
										item.Text = _profileData.VpnConfig[2].Priority.ToString();
										continue;
									case CONTROL_ID_PRIORITY_3:
										item.Text = _profileData.VpnConfig[3].Priority.ToString();
										continue;
									case CONTROL_ID_PRIORITY_4:
										item.Text = _profileData.VpnConfig[4].Priority.ToString();
										continue;
									case CONTROL_ID_PRIORITY_5:
										item.Text = _profileData.VpnConfig[5].Priority.ToString();
										continue;
								}

								continue;
						}
					}
				}
				catch (Exception ex)
				{
					LogWriter.Write($"ConfigManager # Failed to extract tag from a control item. \n\n{ex}");
				}

				LogWriter.Write($"ConfigManager # Finished processing controls, moving on.");
				SetState(ConfigManagerState.CheckForConfigs);
			}

			private void ProcessCheckForConfigs()
			{
				List<KeyValuePair<int, string>> configs = new List<KeyValuePair<int, string>>
				{
					new KeyValuePair<int, string>(_profileData.VpnConfig[0].Priority, _profileData.VpnConfig[0].Path),
					new KeyValuePair<int, string>(_profileData.VpnConfig[1].Priority, _profileData.VpnConfig[1].Path),
					new KeyValuePair<int, string>(_profileData.VpnConfig[2].Priority, _profileData.VpnConfig[2].Path),
					new KeyValuePair<int, string>(_profileData.VpnConfig[3].Priority, _profileData.VpnConfig[3].Path),
					new KeyValuePair<int, string>(_profileData.VpnConfig[4].Priority, _profileData.VpnConfig[4].Path),
					new KeyValuePair<int, string>(_profileData.VpnConfig[5].Priority, _profileData.VpnConfig[5].Path),
				};

				_orderedConfigs = new List<KeyValuePair<int, string>>();

				foreach (KeyValuePair<int, string> kvpItem in configs.OrderBy(item => item.Key))
				{
					_orderedConfigs.Add(kvpItem);
				}

				bool foundConfig = false;
					
				foreach (var configItem in _orderedConfigs)
				{
					if (string.IsNullOrWhiteSpace(configItem.Value)) continue;

					foundConfig = true;
					_kvpBestConfig = configItem; // Going to need this later, no point in throwing it away.
					_profileData.ActiveConfig = configItem.Value;
					break;
				}

				if (!foundConfig)
				{
					LogWriter.Write($"ConfigManager # Warning! No valid configs found, moving to maintain to wait for configs.");
					SetState(ConfigManagerState.MaintainConfigs);
				}
				else
				{
					LogWriter.Write($"ConfigManager # At least 1 valid config found, moving to parse it ({_kvpBestConfig})");
					SetState(ConfigManagerState.ValidateConfigs);
				}				
			}

			private void ProcessValidateConfigs()
			{
				LogWriter.Write($"ConfigManager # Attempting to validate config files.");

				foreach (var kvpConfig in _orderedConfigs)
				{
					if (string.IsNullOrEmpty(kvpConfig.Value)) continue;

					var ovpnConfigRootPath = "";

					if (!Path.IsPathRooted(kvpConfig.Value))
					{
						ovpnConfigRootPath = Path.Combine(_profileData.ConfigPath, kvpConfig.Value);
					}

					var ovpnFileContent = File.ReadAllLines(ovpnConfigRootPath);

					try
					{
						// Management - Used to define the OpenVPN instance's specific host address and port
						var index = Array.FindIndex(ovpnFileContent, x => x.StartsWith("management"));
						if (index >= 0)
						{
							ovpnFileContent[index] = $"management {LOCAL_HOST} {LOCAL_PORT}";
						}
						else
						{
							var lastIdx = ovpnFileContent.Length - 1;
							var lastLine = ovpnFileContent[lastIdx];
							ovpnFileContent[lastIdx] = $"{lastLine}{Environment.NewLine}management {LOCAL_HOST} {LOCAL_PORT}";
						}

						// Auth-User-Pass - Used to define Username and Password in a handly file.
						index = Array.FindIndex(ovpnFileContent, x => x.StartsWith("auth-user-pass"));
						if (index >= 0)
						{
							if (_profileData.Username == null || _profileData.Password == null)
							{
								throw new ArgumentException("Username or password cannot be null");
							}

							// Create a credentials file as we're assuming there isn't one, if so it doesn't matter anyway.
							var passFileName = Path.Combine(Path.GetTempPath(), "ovpnpass.txt").Replace(@"\", @"\\");
							File.WriteAllLines(passFileName, new[] { _profileData.Username, _profileData.Password });
							ovpnFileContent[index] = $"auth-user-pass {passFileName}";
						}
						else
						{
							if (_profileData.Username != null || _profileData.Password != null)
							{
								throw new ArgumentException($"Username or password are provided but the *.ovpn file ({ovpnConfigRootPath}) doesn't have the line 'auth-user-pass'");
							}
						}
					}
					catch (ArgumentException argsException)
					{
						LogWriter.Write($"ConfigManager # Args exception occurred: \n{argsException}");
					}

					File.WriteAllLines(ovpnConfigRootPath, ovpnFileContent);
				}

				LogWriter.Write($"ConfigManager # Finished validating config files.");
				SetState(ConfigManagerState.MaintainConfigs);
			}

			private void ProcessMaintainConfigs()
			{
				UpdateConfigControls();
			}

			#endregion FSM Methods

			#region Control Event Handlers

			private void ConfigButtonClick(Button sender, EventArgs e)
			{
				if (sender == null) return;

				var dialog = new OpenFileDialog
				{
					CheckFileExists = true,
					Multiselect = false,
					InitialDirectory = _profileData.ConfigPath,
					Filter = "Open VPN Config Files (*.ovpn)|*.ovpn"
				};

				DialogResult result = dialog.ShowDialog();

				if (result != DialogResult.OK) return;
				var newPath = dialog.FileName;

				if (newPath.Length <= 0)
				{
					return;
				}

				int.TryParse(sender.Tag.ToString(), out var buttonTag);

				switch (buttonTag)
				{
					case CONTROL_ID_BROWSE_0:
						_profileData.VpnConfig[0].Path = dialog.SafeFileName;
						LogWriter.Write($"ConfigManager # Updating config file for slot " +
						            $"[{buttonTag}, { _profileData.VpnConfig[0].Path}].");
						break;

					case CONTROL_ID_BROWSE_1:
						_profileData.VpnConfig[1].Path = dialog.SafeFileName;
						LogWriter.Write($"ConfigManager # Updating config file for slot " +
						            $"[{buttonTag}, { _profileData.VpnConfig[1].Path}].");
						break;

					case CONTROL_ID_BROWSE_2:
						_profileData.VpnConfig[2].Path = dialog.SafeFileName;
						LogWriter.Write($"ConfigManager # Updating config file for slot " +
						            $"[{buttonTag}, { _profileData.VpnConfig[2].Path}].");
						break;

					case CONTROL_ID_BROWSE_3:
						_profileData.VpnConfig[3].Path = dialog.SafeFileName;
						LogWriter.Write($"ConfigManager # Updating config file for slot " +
						                $"[{buttonTag}, { _profileData.VpnConfig[3].Path}].");
						break;

					case CONTROL_ID_BROWSE_4:
						_profileData.VpnConfig[4].Path = dialog.SafeFileName;
						LogWriter.Write($"ConfigManager # Updating config file for slot " +
						                $"[{buttonTag}, { _profileData.VpnConfig[4].Path}].");
						break;

					case CONTROL_ID_BROWSE_5:
						_profileData.VpnConfig[5].Path = dialog.SafeFileName;
						LogWriter.Write($"ConfigManager # Updating config file for slot " +
						                $"[{buttonTag}, { _profileData.VpnConfig[5].Path}].");
						break;
				}

				_profileData.Export();
			}

			private void PriorityButtonClick(Button sender, MouseEventArgs e)
			{
				if (sender == null) return;

				int priority = 0;
				int.TryParse(sender.Tag.ToString(), out var buttonTag);

				switch (e.Button)
				{
					case MouseButtons.Left:

						switch (buttonTag)
						{
							case CONTROL_ID_PRIORITY_0:

								if (_profileData.VpnConfig[0].Priority < LOWEST_PRIORITY)
								{
									_profileData.VpnConfig[0].Priority++;
								}
								else
								{
									_profileData.VpnConfig[0].Priority = HIGHEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[0].Priority;

								LogWriter.Write($"ConfigManager # Increase config priority for slot " +
											$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_1:

								if (_profileData.VpnConfig[1].Priority < LOWEST_PRIORITY)
								{
									_profileData.VpnConfig[1].Priority++;
								}
								else
								{
									_profileData.VpnConfig[1].Priority = HIGHEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[1].Priority;

								LogWriter.Write($"ConfigManager # Increase config priority for slot " +
											$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_2:

								if (_profileData.VpnConfig[2].Priority < LOWEST_PRIORITY)
								{
									_profileData.VpnConfig[2].Priority++;
								}
								else
								{
									_profileData.VpnConfig[2].Priority = HIGHEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[2].Priority;

								LogWriter.Write($"ConfigManager # Increase config priority for slot " +
											$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_3:

								if (_profileData.VpnConfig[3].Priority < LOWEST_PRIORITY)
								{
									_profileData.VpnConfig[3].Priority++;
								}
								else
								{
									_profileData.VpnConfig[3].Priority = HIGHEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[3].Priority;

								LogWriter.Write($"ConfigManager # Increase config priority for slot " +
												$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_4:

								if (_profileData.VpnConfig[4].Priority < LOWEST_PRIORITY)
								{
									_profileData.VpnConfig[4].Priority++;
								}
								else
								{
									_profileData.VpnConfig[4].Priority = HIGHEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[4].Priority;

								LogWriter.Write($"ConfigManager # Increase config priority for slot " +
												$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_5:

								if (_profileData.VpnConfig[5].Priority < LOWEST_PRIORITY)
								{
									_profileData.VpnConfig[5].Priority++;
								}
								else
								{
									_profileData.VpnConfig[5].Priority = HIGHEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[5].Priority;

								LogWriter.Write($"ConfigManager # Increase config priority for slot " +
												$"[{buttonTag}, {priority}].");

								break;
						}

						break;
					
					case MouseButtons.Right:

						switch (buttonTag)
						{
							case CONTROL_ID_PRIORITY_0:

								if (_profileData.VpnConfig[0].Priority > HIGHEST_PRIORITY)
								{
									_profileData.VpnConfig[0].Priority--;
								}
								else
								{
									_profileData.VpnConfig[0].Priority = LOWEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[0].Priority;

								LogWriter.Write($"ConfigManager # Decrease config priority for slot " +
											$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_1:

								if (_profileData.VpnConfig[1].Priority > HIGHEST_PRIORITY)
								{
									_profileData.VpnConfig[1].Priority--;
								}
								else
								{
									_profileData.VpnConfig[1].Priority = LOWEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[1].Priority;

								LogWriter.Write($"ConfigManager # Decrease config priority for slot " +
											$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_2:

								if (_profileData.VpnConfig[2].Priority > HIGHEST_PRIORITY)
								{
									_profileData.VpnConfig[2].Priority--;
								}
								else
								{
									_profileData.VpnConfig[2].Priority = LOWEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[2].Priority;

								LogWriter.Write($"ConfigManager # Decrease config priority for slot " +
											$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_3:

								if (_profileData.VpnConfig[3].Priority > HIGHEST_PRIORITY)
								{
									_profileData.VpnConfig[3].Priority--;
								}
								else
								{
									_profileData.VpnConfig[3].Priority = LOWEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[3].Priority;

								LogWriter.Write($"ConfigManager # Decrease config priority for slot " +
												$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_4:

								if (_profileData.VpnConfig[4].Priority > HIGHEST_PRIORITY)
								{
									_profileData.VpnConfig[4].Priority--;
								}
								else
								{
									_profileData.VpnConfig[4].Priority = LOWEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[4].Priority;

								LogWriter.Write($"ConfigManager # Decrease config priority for slot " +
												$"[{buttonTag}, {priority}].");

								break;

							case CONTROL_ID_PRIORITY_5:

								if (_profileData.VpnConfig[5].Priority > HIGHEST_PRIORITY)
								{
									_profileData.VpnConfig[5].Priority--;
								}
								else
								{
									_profileData.VpnConfig[5].Priority = LOWEST_PRIORITY;
								}

								priority = _profileData.VpnConfig[5].Priority;

								LogWriter.Write($"ConfigManager # Decrease config priority for slot " +
												$"[{buttonTag}, {priority}].");

								break;
						}

						break;
				}

				sender.Text = priority.ToString();

				_profileData.Export();
			}

			#endregion Control Event Handlers
		}
	}
}
