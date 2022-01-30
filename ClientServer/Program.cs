using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 8888, number: 2);
            _ = server.ListenAsync();
            Console.ReadKey();
        }
    }
}