using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingEvent(TcpServer server);
    [CPPExtends("QTcpServer")]
    public class TcpServer {
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
            return CPP.ReturnBool("listen(QHostAddress::Any, port)");
        }
        /** Accepts TcpSocket.  Must be used in same thread. */
        public TcpSocket Accept() {
            return (TcpSocket)CPP.ReturnObject("TcpSocket::$new(nextPendingConnection())");
        }
        /** Returns status of pending connection. */
        public bool IsPending() {
            return CPP.ReturnBool("hasPendingConnections()");
        }
        /** Closes TCP socket server. */
        public void Close() {
            CPP.Add("close();");
        }
        /** Calls pending delegate when a new connection arrives. */
        public void OnPending(PendingEvent pending) {
            this.pending = pending;
            CPP.Add("connect(this, &QTcpServer::newConnection, this, &TcpServer::SlotNewConnection);");
        }
    }
}
