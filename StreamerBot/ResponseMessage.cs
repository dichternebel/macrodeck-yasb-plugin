namespace dichternebel.YaSB.StreamerBot
{
    public class ResponseMessage
    {
        public ResponseType ResponseType { get; set; }
        public Info Info { get; set; }
        public Authentication Authentication { get; set; }
        public Dictionary<string, string[]> Events { get; set; }
        public List<Action> Actions { get; set; }
        public BotEvent Event { get; set; }
        public string RawMessage { get; set; }
    }
}