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
            var client = new TextClient();
            client.StartClient();
            client.Sendmessage(Console.ReadLine());
            Console.ReadKey();
        }
     

    }
    
}
