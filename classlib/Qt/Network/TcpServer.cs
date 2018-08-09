using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingTcpEvent(TcpServer server);
    [CPPClass(
        "std::qt_ptr<QTcpServer> $q;" +
        "void $base(QTcpServer* $b) {$q = $b;}"
    )]
    public class TcpServer {
        public TcpServer() {
            CPP.Add("$q = new QTcpServer();");
        }
        private PendingTcpEvent pending;
        private void SlotNewConnection() {
            try {
                if (pending != null) {
                    pending(this);
                }
            } catch {}
        }
        /** Starts listening on port. */
        public bool Listen(int port) {
            return CPP.ReturnBool("$q->listen(QHostAddress::Any, port)");
        }
        /** Accepts TcpSocket.  Must be used in same thread. */
        public TcpSocket Accept() {
            return (TcpSocket)CPP.ReturnObject("new TcpSocket($q->nextPendingConnection())");
        }
        /** Returns status of pending connection. */
        public bool IsPending() {
            return CPP.ReturnBool("$q->hasPendingConnections()");
        }
        /** Closes TCP socket server. */
        public void Close() {
            CPP.Add("$q->close();");
        }
        /** Calls pending delegate when a new connection arrives. */
        public void OnPending(PendingTcpEvent pending) {
            this.pending = pending;
            CPP.Add("QObject::connect($q.get(), &QTcpServer::newConnection, [=] () {this->SlotNewConnection();});");
        }
    }
}
