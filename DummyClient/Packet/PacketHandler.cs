using DummyClient;
using ServerCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    // {패킷이름}Handler 요청하면 됩니다 라고 약속
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerId == 1)
        //{
            Console.WriteLine(chatPacket.chat);
        //}
    }
}