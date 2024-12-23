namespace dichternebel.YaSB
{
    internal static class Helper
    {
        // Helper methods for the configuration format
        public static string CreateEventKey(string group, string eventName) => $"{group}_{eventName}";

        public static Dictionary<string, string[]> ExplodeEventKey(string eventIdentifier)
        {
            var parts = eventIdentifier.Split(new[] { '_' }, 2);
            return new Dictionary<string, string[]>() { { parts[0], new[] { parts[1] } } };
        }
    }
}
