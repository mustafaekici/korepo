using SocketWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientConnection
{
    public class TextClient : IDisposable
    {

        ISocket _listener;
        byte[] buffer = new byte[2048];
        public event Action<string> OnMessageReceived;

        public TextClient() : this(new SocketAdapter())
        {

        }

        public TextClient(ISocket socket)
        {
            if (socket == null)
                throw new ArgumentNullException();

            _listener = socket;
        }

        public void StartClient(string serverIp, int port)
        {

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);
            _listener.Blocking = false;
            _listener.BeginConnect(serverEndPoint, new AsyncCallback(OnConnect), _listener);

        }

        public void Sendmessage(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Byte[] byteDateLine = Encoding.Unicode.GetBytes(msg);
                int result = _listener.Send(byteDateLine, byteDateLine.Length, 0);
            }
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
            OnMessageReceived?.Invoke(sRecieved);
            SetupRecieveCallback(sock);
        }

        public void SetupRecieveCallback(ISocket sock)
        {

            AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
            sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, recieveData, sock);
        }

        #region IDisposable Member

        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.Close();
            }
        }

        #endregion
    }
}
