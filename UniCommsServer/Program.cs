using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCommsServer.Server;
using System.Threading;

namespace UniCommsServer
{
    class Program
    {
        public static Thread PingThread;
        public static int pingMilliseconds = 1500;
        static void Main(string[] args)
        {
            PingThread = new Thread(new ThreadStart(Ping));
            Server.Server.Start(26950, 10);
            Console.ReadLine();
        }

        private static void Ping()
        {
            for (int i = 1; i <= Server.Server.MaxConnections; i++)
            {
                using (Packet p = new Packet((int) ServerPackets.ping))
                {
                    p.Write("Ping");
                    Server.Server.clients[i].tcp.SendData(p);
                }
            }
            Thread.Sleep(pingMilliseconds);
        }
    }
}
