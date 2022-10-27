using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    void Start()
    {
        // DNS
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = null;
        foreach (IPAddress ip in ipHost.AddressList)
        {
            if (ip.AddressFamily != AddressFamily.InterNetworkV6 & !ip.Equals(IPAddress.Parse("127.0.0.1")))
            {
                Debug.Log(ip.Equals(new byte[4] {127, 0, 0, 1}));
                ipAddr = ipHost.AddressList[0];
                break;
            }
        }
        Debug.Log(ipAddr);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint,
            () => { return _session; },
            1);

        Debug.Log(connector.ToString());
    }

    void Update()
    {
        List<IPacket> list = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in list)
        {
            PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

}
