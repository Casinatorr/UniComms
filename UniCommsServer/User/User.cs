using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCommsServer.User
{
    class User
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
            foreach(KeyValuePair<int, User> kv in users)
            {
                if (kv.Value.username.Equals(username))
                    return kv.Value;
            }
            return null;
        }

    }
}
