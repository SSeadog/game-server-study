using System;
using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient
{
    // 패킷의 헤더 정보로 보면 됨
    abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    // 플레이어 인포 요청할 때 사용할 패킷
    class PlayerInfoReq : Packet
    {
        public long playerId;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> s)
        {
            ushort count = 0;

            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            count += 2;
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + count);
            count += 2;
            BitConverter.ToUInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            count += 8;
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> s = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            // 효율을 위해 GetBytes 대신 TryWriteBytes 사용
            // count를 먼저 더한 이유는 TyrWriteBytes에서 해당 비트만큼 쓸거니까 계산하기 위해 먼저 더하기?
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.packetId);
            count += 2;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset + count, s.Count - count), this.playerId);
            count += 8;
            success &= BitConverter.TryWriteBytes(new Span<byte>(s.Array, s.Offset, count), (ushort)4);

            //byte[] size = BitConverter.GetBytes(packet.size); // 2
            //byte[] packetId = BitConverter.GetBytes(packet.packetId); // 2
            //byte[] playerId = BitConverter.GetBytes(packet.playerId); // 8

            //Array.Copy(size, 0, s.Array, s.Offset + count, 2);
            //count += 2;
            //Array.Copy(packetId, 0, s.Array, s.Offset + count, 2);
            //count += 2;
            //Array.Copy(playerId, 0, s.Array, s.Offset + count, 8);
            //count += 8;

            if (success == false)
            {
                return null;
            }

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    class ServerSession : Session
    {



        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"Onconnected: {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001 };

            // 보낸다
            //for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> s = packet.Write();
                if (s != null)
                    Send(s);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        // 패킷을 받으면 어떻게 처리하겠다는 규약을 정해야함
        // 패킷은 일부만 받으면 수행x
        // 이동 패킷 (특정 좌표로 이동하고 싶다)
        // 15 3 2
        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}

