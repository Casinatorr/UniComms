using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCommsServer.Rooms;

namespace UniCommsServer.Server
{
    static class ServerHandle
    {
        public static void HandleLogin(int fromClient, Packet p)
        {
            bool register = p.ReadBool();
            string username = p.ReadString();
            string password = p.ReadString();
            Server.Alert($"Incoming login... ({username}, {password}, {register})");
            if (register)
            {
                User.User newUser = new User.User(username, password);
                Server.clients[fromClient].user = newUser;
            }
            User.User u = User.User.byUsername(username);
            if (u == null)
            {
                ServerSend.SendLogin(fromClient, register, false, false);
                Server.Alert($"{username} is stupid af");
            } else if (u.password == password)
            {
                Server.clients[fromClient].user = u;
                ServerSend.SendLogin(fromClient, register, true, true);
                Server.Alert($"{username} logged in!");
            } else
            {
                ServerSend.SendLogin(fromClient, register, true, false);
                Server.Alert($"{username} chose wrong password!");
            }
        }

        public static void HandleKnownUsers(int fromClient, Packet p)
        {
            int length = p.ReadInt();
            List<User.User> toSend = User.User.users.Values.ToList();
            User.User u = Server.clients[fromClient].user;
            for (int i = 0; i < length; i++)
            {
                int id = p.ReadInt();
                User.User current = User.User.users[id];
                if (User.User.changedUsers.ContainsKey(id))
                {
                    if (current.recognizedChange.Contains(u))
                    {
                        toSend.Remove(current);
                        continue;
                    }
                    current.recognizedChange.Add(u);
                } else
                {
                    toSend.Remove(current);
                }
            }

            ServerSend.SendUsers(fromClient, toSend);
        }

        public static void HandleCreateRoom(int fromClient, Packet p)
        {
            string name = p.ReadString();
            string password = p.ReadString();
            Room room = new Room(fromClient, name, password);
            room.Join(fromClient);
        }

        public static void HandleJoinRoom(int fromClient, Packet p) 
        {
            int id = p.ReadInt();
            string password = p.ReadString();
            if (!Room.rooms.ContainsKey(id))
            {
                ServerSend.AnswerRoomLogin(fromClient, false, false, null);
                return;
            }
            Room room = Room.rooms[id];
            if (room.password != password)
            {
                ServerSend.AnswerRoomLogin(fromClient, true, false, null);
                return;
            }
            room.Join(fromClient);
            ServerSend.AnswerRoomLogin(fromClient, true, true, room.users);
        }

        private enum Chat
        {
            PUBLIC,
            PRIVATE
        }
    }
}
