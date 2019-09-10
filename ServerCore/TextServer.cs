using SocketWrapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class TextServer : IServer
    {
        ISocket _listener;
        Transmission _transmission;
        TextClientPackage _textClient;
        internal List<TextClientPackage> connectedClients { get; set; }
        public TextServer(Transmission transmission) : this(new SocketAdapter(), transmission)
        {
            connectedClients = new List<TextClientPackage>();
        }
        public TextServer(ISocket socket, Transmission transmission)
        {
            _listener = socket;
            _transmission = transmission;
        }
        public string StartServer(int port, int maxConnections)
        {
            try
            {
                IPAddress[] addresses = null;
                string serverHostName = "";
                try
                {
                    serverHostName = Dns.GetHostName();
                    IPHostEntry ipEntry = Dns.GetHostByName(serverHostName);
                    addresses = ipEntry.AddressList;
                }
                catch (Exception) { }

                if (addresses == null || addresses.Length < 1)
                {
                    return "Couldnt get local address.";
                }


                //check null

                _listener.Bind(new IPEndPoint(addresses[0], port));
                _listener.Listen(10);

                _listener.BeginAccept(new AsyncCallback(AcceptClient), _listener);

                return ("Listening On Port " + port);

            }
            catch (Exception ex) { return ex.Message; }
        }

        private void AcceptClient(IAsyncResult arg)
        {

            _listener = (SocketAdapter)arg.AsyncState;
            CreateSocketForClients(_listener.EndAccept(arg));
            _listener.BeginAccept(new AsyncCallback(AcceptClient), _listener);
        }

        private void CreateSocketForClients(Socket sockClient)
        {
            //Crete socket package for client
            _textClient = new TextClientPackage(new SocketAdapter(sockClient));
            //add to client list
            connectedClients.Add(_textClient);

            //todo: create clientlist for unblocked-blocked clients

            byte[] info = System.Text.Encoding.Unicode.GetBytes("Connected to server!");
            _textClient.GetSocket.Send(info, info.Length, 0);
            _textClient.SetupRecieveCallback();
        }

        protected internal static void OnRecievedData(IAsyncResult arg)
        {
            TextClientPackage client = (TextClientPackage)arg.AsyncState;
            byte[] aryRet = client.GetRecievedData(arg);
            //send to all clients or single

        }

        public string StopServer()
        {
            try
            {
                _listener.Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return ("Shutdown");
            }
            catch (Exception ex) { return (ex.Message); }

        }


    }
    internal class TextClientPackage
    {
        // To create a new socket for each client 

        private ISocket _newSocket;
        private byte[] buffer = new byte[50];

        public TextClientPackage(ISocket passedSock)
        {
            _newSocket = passedSock;
        }

        public ISocket GetSocket
        {
            get { return _newSocket; }
        }

        public void SetupRecieveCallback()
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(TextServer.OnRecievedData);
                _newSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, recieveData, this);

            }
            catch (Exception)
            {
            }
        }
        public byte[] GetRecievedData(IAsyncResult arg)
        {
            int nBytesRec = 0;
            try
            {
                nBytesRec = _newSocket.EndReceive(arg);
            }
            catch { }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(buffer, byReturn, nBytesRec);
            return byReturn;
        }

    }
}
