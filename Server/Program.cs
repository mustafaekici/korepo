using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter port number:");
            var port = Console.ReadLine();

            var t = new ServerCore.TextServer();
            var res = t.StartServer(int.Parse(port));
            Console.WriteLine(res);
            Console.WriteLine("Press any key to exit");

            Console.ReadKey();
        }
    }
}
