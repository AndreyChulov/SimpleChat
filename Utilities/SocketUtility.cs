using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Utilities
{
    public static class SocketUtility
    {
        public static string ReceiveString(Socket clientSocket, 
            Action onReceiveDataSizeCheckFail, Action onReceiveDataCheckFail)
        {
            using (Stream dataStream = new MemoryStream())
            using (BinaryReader dataStreamReader = new BinaryReader(dataStream))
            {
                var stringSize = ReceiveStringSize(clientSocket, dataStream, dataStreamReader, onReceiveDataSizeCheckFail);
                return ReceiveString(clientSocket, stringSize + 1, dataStream, dataStreamReader, onReceiveDataCheckFail);
            }
        }
        
        private static string ReceiveString(Socket clientSocket, int stringSize, 
            Stream dataStream, BinaryReader dataStreamReader, Action onReceiveDataCheckFail)
        {
            WaitDataFromClient(clientSocket, stringSize);
            
            byte[] dataBuffer = new byte[stringSize];
            var receivedBufferSize = clientSocket.Receive(dataBuffer);

            if (receivedBufferSize != dataBuffer.Length)
            {
                onReceiveDataCheckFail();
            }

            dataStream.Seek(0, SeekOrigin.Begin);
            dataStream.Write(dataBuffer, 0, dataBuffer.Length);
            dataStream.Seek(0, SeekOrigin.Begin);
            return dataStreamReader.ReadString();
        }

        private static int ReceiveStringSize(Socket clientSocket, Stream dataStream, 
            BinaryReader dataStreamReader, Action onReceiveDataCheckFail)
        {
            WaitDataFromClient(clientSocket, sizeof(int));
            byte[] dataBuffer = new byte[sizeof(int)];
            var receivedBufferSize = clientSocket.Receive(dataBuffer);

            if (receivedBufferSize != dataBuffer.Length)
            {
                onReceiveDataCheckFail();
            }

            dataStream.Seek(0, SeekOrigin.Begin);
            dataStream.Write(dataBuffer, 0, dataBuffer.Length);
            dataStream.Seek(0, SeekOrigin.Begin);
            return dataStreamReader.ReadInt32();
        }

        public static void WaitDataFromClient(Socket clientSocket)
        {
            WaitDataFromClient(clientSocket, 1);
        }

        private static void WaitDataFromClient(Socket clientSocket, int waitForBytesAvailable)
        {
            while (clientSocket.Available < waitForBytesAvailable)
            {
                Thread.Sleep(100);
            }
        }

        public static void SendString(Socket clientSocket, string dataToSend, Action onSendDataCheckFail)
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
                    onSendDataCheckFail();
                }

                clientSocket.Send(sendDataBuffer);
            }
        }

    }
}