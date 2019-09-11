using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketWrapper
{
    public class DnsAdapter : IDns
    {
        [Obsolete]
        public IPHostEntry GetHostByName(string hostName)
        {
            return Dns.GetHostByName(hostName);
        }

        public string GetHostName()
        {
            return Dns.GetHostName();
        }
    }
}
