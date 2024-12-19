using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SuchByte.MacroDeck.Logging;
using dichternebel.YaSB.StreamerBot;
using System.Security.Cryptography;

namespace dichternebel.YaSB
{
    public class WebSocketClient
    {
        private ClientWebSocket _webSocket;
        private string _serverUrl;
        private readonly CancellationTokenSource _cts;

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
                            await ConnectToServerAsync();
                            await GetEventsAsync();
                            await GetActionsAsync();
                            //await SubscribeToServerAsync();
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

            await _webSocket.ConnectAsync(new Uri(_serverUrl), _cts.Token);
            Main.Model.IsConnectedToStreamerBot = true;
            MacroDeckLogger.Info(Main.Instance, "Connected to WebSocket server.");
        }

        private async Task GetEventsAsync()
        {
            try
            {
                var request = new
                {
                    request = RequestType.GetEvents.ToString(),
                    id = "dichternebel-yasb-get-events"
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

                MacroDeckLogger.Info(Main.Instance, "GetEvents request sent successfully.");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to get events from WebSocket server: {ex.Message}");
            }
        }

        private async Task GetActionsAsync()
        {
            try
            {
                var request = new
                {
                    request = RequestType.GetActions.ToString(),
                    id = "dichternebel-yasb-get-actions"
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
                    events = eventsDictionary
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
                var eventsDictionary = Main.Model?.ConfiguredEvents;

                if (eventsDictionary == null || !eventsDictionary.Any()) return;

                var request = new
                {
                    request = RequestType.Subscribe.ToString(),
                    id = "dichternebel-yasb-subscribe",
                    events = eventsDictionary
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
                MacroDeckLogger.Info(Main.Instance, $"Subscription request sent successfully:\n{payload}");
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(Main.Instance, $"Failed to subscribe to WebSocket server: {ex.Message}");
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
            MacroDeckLogger.Trace(Main.Instance, $"Received message:\n{prettyJson}");

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

            // Check if it's an get action list response
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

            // Check if it's an get event list response
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

            MacroDeckLogger.Info(Main.Instance, "Do-Action request sent successfully.");

        }

        public async Task SwitchHostAsync(string serverUrl)
        {
            _serverUrl = serverUrl;
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Switching host", _cts.Token);
        }

        // ToDo: getting { "error": "malformed command" }
        // No idea how to authenticate against Streamer.Bot, couldn't find any examples
        public async Task AuthenticateAsync(string password, string salt, string challenge)
        {
            // Combine the password, salt, and challenge
            var combined = password + salt + challenge;

            MacroDeckLogger.Info(Main.Instance, combined);

            // Hash the combined string using SHA256
            using var sha256 = SHA256.Create();
            var shaBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            var responseMessage = Convert.ToHexString(shaBytes);

            var request = new
            {
                request = RequestType.Authenticate.ToString(),
                responseMessage
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

            MacroDeckLogger.Info(Main.Instance, "Authentication request sent successfully.");
        }

        public void Dispose()
        {
            _cts.Cancel();
            _webSocket.Dispose();
            Main.Model.IsConnectedToStreamerBot = false;
        }
    }
}
