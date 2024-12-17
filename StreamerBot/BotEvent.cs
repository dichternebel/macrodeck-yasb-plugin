namespace dichternebel.YaSB.StreamerBot
{
    public class BotEvent
    {
        public DateTime TimeStamp { get; set; }
        public EventInfo EventInfo { get; set; }
        public EventData Data { get; set; }
    }

    public class EventInfo
    {
        public string Source { get; set; }
        public string Type { get; set; }
    }

    public class EventData
    {
        public Dictionary<string, object> KeyValuePairs { get; set; }
    }
}
