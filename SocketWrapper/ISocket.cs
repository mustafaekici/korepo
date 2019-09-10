using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketWrapper
{
    //abstract socket for mocking
    public interface ISocket
    {
        bool Blocking { get; set; }

        void Bind(EndPoint localEP);
        void Listen(int backlog);
        IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state);
        IAsyncResult BeginAccept(AsyncCallback callback, object state);
        Socket EndAccept(IAsyncResult asyncResult);
        IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state);
        int EndReceive(IAsyncResult asyncResult);
        void Connect(string host, int port);
        void Close();
        int Receive(byte[] buffer, int size, SocketFlags flags);
        int Send(byte[] buffer, int size, SocketFlags flags);
        void Shutdown(SocketShutdown how);
    }
}
