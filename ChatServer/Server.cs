using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Server : IDisposable
    {
        private const int MAX_CLIENTS_WAITING_FOR_CONNECT = 5;

        public event EventHandler<Exception> AcceptClientException;
        public event EventHandler WaitingForClientConnect;
        public event EventHandler<Socket> ClientConnected;

        private int _serverPort;
        private Socket _serverSocket;
        private bool _isServerAlive;

        public static Server Initialise(int listeningPort)
        {
            return new Server(listeningPort);
        }

        private Server(int serverPort)
        {
            _serverPort = serverPort;
        }

        public void Start()
        {
            _serverSocket = new Socket(SocketType.Stream, ProtocolType.IP);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, _serverPort));
            _serverSocket.Listen(MAX_CLIENTS_WAITING_FOR_CONNECT);
            _isServerAlive = true;

            StartClientTask();
        }

        private void StartClientTask()
        {
            Task.Run(() => ClientWorker());
        }

        public void Stop()
        {
            _isServerAlive = false;
            _serverSocket.Close();
        }

        public void Dispose()
        {
            Stop();
            _serverSocket?.Dispose();
        }

        private void ClientWorker()
        {
            Socket clientSocket = null;

            WaitingForClientConnect?.Invoke(this, EventArgs.Empty);

            try
            {
                clientSocket = _serverSocket.Accept();
            } 
            catch (SocketException ex)
            {
                AcceptClientException?.Invoke(this, ex);
            }
            catch (ObjectDisposedException ex)
            {
                AcceptClientException?.Invoke(this, ex);
            }
            catch (InvalidOperationException ex)
            {
                AcceptClientException?.Invoke(this, ex);
            }

            ClientConnected?.Invoke(this, clientSocket);
            StartClientTask();

            Stream ff = new MemoryStream();
            byte[] buffer = new byte[4096];
            TextWriter tw = new StreamWriter(ff);
            tw.Write(ChatDatabase.GetChat());
            tw.Flush();
            ff.Seek(0, SeekOrigin.Begin);
            ff.Read(buffer, 0, 4096);

            clientSocket.Send(buffer);

            Thread.CurrentThread.Join();
            //clientSocket.Close();
        }
    }
}
