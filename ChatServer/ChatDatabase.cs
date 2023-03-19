using System.Collections.Generic;
using System.Linq;

namespace ChatServer
{
    internal static class ChatDatabase
    {
        private static List<string> _chatLines = new List<string>() { "asasas", "asasas"};

        public static void AddMessage(string message)
        {
            _chatLines.Add(message);
        }

        public static string GetChat()
        {
            return _chatLines
                .Aggregate("", (accumulate, line) => $"{accumulate}\n{line}")
                .TrimStart('\n');
        }
    }
}
