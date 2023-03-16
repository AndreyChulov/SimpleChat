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

            Console.WriteLine(socket.ToString());

            byte[] buffer = new byte[4096];
            var chatBytes = socket.Receive(buffer);
            var ms = new MemoryStream();
            ms.Write(buffer, 0, chatBytes);
            TextReader tr = new StreamReader(ms);
            ms.Seek(0, SeekOrigin.Begin);
            var chatContent = tr.ReadToEnd();

            Console.WriteLine(chatContent.TrimEnd('\0'));

            Console.ReadLine();

            socket.Close();
            socket.Dispose();


        }
    }
}
