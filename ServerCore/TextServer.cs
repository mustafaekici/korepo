using SocketWrapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
namespace ServerCore
{
    public class TextServer : IServer
    {

        ISocket _listener;
        IDns _dns;
        TextClientPackage _textClient;
        public static DateTime firstTime;
        public static DateTime lastTime = DateTime.Now;

        internal static List<TextClientPackage> connectedClients { get; set; }
        public TextServer() : this(new SocketAdapter(), new DnsAdapter())
        {
            connectedClients = new List<TextClientPackage>();
        }

        public TextServer(ISocket socket, IDns dns)
        {
            if (socket == null)
                throw new ArgumentNullException();

            if (dns == null)
                throw new ArgumentNullException();

            _listener = socket;
            _dns = dns;
        }

        public string StartServer(int port)
        {

            IPAddress[] addresses = null;
            string serverHostName = "";
            IPHostEntry ipEntry;

            try
            {
                serverHostName = _dns.GetHostName();
                ipEntry = _dns.GetHostByName(serverHostName);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
    
            addresses = ipEntry.AddressList;
            if (addresses == null || addresses.Length < 1)
            {
                return "Couldnt get local address.";
            }

            _listener.Bind(new IPEndPoint(addresses[0], port));
            _listener.Listen(10);
            _listener.BeginAccept(new AsyncCallback(AcceptClient), _listener);

            return ($"Listening On: {addresses[0]} : {port}");

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

            byte[] info = System.Text.Encoding.Unicode.GetBytes("Connected to server!...\nClients in the Room:");
            _textClient.GetSocket.Send(info, info.Length, 0);

            foreach (var item in connectedClients)
            {
                //send connected clients except newbie info to newbie
                if (item.GetSocket.RemoteEndPoint.ToString() != _textClient.GetSocket.RemoteEndPoint.ToString())
                {
                    byte[] endpoint = System.Text.Encoding.Unicode.GetBytes(item.GetSocket.RemoteEndPoint.ToString() + "\n");
                    _textClient.GetSocket.Send(endpoint, endpoint.Length, 0);
                }
                //and send newbie info to all clients except newbie
                if (item.GetSocket.RemoteEndPoint.ToString() != _textClient.GetSocket.RemoteEndPoint.ToString())
                {
                    byte[] connectedclientinfo = System.Text.Encoding.Unicode.GetBytes(_textClient.GetSocket.RemoteEndPoint.ToString() + " Connected to server!");
                    item.GetSocket.Send(connectedclientinfo, connectedclientinfo.Length, 0);
                }
            }

            _textClient.SetupRecieveCallback();
        }

        public static void OnRecievedData(IAsyncResult arg)
        {
            firstTime = DateTime.Now;
            var timeres = (firstTime - lastTime).TotalSeconds;
            TextClientPackage client = (TextClientPackage)arg.AsyncState;
            if (timeres > 1)
            {
                client = (TextClientPackage)arg.AsyncState;
                string aryRet = client.GetRecievedData(arg);

                //send to all clients or single

                var clientip = string.Concat(aryRet.TakeWhile(x => x != '_'));
                var clientmsg = string.Concat(aryRet.SkipWhile(x => x != '_')).Remove(0, 1);
                var sendto = connectedClients.Where(x => x.GetSocket.RemoteEndPoint.ToString() == clientip).FirstOrDefault();

                var bytedata = System.Text.Encoding.Unicode.GetBytes(client.GetSocket.RemoteEndPoint.ToString() + "says : " + clientmsg);
                sendto?.GetSocket.Send(bytedata, bytedata.Length, 0);
            }
            else
            {
                //Warn Client
                var warninfo = System.Text.Encoding.Unicode.GetBytes("Server: You cant send more than one msg per sec");
                client.GetSocket.Send(warninfo, warninfo.Length, 0);

                //Todo: create blacklist to kick client. close connection  client.GetSocket.Close();
            }
            client.SetupRecieveCallback();
            lastTime = firstTime;

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
    public class TextClientPackage
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

                var timeout = _newSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, recieveData, this);
                bool intime = timeout.AsyncWaitHandle.WaitOne(10000, true);
                //if(!intime)
                //{
                //    //TODO
                //}

            }
            catch (Exception)
            {
            }
        }
        public string GetRecievedData(IAsyncResult arg)
        {

            string sRecieved;
            int nBytesRec = _newSocket.EndReceive(arg);
            if (nBytesRec > 0)
            {
                sRecieved = Encoding.Unicode.GetString(buffer, 0, nBytesRec);
            }
            else
            {
                sRecieved = null;
                _newSocket.Shutdown(SocketShutdown.Both);
                _newSocket.Close();
            }

            return sRecieved;

        }
    }
}
