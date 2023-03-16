using System;

namespace ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var server = Server.Initialise(10111))
            {
                server.AcceptClientException += Server_AcceptClientException;
                server.WaitingForClientConnect += Server_WaitingForClientConnect;
                server.ClientConnected += Server_ClientConnected;
                server.Start();

                Console.ReadLine();
                server.Stop();
            }
        }

        private static void Server_ClientConnected(object sender, System.Net.Sockets.Socket e)
        {
            Console.WriteLine($"Client with IP {e.RemoteEndPoint} connected");
        }

        private static void Server_WaitingForClientConnect(object sender, EventArgs e)
        {
            Console.WriteLine($"Server waiting for client connection");
        }

        private static void Server_AcceptClientException(object sender, Exception e)
        {
            Console.WriteLine($"Server caused exception while client acception {e.Message}");
        }
    }
}
