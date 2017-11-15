using Qt.QSharp;

namespace Qt.Network {
    public delegate void Pending(TcpServer server);
    [CPPExtends("QTcpServer")]
    public class TcpServer {
        private Pending pending;
        private void NewConnection() {
            if (pending != null) {
                pending(this);
            }
        }
        public bool Listen(int port) {
            return CPP.ReturnBool("listen(QHostAddress::Any, port)");
        }
        /** Accepts TcpSocket.  Must be used in same thread. */
        public TcpSocket Accept() {
            CPP.Add("std::shared_ptr<TcpSocket> socket;");
            CPP.Add("socket.reset(new TcpSocket(nextPendingConnection()));");
            return (TcpSocket)CPP.ReturnObject("socket");
        }
        public bool Pending() {
            return CPP.ReturnBool("hasPendingConnections()");
        }
        public void Close() {
            CPP.Add("close();");
        }
        /** Called pending delegate when a new connection arrives. */
        public void OnPending(Pending pending) {
            this.pending = pending;
            CPP.Add("connect(this, &QTcpServer::newConnection, this, &TcpServer::NewConnection);");
        }
    }
}
