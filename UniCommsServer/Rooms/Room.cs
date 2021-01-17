using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCommsServer.Rooms
{
    class Room
    {
        public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        public int owner;

        public List<int> users;
        public int id;
        public string name;
        public static int currentFreeId;
        public string password;
        public Room(int owner, string name, string password)
        {
            this.owner = owner;
            this.name = name;
            this.password = password;
            users = new List<int>();
            id = currentFreeId;
            currentFreeId++;
            rooms.Add(id, this);
        }

        public void Delete()
        {
            rooms.Remove(id);
        }

        public void Join(int id)
        {
            users.Add(id);
            Server.Server.Alert($"{Server.Server.clients[id].user.username} joined room {id}");
        }

        public void Disconnect(int id)
        {
            users.Remove(id);
        }
    }
}
