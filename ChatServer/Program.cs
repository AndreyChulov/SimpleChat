using System;
using System.Net;

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
            var remoteEndPoint = (IPEndPoint)e.RemoteEndPoint;
            Console.WriteLine($"Client with IP {remoteEndPoint.Address.MapToIPv4()} connected");
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
