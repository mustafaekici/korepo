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

            var t = new ServerCore.TextServer(ServerCore.Transmission.MultiCast);
            var res = t.StartServer(1232,9);
            Console.WriteLine(res);
            Console.WriteLine("exit:");

            Console.ReadKey();
        }
    }
}
