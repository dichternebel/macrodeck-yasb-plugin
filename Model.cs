using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using dichternebel.YaSB.StreamerBot;
using dichternebel.YaSB.MacroDeckPlug;

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

        private Authentication _streamerBotAuthentication;
        public Authentication StreamerBotAuthentication
        {
            get => _streamerBotAuthentication;
            set
            {
                if (value == _streamerBotAuthentication) return;
                _streamerBotAuthentication = value;
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

        private List<YaSBTransformation> _transformations;
        public BindingList<YaSBTransformation> Transformations
        {
            get
            {
                // Instantiate and fill list with current existing variables
                _transformations = new List<YaSBTransformation>();
                var existingVariables = VariableManager.GetVariables(Main.Instance);

                foreach (var variable in existingVariables)
                {
                    var transformation = new YaSBTransformation
                    {
                        Variable = variable.Name,
                        Value = variable.Value
                    };
                    _transformations.Add(transformation);
                }

                // Add existing transformations to the list
                var existingTransformationsJson = PluginConfiguration.GetValue(Main.Instance, "YaSBTransformationList");

                if (!string.IsNullOrEmpty(existingTransformationsJson))
                {
                    var existingTransformationsList = JsonSerializer.Deserialize<List<YaSBTransformation>>(existingTransformationsJson);

                    foreach (var item in existingTransformationsList)
                    {
                        var existingVariable = _transformations.FirstOrDefault(x => x.Variable == item.Variable);
                        if (existingVariable != null)
                        {
                            existingVariable.JsonKey = item.JsonKey;
                        }
                        else
                        {
                            _transformations.Add(item);
                        }
                    }
                }

                return new BindingList<YaSBTransformation>(_transformations);
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
            Transformations.ListChanged += Transformations_ListChanged;
        }

        private void Transformations_ListChanged(object? sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                MacroDeckLogger.Trace(Main.Instance, "TransformationList changed!");
                SaveTransformations();
            }
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
                    StreamerBotInfo = message.Info;
                    StreamerBotAuthentication = message.Authentication;
                    if (WebSocketAuthenticationEnabled && StreamerBotAuthentication != null)
                    {
                        WebSocketClient.AuthenticateAsync(WebSocketPassword, StreamerBotAuthentication.Salt, StreamerBotAuthentication.Challenge).Wait();
                    }
                    else
                    {
                        IsConnectedToStreamerBot = true;
                        WebSocketClient.GetEventsAsync().Wait();
                        WebSocketClient.GetActionsAsync().Wait();
                    }
                    break;
                case ResponseType.Authentication:
                    IsConnectedToStreamerBot = true;
                    WebSocketClient.GetEventsAsync().Wait();
                    WebSocketClient.GetActionsAsync().Wait();
                    break;
                case ResponseType.Actions:
                    StreamerBotActions = message.Actions;
                    break;
                case ResponseType.Events:
                    StreamerBotEvents = message.Events;
                    // Now that we have all available events subsribe to the selected ones
                    WebSocketClient.SubscribeToServerAsync().Wait();
                    // And get the Streamer.Bot global variables
                    WebSocketClient.GetGlobalsAsync().Wait();
                    break;
                case ResponseType.Globals:
                    var globalVars = message.Event;
                    SetVariables(globalVars);
                    break;
                case ResponseType.Event:
                    var botEvent = message.Event;
                    SetVariable(botEvent);
                    break;
                default:
                    //MacroDeckLogger.Trace(Main.Instance,$"Raw message:\n{message.RawMessage}");
                    break;
            }
        }

        private void SetVariable(BotEvent currentEvent)
        {
            // I guess this is how the Steamer.Bot Plugin would make it so I keep it here for compatability
            if (currentEvent.EventInfo.Source == "General" && currentEvent.EventInfo.Type == "Custom")
            {
                var value = currentEvent.Data.KeyValuePairs.First().Value;
                var currentType = GetValueType(value);
                VariableType variableType = TypeMapping.TryGetValue(currentType, out var type) ? type : VariableType.String;
                VariableManager.SetValue($"{currentEvent.EventInfo.Type}_{currentEvent.Data.KeyValuePairs.First().Key}", currentEvent.Data.KeyValuePairs.First().Value, variableType, Main.Instance, []);
                return;
            }
            // Doing the same with global variables to make switch easier
            if (currentEvent.EventInfo.Source == "Misc" && currentEvent.EventInfo.Type == "GlobalVariableUpdated")
            {
                var value = currentEvent.Data.KeyValuePairs["newValue"];
                var currentType = GetValueType(value);

                VariableType variableType = TypeMapping.TryGetValue(currentType, out var type) ? type : VariableType.String;
                VariableManager.SetValue($"{currentEvent.EventInfo.Type}_{currentEvent.Data.KeyValuePairs["name"]}", currentEvent.Data.KeyValuePairs["newValue"], variableType, Main.Instance, []);
                return;
            }
            
            // I expect that data is something, so I will store a string for now to keep it simple
            foreach(var keyValuePair in currentEvent.Data.KeyValuePairs)
            {
                VariableManager.SetValue($"{currentEvent.EventInfo.Source}_{currentEvent.EventInfo.Type}", keyValuePair.Value, VariableType.String, Main.Instance, []);
            }
        }

        private void SetVariables(BotEvent globalsEvent)
        {
            foreach (var item in globalsEvent.Data.KeyValuePairs)
            {
                var currentBotEvent = JsonSerializer.Deserialize<BotEvent>(JsonSerializer.Serialize(globalsEvent));
                if (currentBotEvent == null) continue;

                var dynamicObject = JsonSerializer.Deserialize<dynamic>(item.Value.ToString());
                currentBotEvent.Data.KeyValuePairs = new Dictionary<string, object>
                {
                    { "name", item.Key },
                    { "newValue", dynamicObject.GetProperty("value") }
                };
                SetVariable(currentBotEvent);
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

        private Type GetValueType(object value)
        {
            if (value == null) return typeof(string);

            if (int.TryParse(value.ToString(), out _)) return typeof(int);
            if (decimal.TryParse(value.ToString(), out _)) return typeof(decimal);
            if (float.TryParse(value.ToString(), out _)) return typeof(float);
            if (double.TryParse(value.ToString(), out _)) return typeof(double);
            if (bool.TryParse(value.ToString(), out _)) return typeof(bool);

            return typeof(string);
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
           
            if (value) WebSocketClient.SubscribeToServerAsync().Wait();
            else WebSocketClient.UnSubscribeFromServerAsync(eventIdentifier).Wait();
        }

        public void SaveEvents(List<string> eventList, bool value)
        {
            foreach (var eventIdentifier in eventList)
            {
                PluginConfiguration.SetValue(Main.Instance, eventIdentifier, value.ToString());
                if (!value) WebSocketClient.UnSubscribeFromServerAsync(eventIdentifier).Wait();
            }

            if (value) WebSocketClient.SubscribeToServerAsync().Wait();
        }

        public void SaveTransformations()
        {
            List<YaSBTransformation> transformationList = new List<YaSBTransformation>();

            foreach (var item in this._transformations)
            {
                if (!string.IsNullOrEmpty(item.JsonKey)) transformationList.Add(item);
            }

            PluginConfiguration.SetValue(Main.Instance, "YaSBTransformationList", JsonSerializer.Serialize(transformationList));

            // ToDo: Update variables to use the transformation value
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
