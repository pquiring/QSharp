using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingWSEvent(WebSocketServer server);
    [CPPClass(
        "std::shared_ptr<QWebSocketServer> $q;" +
        "void $base(std::shared_ptr<QWebSocketServer> $b) {$q = $b;}"
    )]
    public class WebSocketServer {
        public WebSocketServer(String name, bool secure) {
            CPP.Add("$q = std::make_shared<QWebSocketServer>($check(name)->qstring(), (QWebSocketServer::SslMode)secure);");
        }
        private PendingWSEvent pending;
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
        /** Accepts WebSocket.  Must be used in same thread. */
        public WebSocket Accept() {
            return (WebSocket)CPP.ReturnObject("WebSocket::$new($q->nextPendingConnection())");
        }
        /** Returns status of pending connection. */
        public bool IsPending() {
            return CPP.ReturnBool("$q->hasPendingConnections()");
        }
        /** Closes Web socket server. */
        public void Close() {
            CPP.Add("$q->close();");
        }
        /** Calls pending delegate when a new connection arrives. */
        public void OnPending(PendingWSEvent pending) {
            this.pending = pending;
            CPP.Add("QObject::connect($q.get(), &QWebSocketServer::newConnection, [=] () {this->SlotNewConnection();});");
        }
    }
}
