using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCommsServer.Copying
{
    class ClientHandle
    {
        public static void HandleLogin(Packet p)
        {
            bool username = p.ReadBool();
            bool password = p.ReadBool();
            Console.WriteLine($"Username: {username}, Password: {password}");
        }

        public static void HandleKnownUsers(Packet p)
        {

        }


    }
}
