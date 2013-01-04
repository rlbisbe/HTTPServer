using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer server = new WebServer("127.0.0.1", 5000);
            server.Start();
            server.Listen();
        }
    }
}

