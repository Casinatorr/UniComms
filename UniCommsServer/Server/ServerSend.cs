using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCommsServer.Server
{
    class ServerSend
    {
        private static void SendTCPData(int toClient, Packet p)
        {
            p.WriteLength();
            Server.clients[toClient].tcp.SendData(p);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxConnections; i++)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet p)
        {
            p.WriteLength();
            for (int i = 1; i <= Server.MaxConnections; i++)
            {
                if (i == exceptClient) continue;
                Server.clients[i].tcp.SendData(p);
            }
        }

        public static void SendLogin(int toClient, bool register, bool username, bool password)
        {
            using (Packet p = new Packet((int) ServerPackets.sendLogin))
            {
                p.Write(register);
                p.Write(username);
                p.Write(password);
                SendTCPData(toClient, p);
                return;
            }
        }

        public static void SendUsers(int toClient, List<User.User> users)
        {
            using(Packet p = new Packet((int) ServerPackets.sendUsers))
            {
                p.Write(users.Count);
                foreach(User.User u in users)
                {
                    p.Write(u.id);
                    p.Write(u.username);
                }
                SendTCPData(toClient, p);
            }
        }

        public static void AnswerRoomLogin(int toClient, bool id, bool password, List<int> members)
        {
            using (Packet p = new Packet((int) ServerPackets.validateRoomLogin))
            {
                p.Write(id);
                p.Write(password);
                if (members == null)
                {
                    SendTCPData(toClient, p);
                    return;
                }
                p.Write(members.Count);

                foreach (int m in members)
                    p.Write(Server.clients[m].user.id);

                SendTCPData(toClient, p);
            }
        }

        public static void AnswerRoomCreate(int toClient, int roomId)
        {
            using (Packet p = new Packet((int) ServerPackets.answerRoomCreate))
            {
                p.Write(roomId);
                SendTCPData(toClient, p);
            }
        }
    }
}
