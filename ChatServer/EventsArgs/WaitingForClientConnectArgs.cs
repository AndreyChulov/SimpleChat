using System.Net.Sockets;

namespace ChatServer.EventsArgs
{
    public class WaitingForClientConnectArgs
    {
        public Socket ServerSocket { get; }
        
        public static WaitingForClientConnectArgs Create(Socket serverSocket)
        {
            return new WaitingForClientConnectArgs(serverSocket);
        }
        
        private WaitingForClientConnectArgs(Socket serverSocket)
        {
            ServerSocket = serverSocket;
        }

        
    }
}