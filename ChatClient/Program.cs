using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(SocketType.Stream, ProtocolType.IP);
            socket.Connect(new IPEndPoint(IPAddress.Loopback, 10111));

            Console.WriteLine($"Clinet connected Local {socket.LocalEndPoint} Remote {socket.RemoteEndPoint}");

            string chatContent = RecieveString(socket);

            Console.WriteLine(chatContent);

            Console.ReadLine();

            socket.Close();
            socket.Dispose();


        }

        private static string RecieveString(Socket socket)
        {
            byte[] dataBuffer = new byte[4096];

            var recievedBytesCount = socket.Receive(dataBuffer);

            using (var recieveMemoryStream = new MemoryStream())
            using (TextReader receivedTextReader = new StreamReader(recieveMemoryStream))
            {
                recieveMemoryStream.Write(dataBuffer, 0, recievedBytesCount);
                recieveMemoryStream.Seek(0, SeekOrigin.Begin);
                var chatContent = receivedTextReader.ReadToEnd();
                return chatContent.TrimEnd('\0');
            }
        }
    }
}
