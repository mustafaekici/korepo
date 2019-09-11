using ClientConnection;
using System;

namespace Client
{
    class Program
    {
         
        static void Main(string[] args)
        {

            Console.Write("Enter server ip:");
            var serverip = Console.ReadLine();
            Console.Write("Enter port number:");
            var port = Console.ReadLine();

            Console.WriteLine("Select Ip address to send message");
            var client = new TextClient();
            client.StartClient(serverip, int.Parse(port)); 
            client.OnMessageReceived += Client_OnMessageReceived;

            string selectedClientip = Console.ReadLine();
            Console.WriteLine("Conversation started with " + selectedClientip);
          
            while (true)
            {
              
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

        private static void Client_OnMessageReceived(string message)
        {
            Console.WriteLine(message);
        }
    }
    
}
