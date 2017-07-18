using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileGameServer.Server;

namespace MobileGameServer
{

    class Program
    {
        static void Main(string[] args)
        {
            Server.Server.Init();
            while (true)
            {
                Server.Server.Update();
                //Console.ReadLine();
            }
        }
    }
}
