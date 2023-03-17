using System;
using System.Net.Sockets;

namespace ChatServer.EventsArgs
{
    public class SendDataToClientExceptionArgs
    {
        public Exception Exception { get; }
        public Socket ClientSocket { get; }
        public byte[] DataToSend { get; }
        public string StringToSend { get; }

        public static SendDataToClientExceptionArgs Create(Exception exception, Socket clientSocket, 
            byte[] dataToSend, string stringToSend)
        {
            return new SendDataToClientExceptionArgs(exception, clientSocket, dataToSend, stringToSend);
        }

        private SendDataToClientExceptionArgs(Exception exception, Socket clientSocket, 
            byte[] dataToSend, string stringToSend)
        {
            Exception = exception;
            ClientSocket = clientSocket;
            DataToSend = dataToSend;
            StringToSend = stringToSend;
        }
    }
}