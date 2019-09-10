using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketWrapper
{
    public class SocketAdapter : ISocket
    {
        private Socket _socket;
        public Socket Socket
        {
            get { return _socket; }
            set { _socket = value; }
        }

        public SocketAdapter()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public SocketAdapter(Socket socket)
        {
            _socket = socket;
        }

        public void Connect(string host, int port)
        {
            _socket.Connect(host, port);
        }

        public void Close()
        {
            _socket.Close();
        }

        public int Receive(byte[] buffer, int size, SocketFlags flags)
        {
            return _socket.Receive(buffer, size,  flags);
        }

        public int Send(byte[] buffer, int size, SocketFlags flags)
        {
            return _socket.Send(buffer,size,flags);
        }

        public void Bind(EndPoint localEP)
        {
            _socket.Bind(localEP);
        }

        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            return _socket.BeginAccept(callback, state);
        }

        public Socket EndAccept(IAsyncResult asyncResult)
        {
            return (Socket)_socket.EndAccept(asyncResult);
        }

        public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            return _socket.BeginReceive(buffer, offset, size, socketFlags, callback, state);
        }

        public int EndReceive(IAsyncResult asyncResult)
        {
            return _socket.EndReceive(asyncResult);
        }

        public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
        {
            return _socket.BeginConnect(remoteEP, callback, state);
        }

        public void Shutdown(SocketShutdown how)
        {
            throw new NotImplementedException();
        }
    }
}
