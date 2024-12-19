using System.ComponentModel;
using System.Runtime.CompilerServices;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using dichternebel.YaSB.StreamerBot;
using dichternebel.YaSB.MacroDeckPlug;
using System.Text.Json;

namespace dichternebel.YaSB
{
    public class Model : INotifyPropertyChanged
    {
        public WebSocketClient WebSocketClient { get; private set; }

        private bool _isConnectedToStreamerBot;
        public bool IsConnectedToStreamerBot
        {
            get => _isConnectedToStreamerBot;
            set
            {
                if (_isConnectedToStreamerBot == value) return;
                _isConnectedToStreamerBot = value;
                if (value)
                {
                    Button.BackgroundImage = Properties.Resources.streamerbot_logo_white_checked;
                    if (!IsConfigured)
                    {
                        SetDefaultEvents();
                    }
                }
                else
                {
                    Button.BackgroundImage = Properties.Resources.streamerbot_logo_white_error;
                }
                OnPropertyChanged();
            }
        }

        private ResponseMessage _streamerBotMessage;
        public ResponseMessage StreamerBotMessage
        {
            get => _streamerBotMessage;
            set
            {
                if (_streamerBotMessage == value) return;
                _streamerBotMessage = value;
                HandleStreamerBotMessage(_streamerBotMessage);
                OnPropertyChanged();
            }
        }

        private Info _streamerBotInfo;
        public Info StreamerBotInfo
        {
            get => _streamerBotInfo;
            set
            {
                if (value == _streamerBotInfo) return;
                _streamerBotInfo = value;
                OnPropertyChanged();
            }
        }

        private Authentication _streamerAuthentication;
        public Authentication StreamerAuthentication
        {
            get => _streamerAuthentication;
            set
            {
                if (value == _streamerAuthentication) return;
                _streamerAuthentication = value;
                OnPropertyChanged();
            }
        }

        private List<StreamerBot.Action> _streamerBotActions;
        public List<StreamerBot.Action> StreamerBotActions
        {
            get => _streamerBotActions;
            set
            {
                if (value == _streamerBotActions) return;
                _streamerBotActions = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, string[]> _streamerBotEvents;
        public Dictionary<string, string[]> StreamerBotEvents
        {
            get => _streamerBotEvents;
            set
            {
                if (value == _streamerBotEvents) return;
                _streamerBotEvents = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, string[]> ConfiguredEvents {
            get
            {
                var configuredEvents = new Dictionary<string, string[]>();

                if (StreamerBotEvents != null)
                {
                    foreach (var key in StreamerBotEvents.Keys)
                    {
                        var valueArray = StreamerBotEvents[key];
                        var checkedValues = new List<string>();
                        foreach (var value in valueArray)
                        {
                            var isChecked = IsEventChecked(Helper.CreateEventKey(key, value));
                            if (isChecked)
                            {
                                checkedValues.Add(value);
                            }
                        }

                        if (checkedValues.Any())
                        {
                            configuredEvents.Add(key, checkedValues.ToArray());
                        }
                    }
                }

                return configuredEvents;
            }
        } 

        public ContentSelectorButton Button { get; set; }

        public bool IsConfigured
        {
            get
            {
                return bool.TryParse(GetPluginConfiguration(), out bool result) ? result : false;
            }
            set
            {
                if (value.ToString() == GetPluginConfiguration()) return;
                SetPluginConfiguration(value.ToString());
                OnPropertyChanged();
            }
        }

        public bool IsDeleteVariablesOnExit
        {
            get
            {
                return bool.TryParse(GetPluginConfiguration(), out bool result) ? result : false;
            }
            set
            {
                if (value.ToString() == GetPluginConfiguration()) return;
                SetPluginConfiguration(value.ToString());
                OnPropertyChanged();
            }
        }

        public string WebSocketHost
        {
            get
            {
                return GetPluginConfiguration() ?? "127.0.0.1";
            }
            set
            {
                if (value == GetPluginConfiguration()) return;
                SetPluginConfiguration(value);
                OnPropertyChanged();
                OnPropertyChanged("WebSocketUri");
            }
        }

        public string WebSocketEndpoint
        {
            get
            {
                return GetPluginConfiguration() ?? "/";
            }
            set
            {
                if (value == GetPluginConfiguration()) return;
                SetPluginConfiguration(value);
                OnPropertyChanged();
                OnPropertyChanged("WebSocketUri");
            }
        }

        public int WebSocketPort
        {
            get
            {
                return int.TryParse(GetPluginConfiguration(), out int value) ? value : 8080;
            }
            set
            {
                if (value.ToString() == GetPluginConfiguration()) return;
                SetPluginConfiguration(value.ToString());
                OnPropertyChanged();
                OnPropertyChanged("WebSocketUri");
            }
        }

        public bool WebSocketAuthenticationEnabled
        {
            get
            {
                return bool.TryParse(GetPluginConfiguration(), out bool value)? value : false;
            }
            set
            {
                if (value.ToString() == GetPluginConfiguration()) return;
                SetPluginConfiguration(value.ToString());
                OnPropertyChanged();
                OnPropertyChanged("WebSocketUri");
            }
        }

        public string WebSocketPassword
        {
            get
            {
                return GetPluginCredential() ?? "";
            }
            set
            {
                if (value == GetPluginCredential()) return;
                SetPluginCredential(value);
                OnPropertyChanged();
                OnPropertyChanged("WebSocketUri");
            }
        }

        public string WebSocketUri
        {
            get
            {
                return new UriBuilder
                {
                    Scheme = "ws",
                    Host = WebSocketHost,
                    Path = WebSocketEndpoint,
                    Port = WebSocketPort
                }.Uri.ToString();
            }
        }


        public Model()
        {
            _isConnectedToStreamerBot = false;

            Button = new ContentSelectorButton
            {
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = Properties.Resources.streamerbot_logo_white
            };

            PropertyChanged += HandleOnPropertyChanged;
        }

        private void HandleOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WebSocketUri")
            {
                WebSocketClient.SwitchHostAsync(WebSocketUri);
            }
        }

        private void HandleStreamerBotMessage(ResponseMessage message)
        {
            if (message == null) return;

            switch (message.ResponseType)
            {
                case ResponseType.Hello:
                    this.StreamerBotInfo = message.Info;
                    this.StreamerAuthentication = message.Authentication;
                    if (WebSocketAuthenticationEnabled)
                    {
                        WebSocketClient.AuthenticateAsync(WebSocketPassword, StreamerAuthentication.Salt, StreamerAuthentication.Challenge).Wait();
                    }
                    break;
                case ResponseType.Actions:
                    this.StreamerBotActions = message.Actions;
                    break;
                case ResponseType.Events:
                    this.StreamerBotEvents = message.Events;
                    // Now that we have all available events subsribe to the selected ones
                    WebSocketClient.SubscribeToServerAsync().Wait();
                    break;
                case ResponseType.Event:
                    var botEvent = message.Event;
                    SetVariable(botEvent);
                    break;
                default:
                    break;
            }
        }

        private void SetVariable(BotEvent currentEvent)
        {
            // I guess this is how the Steamer.Bot Plugin would make it so I keep it here for compatability
            if (currentEvent.EventInfo.Source == "General" && currentEvent.EventInfo.Type == "Custom")
            {
                var currentType = currentEvent.Data.KeyValuePairs.First().Value.GetType();
                VariableType variableType = TypeMapping.TryGetValue(currentType, out var type) ? type : VariableType.String;
                VariableManager.SetValue($"{currentEvent.EventInfo.Type}_{currentEvent.Data.KeyValuePairs.First().Key}", currentEvent.Data.KeyValuePairs.First().Value, variableType, Main.Instance, new string[] { currentEvent.EventInfo.Type });
                return;
            }
            // Doing the same with global variables to make switch easier
            if (currentEvent.EventInfo.Source == "Misc" && currentEvent.EventInfo.Type == "GlobalVariableUpdated")
            {
                var currentType = currentEvent.Data.KeyValuePairs["newValue"].GetType();
                VariableType variableType = TypeMapping.TryGetValue(currentType, out var type) ? type : VariableType.String;
                VariableManager.SetValue($"{currentEvent.EventInfo.Type}_{currentEvent.Data.KeyValuePairs["name"]}", currentEvent.Data.KeyValuePairs["newValue"], variableType, Main.Instance, new string[] { currentEvent.EventInfo.Type });
                return;
            }
            
            // I expect that data is something, so I will store a string for now to keep it simple
            foreach(var kvp in currentEvent.Data.KeyValuePairs)
            {
                VariableManager.SetValue($"{currentEvent.EventInfo.Source}_{currentEvent.EventInfo.Type}", kvp.Value, VariableType.String, Main.Instance, new string[] { kvp.Key });
            }
        }

        public void DeleteVariables()
        {
            var pluginVariables = VariableManager.GetVariables(Main.Instance);
            foreach (var pluginVariable in pluginVariables)
            {
                VariableManager.DeleteVariable(pluginVariable.Name);
            }
        }

        private static readonly Dictionary<Type, VariableType> TypeMapping = new()
            {
                { typeof(int), VariableType.Integer },
                { typeof(double), VariableType.Float },
                { typeof(float), VariableType.Float },
                { typeof(decimal),VariableType.Float },
                { typeof(bool), VariableType.Bool },
                { typeof(string),VariableType.String }
            };

        private string? GetPluginConfiguration([CallerMemberName] string propertyName = null)
        {
            var value = PluginConfiguration.GetValue(Main.Instance, propertyName);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private void SetPluginConfiguration(string value, [CallerMemberName] string propertyName = null)
        {
            PluginConfiguration.SetValue(Main.Instance, propertyName, value);
        }

        private string? GetPluginCredential([CallerMemberName] string propertyName = null)
        {
            var credentialsList = PluginCredentials.GetPluginCredentials(Main.Instance);
            var targetDict = credentialsList.FirstOrDefault(dict => dict.ContainsKey(propertyName));
            var secret = targetDict?[propertyName];
            return string.IsNullOrEmpty(secret) ? null : secret;
        }

        private void SetPluginCredential(string secret, [CallerMemberName] string propertyName = null)
        {
            var keyValuePairs = new Dictionary<string, string>();
            keyValuePairs[propertyName] = secret;
            PluginCredentials.SetCredentials(Main.Instance, keyValuePairs);
        }

        public bool IsEventChecked(string eventIdentifier)
        {
            var value = PluginConfiguration.GetValue(Main.Instance, eventIdentifier);
            return bool.TryParse(value, out bool result) ? result : false;
        }

        public void SaveEvent(string eventIdentifier, bool value)
        {
            PluginConfiguration.SetValue(Main.Instance, eventIdentifier, value.ToString());
            
            // Intentionally not awaited!
            if (value) WebSocketClient.SubscribeToServerAsync();
            else WebSocketClient.UnSubscribeFromServerAsync(eventIdentifier);
        }

        public void ResetConfiguration()
        {
            PluginConfiguration.DeletePluginConfig(Main.Instance);
            PluginCredentials.DeletePluginCredentials(Main.Instance);

            SetDefaultEvents();

            OnPropertyChanged("WebSocketHost");
            OnPropertyChanged("WebSocketPort");
            OnPropertyChanged("WebSocketEndpoint");
            OnPropertyChanged("WebSocketPassword");
            OnPropertyChanged("WebSocketUri");
        }

        private void SetDefaultEvents()
        {
            SaveEvent("General_Custom", true);
            SaveEvent("Misc_GlobalVariableUpdated", true);
            SaveEvent("Misc_UserGlobalVariableUpdated", true);
            IsConfigured = true;
        }

        public void InstantiateWebSocketClient()
        {
            MacroDeckLogger.Info(Main.Instance, $"Instantiating WebSocketClient to: {WebSocketUri}");
            WebSocketClient = new WebSocketClient(WebSocketUri);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
