using System.Net.Sockets;

namespace ChatServer.EventsArgs
{
    public class ChatContentSentToClientArgs
    {
        public Socket ClientSocket { get; }
        
        public static ChatContentSentToClientArgs Create(Socket clientSocket)
        {
            return new ChatContentSentToClientArgs(clientSocket);
        }
        
        private ChatContentSentToClientArgs(Socket clientSocket)
        {
            ClientSocket = clientSocket;
        }

        
    }
}