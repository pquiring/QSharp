using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "std::shared_ptr<QAbstractSocket> $q;" +
        "void $base(std::shared_ptr<QAbstractSocket> as) {$q = as; IOStream::$base((std::shared_ptr<QIODevice>)as);}"
    )]
    public class AbstractSocket : IOStream {
        public void Abort() {CPP.Add("$q->abort();");}
        public void Bind(int port = 0) {CPP.Add("$q->bind(port);");}
        public void Connect(String host, int port) {CPP.Add("$q->connectToHost($check(host)->qstring(), port);");}
        public void Disconnect() {CPP.Add("$q->disconnectFromHost();");}
        public bool WaitForConnected(int msec = 30000) {return CPP.ReturnBool("$q->waitForConnected(msec)");}
        public bool WaitForDisconnected(int msec = 30000) {return CPP.ReturnBool("$q->waitForDisconnected(msec)");}
    }
}
