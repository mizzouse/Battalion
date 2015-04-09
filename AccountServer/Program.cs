using System;
using System.Collections.Generic;

namespace AccountServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AccountServer server = new AccountServer();

            server.Initiate();
        }
    }
}
