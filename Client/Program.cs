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
    class Program
    {
         
        static void Main(string[] args)
        {
          
            //ClientSocket.Blocking = false;
            Test.Start();
            Test.Sendmessgae(Console.ReadLine());
            Console.ReadKey();


        }
     

    }

    class Test
    {
        static Socket ClientSocket;
        public static void Start()
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint Server_EndPoint = new IPEndPoint(IPAddress.Parse("10.0.75.1"), 1232);
            ClientSocket.Blocking = false;

            ClientSocket.BeginConnect(Server_EndPoint, new AsyncCallback(OnConnect), ClientSocket);
        }
        public static void Sendmessgae(string msg)
        {
            Byte[] byteDateLine = Encoding.Unicode.GetBytes(msg);
            ClientSocket.Send(byteDateLine, byteDateLine.Length, 0);
        }
        public static void OnConnect(IAsyncResult arg)
        {
            Socket sock = (Socket)arg.AsyncState;
            SetupRecieveCallback(sock);
        }
        public static void OnRecievedData(IAsyncResult arg)
        {
            Socket sock = (Socket)arg.AsyncState;
            int nBytesRec = sock.EndReceive(arg);
            string sRecieved = Encoding.Unicode.GetString(buffer, 0, nBytesRec);
            Console.WriteLine(sRecieved);
            SetupRecieveCallback(sock);
        }
        static byte[] buffer = new byte[2048];
        public static void SetupRecieveCallback(Socket sock)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, recieveData, sock);
            }
            catch (Exception) { }
        }
    }
    
}
