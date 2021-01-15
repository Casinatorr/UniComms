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
        public static bool shouldPing = true;
        public static int backupMinutes = 7;
        private delegate void Command();
        private static Dictionary<string, Command> commands;
        static void Main(string[] args)
        {
            PingThread = new Thread(new ThreadStart(Ping));
            Thread BackupThread = new Thread(new ThreadStart(Backup));
            BackupThread.Start();
            Server.Server.Start(26950, 10);
            InitializeCommands();
            User.User.Load();
            while (true)
            {
                string input = Console.ReadLine();
                if (commands.ContainsKey(input))
                    commands[input]();
                if (input == "stop")
                    break;
            }
            PingThread.Abort();
            BackupThread.Abort();
        }

        private static void Ping()
        {
            while (true)
            {
                if (shouldPing)
                {
                    for (int i = 1; i <= Server.Server.MaxConnections; i++)
                    {
                        using (Packet p = new Packet((int) ServerPackets.ping))
                        {
                            p.Write("Ping");
                            p.WriteLength();
                            Server.Server.clients[i].tcp.SendData(p);
                        }
                    }
                }
                Thread.Sleep(pingMilliseconds);
            }
        }


        //calls backup every 7 minutes
        private static void Backup()
        {
            while (true)
            {
                Thread.Sleep(backupMinutes * 60000);
                directBackup();
            }
        }

        //backups without delay
        private static void directBackup()
        {
            Server.Server.Alert($"Starting Backup...");
            DateTime start = DateTime.Now;
            User.User.Save();
            Server.Server.Alert($"Backup done in {DateTime.Now.Subtract(start).TotalMilliseconds} milliseconds!");
        }

        private static void InitializeCommands()
        {
            commands = new Dictionary<string, Command>()
            {
                {"backup", directBackup}
            };
        }
    }
}
