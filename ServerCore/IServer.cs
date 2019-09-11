using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    interface IServer
    {
        string StartServer(int port);
        string StopServer();
    }
}
