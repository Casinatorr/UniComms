using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UniCommsServer.User
{
    [Serializable]
    class User:ISerializable
    {
        public int id;
        public string username;
        public string password;
        public static int currentFreeId;

        public static Dictionary<int, User> users = new Dictionary<int, User>();
        public static Dictionary<int, User> changedUsers = new Dictionary<int, User>();
        public List<User> recognizedChange = new List<User>();
        public User(string _username, string _password)
        {
            id = currentFreeId;
            username = _username;
            password = _password;
            currentFreeId++;
            users.Add(id, this);
        }


        public static User byUsername(string username)
        {
            foreach (KeyValuePair<int, User> kv in users)
            {
                if (kv.Value.username.Equals(username))
                    return kv.Value;
            }
            return null;
        }

        public static void Save()
        {
            Stream stream = File.Open("Users.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, users.Values.ToList());
            stream.Close();
        }

        public static void Load()
        {
            try
            {
                users.Clear();
                Stream stream = File.Open("Users.dat", FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Deserialize(stream);
                Console.WriteLine(users.Count);
            }
            catch
            {
                Server.Server.Alert("There is no users file to load");
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", id);
            info.AddValue("username", username);
            info.AddValue("password", password);
        }

        public User(SerializationInfo info, StreamingContext context)
        {
            id = info.GetInt32("id");
            username = info.GetString("username");
            password = info.GetString("password");
            users.Add(id, this);
        }
    }
}
