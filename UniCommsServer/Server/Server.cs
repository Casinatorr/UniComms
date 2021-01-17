using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UniCommsServer.Server
{
    class Server
    {
        public static int Port;
        public static int MaxConnections;

        private static TcpListener tcpListener;

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int from, Packet p);
        public static Dictionary<int, PacketHandler> packetHandlers;


        public static void Start(int _port, int _maxConnections)
        {
            Port = _port;
            MaxConnections = _maxConnections;
            Alert("Starting Server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            Program.PingThread.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Alert($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for(int i = 1; i <= MaxConnections; i++)
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    return;
                }

            Alert($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for(int i = 1; i <= MaxConnections; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.login, ServerHandle.HandleLogin },
                {(int)ClientPackets.knownUsers, ServerHandle.HandleKnownUsers },
                {(int)ClientPackets.joinRoom, ServerHandle.HandleJoinRoom },
                {(int)ClientPackets.createRoom, ServerHandle.HandleCreateRoom }
            };
        }

        public static void Alert(string msg)
        {
            Console.WriteLine(Time + msg);
        }

        private static string Time {
            get {
                DateTime n = DateTime.Now;
                return $"({n.ToString("HH:mm:ss")}) ";
            }
        }
    }
}
