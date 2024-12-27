using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
                    if (!IsConfigured)
                    {
                        SetDefaultEvents();
                    }
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

        public List<string> YaSBEventSubscriptions
        {
            get
            {
                var currentEventSubscriptions = GetPluginConfiguration();
                if (string.IsNullOrEmpty(currentEventSubscriptions)) return new List<string>();
                return JsonSerializer.Deserialize<List<string>>(currentEventSubscriptions) ?? [];
            }
            set
            {
                SetPluginConfiguration(JsonSerializer.Serialize(value));
            }
        }

        public Dictionary<string, string[]> EventSubscriptionDictionary {
            get
            {
                var eventSubscriptionDictionary = new Dictionary<string, string[]>();

                if (StreamerBotEvents != null)
                {
                    foreach (var key in StreamerBotEvents.Keys)
                    {
                        var valueArray = StreamerBotEvents[key];
                        var checkedEvents = new List<string>();
                        foreach (var value in valueArray)
                        {
                            var isChecked = IsEventSubscribed(Helper.CreateEventKey(key, value));
                            if (isChecked)
                            {
                                checkedEvents.Add(value);
                            }
                        }

                        if (checkedEvents.Any())
                        {
                            eventSubscriptionDictionary.Add(key, checkedEvents.ToArray());
                        }
                    }
                }

                return eventSubscriptionDictionary;
            }
        }

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
                    SetAndTransformVariable(botEvent);
                    break;
                default:
                    //MacroDeckLogger.Trace(Main.Instance,$"Raw message:\n{message.RawMessage}");
                    break;
            }
        }

        private void SetAndTransformVariable(BotEvent currentEvent)
        {
            if (currentEvent == null) return;
            if (string.IsNullOrEmpty(currentEvent.Data.KeyValuePairs.First().Key)) return;

            // Macro Deck is "sanitizing" the variable name so we have to respect that
            var identifier = Helper.CreateEventKey(currentEvent.EventInfo.Source, currentEvent.EventInfo.Type).ToLower().Replace("-", "_");

            // We set the naming especially for Misc/GlobalVariableUpdated and General/Custom different here to get multiple variables in Macro Deck
            if (currentEvent.EventInfo.Source == "Misc" && currentEvent.EventInfo.Type == "GlobalVariableUpdated"
                || currentEvent.EventInfo.Source == "General" && currentEvent.EventInfo.Type == "Custom")
            {
                identifier = Helper.CreateEventKey(currentEvent.EventInfo.Type, currentEvent.Data.KeyValuePairs.First().Key).ToLower();
                if (currentEvent.Data.KeyValuePairs.Count > 1)
                {
                    // This assumes that the first KeyValuePair entry after 'id' is the key of the event... Danger, Will Robinson!
                    var identifierKeyValuePair = currentEvent.Data.KeyValuePairs.FirstOrDefault(x => !x.Key.Equals("id", StringComparison.CurrentCultureIgnoreCase));

                    if (identifierKeyValuePair.Value != null)
                    {
                        identifier = Helper.CreateEventKey(currentEvent.EventInfo.Type, identifierKeyValuePair.Value.ToString()).ToLower().Replace("-", "_");
                    }
                }
            }

            var eventKeyValuePair = new KeyValuePair<string, object>(identifier, JsonSerializer.Serialize(currentEvent.Data.KeyValuePairs));
            var transformedeventKeyValuePair = TransformVariable(eventKeyValuePair);
            var transformedVariableType = GetValueType(transformedeventKeyValuePair.Value);
            VariableManager.SetValue(identifier, transformedeventKeyValuePair.Value, transformedVariableType, Main.Instance, []);
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
                SetAndTransformVariable(currentBotEvent);
            }
        }

        private KeyValuePair<string,object> TransformVariable(KeyValuePair<string, object> variable)
        {
            var result = new KeyValuePair<string, object> ( variable.Key, variable.Value );
            var currentTransformations = GetTransformations().ToList();

            var transformation = currentTransformations.FirstOrDefault(x => x.Variable == variable.Key);
            if (transformation != null )
            {
                transformation.Value = variable.Value.ToString();
                result = new KeyValuePair<string, object>(variable.Key, transformation.TransformationValue); 
            }

            return result;
        }

        public void DeleteVariables()
        {
            var pluginVariables = VariableManager.GetVariables(Main.Instance);
            foreach (var pluginVariable in pluginVariables)
            {
                VariableManager.DeleteVariable(pluginVariable.Name);
            }
        }

        private VariableType GetValueType(object value)
        {
            var currentType = typeof(string);
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out _)) currentType = typeof(int);
                else if (decimal.TryParse(value.ToString(), out _)) currentType = typeof(decimal);
                else if (bool.TryParse(value.ToString(), out _)) currentType = typeof(bool);
            }

            var currentVariableType = VariableType.String;
            if (currentType != typeof(string))
            {
                currentVariableType = TypeMapping.TryGetValue(currentType, out var type) ? type : VariableType.String;
            }

            return currentVariableType;
        }

        private static readonly Dictionary<Type, VariableType> TypeMapping = new()
            {
                { typeof(int), VariableType.Integer },
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

        public bool IsEventSubscribed(string eventIdentifier)
        {
            var currentEventSubscriptions = YaSBEventSubscriptions;
            return currentEventSubscriptions.Contains(eventIdentifier);
        }

        public void SetEventSubscription(string eventIdentifier, bool value)
        {
            var currentEventSubscriptions = YaSBEventSubscriptions.ToList();

            if (!currentEventSubscriptions.Contains(eventIdentifier) && value)
            {
                currentEventSubscriptions.Add(eventIdentifier);
            }
            else if (currentEventSubscriptions.Contains(eventIdentifier) && !value)
            {
                currentEventSubscriptions.Remove(eventIdentifier);
            }

            YaSBEventSubscriptions = currentEventSubscriptions;

            if (value) WebSocketClient.SubscribeToServerAsync().Wait();
            else WebSocketClient.UnSubscribeFromServerAsync(eventIdentifier).Wait();
        }

        public void SetEventSubscriptions(List<string> eventList, bool value)
        {
            var currentEventSubscriptions = YaSBEventSubscriptions.ToList();

            foreach (var eventIdentifier in eventList)
            {
                if (!currentEventSubscriptions.Contains(eventIdentifier) && value)
                {
                    currentEventSubscriptions.Add(eventIdentifier);
                }
                else if (currentEventSubscriptions.Contains(eventIdentifier) && !value)
                {
                    currentEventSubscriptions.Remove(eventIdentifier);
                }
            }

            YaSBEventSubscriptions = currentEventSubscriptions;

            if (!value)
            foreach (var eventIdentifier in eventList)
            {
                WebSocketClient.UnSubscribeFromServerAsync(eventIdentifier).Wait();
            }
            else WebSocketClient.SubscribeToServerAsync().Wait();
        }

        public List<YaSBTransformation> GetTransformations()
        {
            // Instantiate
            var transformations = new List<YaSBTransformation>();

            // Add existing transformations to the list
            var existingTransformationsJson = PluginConfiguration.GetValue(Main.Instance, "YaSBTransformationList");

            if (!string.IsNullOrEmpty(existingTransformationsJson))
            {
                var existingTransformationsList = JsonSerializer.Deserialize<List<YaSBTransformation>>(existingTransformationsJson);

                foreach (var item in existingTransformationsList)
                {
                   transformations.Add(item);
                }
            }

            // Fill list with current existing variables
            var existingVariables = VariableManager.GetVariables(Main.Instance);

            foreach (var variable in existingVariables)
            {
                var transformation = new YaSBTransformation
                {
                    Variable = variable.Name,
                    Value = variable.Value
                };

                if (transformations.FirstOrDefault(x => x.Variable == variable.Name) == null)
                {
                    // Add variable to top
                    transformations.Insert(0, transformation);
                }
            }

            return transformations;
        }

        public void SaveTransformations(List<YaSBTransformation> transientTransformations)
        {
            List<YaSBTransformation> transformationList = [];

            foreach (var item in transientTransformations)
            {
                if (!string.IsNullOrEmpty(item.JsonKey)) transformationList.Add(item);
            }

            PluginConfiguration.SetValue(Main.Instance, "YaSBTransformationList", JsonSerializer.Serialize(transformationList));
        }

        public void ResetConfiguration()
        {
            PluginConfiguration.DeletePluginConfig(Main.Instance);
            PluginCredentials.DeletePluginCredentials(Main.Instance);

            DeleteVariables();
            SetDefaultEvents();

            OnPropertyChanged("WebSocketHost");
            OnPropertyChanged("WebSocketPort");
            OnPropertyChanged("WebSocketEndpoint");
            OnPropertyChanged("WebSocketPassword");
            OnPropertyChanged("WebSocketUri");
        }

        private void SetDefaultEvents()
        {
            SetEventSubscription("General_Custom", true);
            SetEventSubscription("Misc_GlobalVariableUpdated", true);
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
