using SocketWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
         
        static void Main(string[] args)
        {
           
            ConsoleKeyInfo k;
            
            Console.WriteLine("Select Ip address to send message");
            var client = new TextClient();
            client.StartClient("10.0.75.1", 1232); //Todo : get ip and port from console.

            string selectedClientip = Console.ReadLine();
            Console.WriteLine("Conversation started with " + selectedClientip);
          

            while (true)
            {
                Console.Write("You:");
                string message = Console.ReadLine();
                if (selectedClientip == "All")
                {
                    Console.WriteLine("Sending...");
                }
                else
                {
                    client.Sendmessage(selectedClientip + "_" + message);      
                }
            }

        }
     

    }
    
}
