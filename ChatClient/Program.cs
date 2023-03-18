using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ChatClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.IP);
            socket.Connect(new IPEndPoint(IPAddress.Loopback, 10111));

            Console.WriteLine($"Clinet connected Local {socket.LocalEndPoint} Remote {socket.RemoteEndPoint}");

            string chatContent = SocketUtility.ReceiveString(socket,
                () =>
                {
                    Console.WriteLine($"Receive string size check from server client side exception");
                }, () =>
                {
                    Console.WriteLine($"Receive string data check from server client side exception");
                });

            Console.WriteLine(chatContent);
            
            Console.Write("Your message:");
            var message = Console.ReadLine();
            SocketUtility.SendString(socket, message, () => {});

            Console.ReadLine();

            socket.Close();
            socket.Dispose();


        }
    }
}
