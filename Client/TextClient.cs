using SocketWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class TextClient
    {

        ISocket _listener;
        byte[] buffer = new byte[2048];

        public TextClient() : this(new SocketAdapter())
        {

        }

        public TextClient(ISocket socket)
        {
            _listener = socket;
        }

        public void StartClient()
        {

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("10.0.75.1"), 1232);
            _listener.Blocking = false;

            _listener.BeginConnect(serverEndPoint, new AsyncCallback(OnConnect), _listener);
        }

        public void Sendmessage(string msg)
        {
            Byte[] byteDateLine = Encoding.Unicode.GetBytes(msg);
            _listener.Send(byteDateLine, byteDateLine.Length, 0);
        }

        public void OnConnect(IAsyncResult arg)
        {
            var sock = (SocketAdapter)arg.AsyncState;
            SetupRecieveCallback(sock);
        }

        public void OnRecievedData(IAsyncResult arg)
        {
            var sock = (SocketAdapter)arg.AsyncState;
            int nBytesRec = sock.EndReceive(arg);
            string sRecieved = Encoding.Unicode.GetString(buffer, 0, nBytesRec);
            Console.WriteLine(sRecieved); //Todo: event handler
            SetupRecieveCallback(sock);
        }

        public void SetupRecieveCallback(ISocket sock)
        {

            AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
            sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, recieveData, sock);
        }
    }
}
