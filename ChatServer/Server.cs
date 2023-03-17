using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatServer.EventsArgs;

namespace ChatServer
{
    internal class Server : IDisposable
    {
        private const int MAX_CLIENTS_WAITING_FOR_CONNECT = 5;

        public event EventHandler<Exception> AcceptClientException;
        public event EventHandler<SendDataToClientExceptionArgs> SendDataToClientException;
        public event EventHandler<ChatContentSentToClientArgs> ChatContentSentToClient;
        public event EventHandler<WaitingForClientConnectArgs> WaitingForClientConnect;
        public event EventHandler<ClientConnectedArgs> ClientConnected;

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
            Socket clientSocket = AcceptClient();

            StartClientTask();

            SendString(clientSocket, ChatDatabase.GetChat());
            ChatContentSentToClient?.Invoke(this, ChatContentSentToClientArgs.Create(clientSocket));

            while (clientSocket.Available == 0)
            {
                Thread.Sleep(100);
            }



            Thread.CurrentThread.Join();
            //clientSocket.Close();
        }

        private void SendString(Socket clientSocket, string dataToSend)
        {
            using (Stream dataStream = new MemoryStream())
            using (BinaryWriter dataStreamWriter = new BinaryWriter(dataStream))
            {
                dataStreamWriter.Write(dataToSend.Length);
                dataStreamWriter.Write(dataToSend);
                dataStreamWriter.Flush();
            
                byte[] sendDataBuffer = new byte[dataStream.Position];

                dataStream.Seek(0, SeekOrigin.Begin);
                
                int readBytesFromMemoryStream = dataStream.Read(sendDataBuffer, 0, sendDataBuffer.Length);

                if (readBytesFromMemoryStream != sendDataBuffer.Length)
                {
                    SendDataToClientException?.Invoke(this,
                        SendDataToClientExceptionArgs.Create(
                            new Exception("Preparation data for socket send check fail"),
                            clientSocket,
                            sendDataBuffer,
                            dataToSend
                            )
                        );
                }

                clientSocket.Send(sendDataBuffer);
            }
        }

        private Socket AcceptClient()
        {

            Socket clientSocket = null;

            WaitingForClientConnect?.Invoke(this, WaitingForClientConnectArgs.Create(_serverSocket));

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

            ClientConnected?.Invoke(this, ClientConnectedArgs.Create(_serverSocket, clientSocket));

            return clientSocket;
        }
    }
}
