using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingSslEvent(SslServer server);
    [CPPClass("$QSslServer* $d() {return dynamic_cast<$QSslServer*>($q.get());}")]
    public class SslServer : TcpServer {
        public SslServer() {
            CPP.Add("TcpServer::$base(new $QSslServer());");
        }
        private PendingSslEvent pending;
        public new SslSocket Accept() {
            return (SslSocket)CPP.ReturnObject("new SslSocket((QSslSocket*)$d()->nextPendingConnection())");
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
            CPP.Add("QObject::connect($d(), &QTcpServer::newConnection, [=] () {this->SlotNewConnection();});");
        }
    }
}
