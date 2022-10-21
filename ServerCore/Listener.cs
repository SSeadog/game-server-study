using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    class Listener
    {
        private Socket _socket;

        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;


            // 문지기 교육
            _socket.Bind(endPoint);

            // 영업 시작
            _socket.Listen(10); // 10을 초과하면 그 이후 요청은 Fail을 띄움

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegistAccept(args);
        }

        public void RegistAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _socket.AcceptAsync(args);
            if (pending == false)
            {
                OnAcceptCompleted(null, args);
            }
        }

        // 메인 쓰레드에서 사용중인 데이터를 수정하면 RaceCondition위험 있음
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                // Todo
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegistAccept(args);
        }

        public Socket Accept()
        {
            return _socket.Accept();
        }
    }
}
