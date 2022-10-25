using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    // {패킷이름}Handler 요청하면 됩니다 라고 약속
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        C_Chat chatPacket = packet as C_Chat;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
        {
            return;
        }

        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Broadcast(clientSession, chatPacket.chat)
        );
    }
}