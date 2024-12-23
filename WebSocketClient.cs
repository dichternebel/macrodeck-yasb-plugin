using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SuchByte.MacroDeck.Logging;
using dichternebel.YaSB.StreamerBot;

namespace dichternebel.YaSB
{
    public class WebSocketClient
    {
        private ClientWebSocket _webSocket;
        private string _serverUrl;
        private readonly CancellationTokenSource _cts;

        private string AuthenticationMessage { get; set; }

        public bool IsAuthenticated { get; private set; }

        public static WebSocketClient Instance { get; private set; }

        public WebSocketClient(string serverUrl)
        {
            Instance ??= this;
            _serverUrl = serverUrl;
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();
        }

        public async Task StartConnectionAsync()
        {
            await Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        await ReceiveMessageAsync();

                        if (_webSocket.State != WebSocketState.Open)
                        {
                            IsAuthenticated = false;
                            Main.Model.IsConnectedToStreamerBot = false;
                            await ConnectToServerAsync();
                       }
                    }
                    catch (Exception ex)
                    {
                        // Macro Deck is terminating the plugin when an error is logged so we are writing warnings for connection losses
                        if (ex.Source == "System.Text.Json" || ex.Source == "System.Net.WebSockets.Client")
                        {
                            MacroDeckLogger.Warning(Main.Instance, ex.Message);
                        }
                        else
                        {
                            MacroDeckLogger.Error(Main.Instance, ex.Message);
                        }
                        Main.Model.IsConnectedToStreamerBot = false;
                        await Task.Delay(5000); // Wait 5 seconds before trying to reconnect
                    }
                }
            });
        }

        private async Task ConnectToServerAsync()
        {
            if (_webSocket.State != WebSocketState.None)
            {
                _webSocket.Dispose();
                _webSocket = new ClientWebSocket();
            }

            MacroDeckLogger.Info(Main.Instance, "Connecting to WebSocket server...");
            await _webSocket.ConnectAsync(new Uri(_serverUrl), _cts.Token);
        }

        public async Task GetEventsAsync()
        {
            try
            {
                var request = new
                {
                    request = RequestType.GetEvents.ToString(),
                    id = "dichternebel-yasb-get-events",
                    authentication = IsAuthenticated ? AuthenticationMessage : null
                };

                string payload = JsonSerializer.Serialize(request);

                var bytes = Encoding.UTF8.GetBytes(payload);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    _cts.Token);

                MacroDeckLogger.Info(Main.Instance, "GetEvents request sent successfully.");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to get events from WebSocket server: {ex.Message}");
            }
        }

        public async Task GetActionsAsync()
        {
            try
            {
                var request = new
                {
                    request = RequestType.GetActions.ToString(),
                    id = "dichternebel-yasb-get-actions",
                    authentication = IsAuthenticated ? AuthenticationMessage : null
                };

                string payload = JsonSerializer.Serialize(request);

                var bytes = Encoding.UTF8.GetBytes(payload);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    _cts.Token);

                MacroDeckLogger.Info(Main.Instance, "GetActions request sent successfully.");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to get actions from WebSocket server: {ex.Message}");
            }
        }

        public async Task UnSubscribeFromServerAsync(string eventIdentifier)
        {
            try
            {
                var eventsDictionary = Helper.ExplodeEventKey(eventIdentifier);
                if (eventsDictionary == null || !eventsDictionary.Any()) return;

                var request = new
                {
                    request = RequestType.UnSubscribe.ToString(),
                    id = "dichternebel-yasb-unsubscribe",
                    events = eventsDictionary,
                    authentication = IsAuthenticated ? AuthenticationMessage : null
                };

                string payload = JsonSerializer.Serialize(request);

                var bytes = Encoding.UTF8.GetBytes(payload);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    _cts.Token);
                MacroDeckLogger.Info(Main.Instance, $"Unsubscription request sent successfully:\n{payload}");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to unsubscribe from WebSocket server: {ex.Message}");
            }
        }

        public async Task SubscribeToServerAsync()
        {
            try
            {
                var eventsDictionary = Main.Model?.EventSubscriptionDictionary;
                if (eventsDictionary == null || !eventsDictionary.Any()) return;

                var request = new
                {
                    request = RequestType.Subscribe.ToString(),
                    id = "dichternebel-yasb-subscribe",
                    events = eventsDictionary,
                    authentication = IsAuthenticated ? AuthenticationMessage : null
                };

                string payload = JsonSerializer.Serialize(request);

                var bytes = Encoding.UTF8.GetBytes(payload);
                await _webSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    _cts.Token);
                MacroDeckLogger.Info(Main.Instance, $"Subscription request sent successfully:\n{payload}");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to subscribe to WebSocket server: {ex.Message}");
            }
        }

        private async Task GetGlobalsAsync(bool persisted)
        {
            var request = new
            {
                request = RequestType.GetGlobals.ToString(),
                id = "dichternebel-yasb-get-globals",
                persisted,
                authentication = IsAuthenticated ? AuthenticationMessage : null
            };

            string payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            var bytes = Encoding.UTF8.GetBytes(payload);
            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cts.Token);

            MacroDeckLogger.Info(Main.Instance, $"GetGlobals request sent successfully:\n{payload}");
        }

        public async Task GetGlobalsAsync()
        {
            try
            {
                var eventsDictionary = Main.Model?.EventSubscriptionDictionary;
                if (eventsDictionary == null || !eventsDictionary.Any()) return;

                if (eventsDictionary.TryGetValue("Misc", out string[] miscEvents))
                {
                    if (!miscEvents.Contains("GlobalVariableUpdated")) return;
                }
                else
                {
                    return;
                }

                await GetGlobalsAsync(true);
                await GetGlobalsAsync(false);
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to GetGlobals from WebSocket server: {ex.Message}");
            }
        }

        private async Task ReceiveMessageAsync()
        {
            MacroDeckLogger.Info(Main.Instance, "Listening...");
            var buffer = new byte[1024 * 4];
            var messageBuilder = new StringBuilder();

            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    var messageChunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    messageBuilder.Append(messageChunk);
                }
                while (!result.EndOfMessage);

                string completeMessage = messageBuilder.ToString();

                // Parse JSON
                var jsonDocument = JsonDocument.Parse(completeMessage);
                var responseMessage = ParseJsonToMessage(jsonDocument);
                Main.Model.StreamerBotMessage = responseMessage;

                // Clear the StringBuilder for the next message
                messageBuilder.Clear();
            }
        }

        private ResponseMessage ParseJsonToMessage(JsonDocument jsonDocument)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var prettyJson = JsonSerializer.Serialize(
                jsonDocument,
                new JsonSerializerOptions { WriteIndented = true }
            );
            //MacroDeckLogger.Trace(Main.Instance, $"Received message:\n{prettyJson}");

            var root = jsonDocument.RootElement;

            // Check if it's a "hello" response
            if (root.TryGetProperty("info", out var infoElement))
            {
                var info = JsonSerializer.Deserialize<Info>(infoElement.GetRawText(), options);

                if (root.TryGetProperty("authentication", out var authElement))
                {
                    var auth = JsonSerializer.Deserialize<Authentication>(authElement.GetRawText(), options);
                    var salt = auth.Salt;
                    var challenge = auth.Challenge;
                    // Ensure proper decoding of Unicode escape sequences
                    auth.Salt = System.Net.WebUtility.HtmlDecode(salt);
                    auth.Challenge = System.Net.WebUtility.HtmlDecode(challenge);

                    return new ResponseMessage { ResponseType = ResponseType.Hello, Info = info, Authentication = auth };
                }

                return new ResponseMessage { ResponseType = ResponseType.Hello, Info = info };
            }

            // Check if it's an authentication response
            if (root.TryGetProperty("id", out var authIdentifierElement))
            {
                var identifier = JsonSerializer.Deserialize<string>(authIdentifierElement.GetRawText());
                if (identifier == "dichternebel-yasb-authentication")
                {
                    if (root.TryGetProperty("status", out var statusElement))
                    {
                        var status = JsonSerializer.Deserialize<string>(statusElement.GetRawText());
                        IsAuthenticated = status == "ok";
                    }
                    else
                    {
                        IsAuthenticated = false;
                    }

                    return new ResponseMessage { ResponseType = ResponseType.Authentication };
                }
            }

            // Check if it's a get action list response
            if (root.TryGetProperty("actions", out var actionsElement))
            {
                if (root.TryGetProperty("id", out var identifierElement))
                {
                    var identifier = JsonSerializer.Deserialize<string>(identifierElement.GetRawText());
                    if (identifier == "dichternebel-yasb-get-actions")
                    {
                        var actions = JsonSerializer.Deserialize<List<StreamerBot.Action>>(actionsElement.GetRawText(), options);
                        return new ResponseMessage { ResponseType = ResponseType.Actions, Actions = actions, RawMessage = prettyJson };
                    }
                }
            }

            // Check if it's a get event list response
            if (root.TryGetProperty("events", out var eventsElement))
            {
                if (root.TryGetProperty("id", out var identifierElement))
                {
                    var identifier = JsonSerializer.Deserialize<string>(identifierElement.GetRawText());
                    if (identifier == "dichternebel-yasb-get-events")
                    {
                        var eventDictionary = JsonSerializer.Deserialize<Dictionary<string, string[]>>(eventsElement.GetRawText(), options);
                        return new ResponseMessage { ResponseType = ResponseType.Events, Events = eventDictionary, RawMessage = prettyJson };
                    }
                }
            }

            // Check if it's a get globals response
            if (root.TryGetProperty("variables", out var globalsElement))
            {
                if (root.TryGetProperty("id", out var identifierElement))
                {
                    var identifier = JsonSerializer.Deserialize<string>(identifierElement.GetRawText());
                    if (identifier == "dichternebel-yasb-get-globals")
                    {
                        var globalDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(globalsElement.GetRawText(), options);

                        var eventInfo = new StreamerBot.EventInfo();
                        eventInfo.Source = "Misc";
                        eventInfo.Type = "GlobalVariableUpdated";

                        var eventData = new EventData
                        {
                            KeyValuePairs = new Dictionary<string, object>()
                        };
                        foreach (var item in globalDictionary)
                        {
                            eventData.KeyValuePairs.Add(item.Key, item.Value);
                        }

                        return new ResponseMessage
                        {
                            ResponseType = ResponseType.Globals,
                            Event = new BotEvent
                            {
                                EventInfo = eventInfo,
                                Data = eventData
                            }
                        };
                    }
                }
            }

            // Check if it's an event being fired
            if (root.TryGetProperty("event", out var eventElement)
                && root.TryGetProperty("data", out var dataElement)
                && root.TryGetProperty("timeStamp", out var timeStampElement))
            {
                var timeStamp = JsonSerializer.Deserialize<DateTime>(timeStampElement.GetRawText(), options);
                var eventInfo = JsonSerializer.Deserialize<EventInfo>(eventElement.GetRawText(), options);

                var eventData = new EventData
                {
                    KeyValuePairs = new Dictionary<string, object>()
                };
                foreach (var property in dataElement.EnumerateObject())
                {
                    eventData.KeyValuePairs.Add(property.Name, property.Value);
                }

                return new ResponseMessage
                {
                    ResponseType = ResponseType.Event,
                    Event = new BotEvent
                    {
                        TimeStamp = timeStamp,
                        EventInfo = eventInfo,
                        Data = eventData
                    },
                    RawMessage = prettyJson
                };
            }

            return new ResponseMessage { ResponseType = ResponseType.Unknown, RawMessage = prettyJson };
        }

        public async Task SendMessageAsync(string actionId, string actionName, string actionArgument = "")
        {
            object args =new { };

            try
            {
                args = string.IsNullOrEmpty(actionArgument) ? new { } : JsonSerializer.Deserialize<object>(actionArgument);
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance,$"Error serializing argument:\n{ex.Message}");
            }

            var request = new
            {
                request = RequestType.DoAction.ToString(),
                id = "dichternebel-yasb-do-action",
                action = new
                {
                    id = actionId,
                    name = actionName
                },
                args
            };

            string payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            var bytes = Encoding.UTF8.GetBytes(payload);
            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cts.Token);

            MacroDeckLogger.Info(Main.Instance, $"DoAction({actionName}) request sent successfully.");

        }

        public async Task SwitchHostAsync(string serverUrl)
        {
            _serverUrl = serverUrl;
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Switching host", _cts.Token);
        }

        public async Task AuthenticateAsync(string password, string salt, string challenge)
        {
            // Compute SHA-256 hash of password and salt
            string secret = ComputeSha256Hash($"{password}{salt}");

            // Compute SHA-256 hash of secret and challenge
            AuthenticationMessage = ComputeSha256Hash($"{secret}{challenge}");

            var request = new
            {
                request = RequestType.Authenticate.ToString(),
                id = "dichternebel-yasb-authentication",
                authentication = AuthenticationMessage
            };

            string payload = JsonSerializer.Serialize(request);
            var bytes = Encoding.UTF8.GetBytes(payload);
            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                _cts.Token);

            MacroDeckLogger.Info(Main.Instance, "Authentication request sent successfully.");
        }

        private static string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _webSocket.Dispose();
            Main.Model.IsConnectedToStreamerBot = false;
        }
    }
}
