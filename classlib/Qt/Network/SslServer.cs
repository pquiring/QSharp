using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingSslEvent(SslServer server);
    [CPPClass(
        "std::shared_ptr<$QSslServer> $q;"
    )]
    public class SslServer : TcpServer {
        public SslServer() {
            CPP.Add("$q = std::make_shared<$QSslServer>();");
            CPP.Add("TcpServer::$base((std::shared_ptr<QTcpServer>)$q);");
        }
        private PendingSslEvent pending;
        public new SslSocket Accept() {
            return (SslSocket)CPP.ReturnObject("SslSocket::$new((QSslSocket*)$q->nextPendingConnection())");
        }
        private void SlotNewConnection() {
            try {
                if (pending != null) {
                    pending(this);
                }
            } catch {}
        }
        /** Calls pending delegate when a new connection arrives. */
        public void OnPending(PendingSslEvent pending) {
            this.pending = pending;
            CPP.Add("QObject::connect($q.get(), &QTcpServer::newConnection, [=] () {this->SlotNewConnection();});");
        }
    }
}
