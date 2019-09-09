using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class TextServer : IServer
    {

        Socket _listener;
        Transmission _transmission;
        TextClient _textClient;

        public TextServer(Transmission transmission)
        {
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

                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                _listener.Bind(new IPEndPoint(addresses[0].Address, port));
                _listener.Listen(10);

                _listener.BeginAccept(new AsyncCallback(AcceptClient), _listener);

                return ("Listening On Port " + port);

            }
            catch (Exception ex) { return ex.Message; }
        }

        private void AcceptClient(IAsyncResult arg)
        {
            try
            {
                _listener = (Socket)arg.AsyncState;

                CreateSocketForClients(_listener.EndAccept(arg));
                _listener.BeginAccept(new AsyncCallback(AcceptClient), _listener);
            }
            catch (Exception) { }
        }

        private void CreateSocketForClients(Socket sockClient)
        {
            //not thread safe!
            _textClient = new TextClient(sockClient);

            //todo: create clientlist for unblocked-blocked clients

            Byte[] byteDateLine = System.Text.Encoding.Unicode.GetBytes("Connected to server!");
            _textClient.ReadOnlySocket.Send(byteDateLine, byteDateLine.Length, 0);
            _textClient.SetupRecieveCallback();
        }

         
        protected internal static void OnRecievedData(IAsyncResult arg)
        {
            //Send received data to all clients
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
    internal class TextClient
    {
        // To create a new socket for each client 

        private Socket _newSocket;
        private byte[] buffer = new byte[50];

        public TextClient(Socket passedSock)
        {
            _newSocket = passedSock;
        }

        public Socket ReadOnlySocket
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
