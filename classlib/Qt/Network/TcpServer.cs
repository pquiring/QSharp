using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingEvent(TcpServer server);
    [CPPClass(
        "private: std::shared_ptr<QTcpServer> $q;" +
        "public: void $base(std::shared_ptr<QTcpServer> $b) {$q = $b;}"
    )]
    public class TcpServer {
        public TcpServer() {
            CPP.Add("$q = std::make_shared<QTcpServer>();");
        }
        private PendingEvent pending;
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
            return (TcpSocket)CPP.ReturnObject("TcpSocket::$new($q->nextPendingConnection())");
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
        public void OnPending(PendingEvent pending) {
            this.pending = pending;
            CPP.Add("QObject::connect($q.get(), &QTcpServer::newConnection, [=] () {this->SlotNewConnection();});");
        }
    }
}
