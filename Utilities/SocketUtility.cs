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
                var dataSize = ReceiveDataSize(clientSocket, dataStream, dataStreamReader, onReceiveDataSizeCheckFail);
                ReceiveDataToStream(clientSocket, dataSize, dataStream, onReceiveDataCheckFail);
                
                dataStream.Seek(0, SeekOrigin.Begin);
                return dataStreamReader.ReadString();
            }
        }
        
        private static void ReceiveDataToStream(
            Socket clientSocket, long dataSize, 
            Stream dataStream, Action onReceiveDataCheckFail)
        {
            var maxBufferSize = 1024;
            var remainingDataSize = dataSize;

            dataStream.Seek(0, SeekOrigin.Begin);
            
            while (remainingDataSize > maxBufferSize)
            {
                ReceiveBufferToStream(clientSocket, dataStream, maxBufferSize, onReceiveDataCheckFail);

                remainingDataSize -= maxBufferSize;
            }
            
            ReceiveBufferToStream(clientSocket, dataStream, (int)remainingDataSize, onReceiveDataCheckFail);
        }

        private static void ReceiveBufferToStream(
            Socket clientSocket, Stream dataStream, int bufferSize,
            Action onReceiveDataCheckFail)
        {
            WaitDataFromClient(clientSocket, bufferSize);

            byte[] dataBuffer = new byte[bufferSize];
            var receivedBufferSize = clientSocket.Receive(dataBuffer);

            if (receivedBufferSize != bufferSize)
            {
                onReceiveDataCheckFail();
            }

            dataStream.Write(dataBuffer, 0, bufferSize);
        }

        private static long ReceiveDataSize(Socket clientSocket, Stream dataStream, 
            BinaryReader dataStreamReader, Action onReceiveDataCheckFail)
        {
            WaitDataFromClient(clientSocket, sizeof(long));
            byte[] dataBuffer = new byte[sizeof(long)];
            var receivedBufferSize = clientSocket.Receive(dataBuffer);

            if (receivedBufferSize != dataBuffer.Length)
            {
                onReceiveDataCheckFail();
            }

            dataStream.Seek(0, SeekOrigin.Begin);
            dataStream.Write(dataBuffer, 0, dataBuffer.Length);
            dataStream.Seek(0, SeekOrigin.Begin);
            return dataStreamReader.ReadInt64();
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
                /*
                 * записываем пустышку вместо размера пакета данных,
                 * на данном этапе мы не знаем размер отправляемых данных
                 */
                dataStreamWriter.Write((long)0);
                
                dataStreamWriter.Write(dataToSend);
                dataStreamWriter.Flush();
            
                byte[] sendDataBuffer = new byte[dataStream.Position];

                /*
                 * Перезаписываем актуальный размер пакета данных,
                 * теперь мы знаем его размер
                 */
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStreamWriter.Write(dataStream.Length - sizeof(long));
                
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