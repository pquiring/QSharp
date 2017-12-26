using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    public delegate void SslPendingEvent(SslServer server);
    [CPPClass(
        "public: void incomingConnection(qintptr socket) {" +
        "  QSslSocket *sslsocket = new QSslSocket();" +
        "  sslsocket->setSocketDescriptor(socket);" +
        "  addPendingConnection(sslsocket);" +
        "  sslsocket->startServerEncryption();" +
        "}"
    )]
    public class SslServer : TcpServer {
        private SslPendingEvent pending;
        public new SslSocket Accept() {
            return (SslSocket)CPP.ReturnObject("SslSocket::$new((QSslSocket*)nextPendingConnection())");
        }
        private void SlotNewConnection() {
            try {
                if (pending != null) {
                    pending(this);
                }
            } catch {}
        }
        /** Calls pending delegate when a new connection arrives. */
        public void OnPending(SslPendingEvent pending) {
            this.pending = pending;
            CPP.Add("connect(this, &QTcpServer::newConnection, this, &SslServer::SlotNewConnection);");
        }
    }
}
