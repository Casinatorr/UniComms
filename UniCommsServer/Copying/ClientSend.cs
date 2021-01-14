using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCommsServer.Copying
{
    class ClientSend
    {
        private static void SendTCPData(Packet p)
        {
            p.WriteLength();
            Client.instance.tcp.SendData(p);
        }

        public static void SendLoginData(bool register, string username, string password)
        {
            using (Packet p = new Packet((int) ClientPackets.login))
            {
                p.Write(register);
                p.Write(username);
                p.Write(password);
                SendTCPData(p);
            }
        }

        public static void SendKnownUsers()
        {

        }
    }
}
