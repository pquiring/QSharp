using Qt.QSharp;

namespace Qt.Network {
    public delegate void PendingLocalEvent(LocalServer server);
    [CPPClass(
        "std::qt_ptr<QLocalServer> $q;" +
        "void $base(QLocalServer* $b) {$q = $b;}"
    )]
    public class LocalServer {
        public LocalServer() {
            CPP.Add("$q = new QLocalServer();");
        }
        private PendingLocalEvent pending;
        private void SlotNewConnection() {
            try {
                if (pending != null) {
                    pending(this);
                }
            } catch {}
        }
        /** Starts listening on port. */
        public bool Listen(string name) {
            return CPP.ReturnBool("$q->listen($check(name)->qstring())");
        }
        /** Accepts LocalSocket.  Must be used in same thread. */
        public LocalSocket Accept() {
            return (LocalSocket)CPP.ReturnObject("new LocalSocket($q->nextPendingConnection())");
        }
        /** Returns status of pending connection. */
        public bool IsPending() {
            return CPP.ReturnBool("$q->hasPendingConnections()");
        }
        /** Closes Local socket server. */
        public void Close() {
            CPP.Add("$q->close();");
        }
        /** Calls pending delegate when a new connection arrives. */
        public void OnPending(PendingLocalEvent pending) {
            this.pending = pending;
            CPP.Add("QObject::connect($q.get(), &QLocalServer::newConnection, [=] () {this->SlotNewConnection();});");
        }
    }
}
