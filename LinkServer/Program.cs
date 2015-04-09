using System;
using System.Collections.Generic;

namespace LinkServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LinkServer server = new LinkServer();

            server.Initiate();
        }
    }
}
