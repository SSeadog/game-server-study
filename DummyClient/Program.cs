using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{ 
    class Program
    {
        static void Main(string[] args)
        {
            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = null;
            foreach (IPAddress ip in ipHost.AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetworkV6 & !ip.Equals(IPAddress.Parse("127.0.0.1")))
                {
                    ipAddr = ip;
                    break;
                }
            }
            Console.WriteLine(ipAddr);
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            connector.Connect(endPoint,
                () => { return SessionManager.Instance.Generate(); },
                50);

            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(250);
            }
        }
    }
}