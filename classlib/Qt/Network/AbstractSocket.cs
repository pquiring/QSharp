using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: QAbstractSocket *$q;\r\n" +
        "public: void $base(QAbstractSocket *socket) {$q = socket; IODevice::$base(socket);}\r\n"
    )]
    public class AbstractSocket : IODevice {
        public void Abort() {CPP.Add("$q->abort();");}
        public void Connect(String host, int port) {CPP.Add("$q->connectToHost(host->qstring(), port);");}
        public void Disconnect() {CPP.Add("$q->disconnectFromHost();");}
        public bool WaitForConnected(int msec = 30000) {return CPP.ReturnBool("$q->waitForConnected(msec)");}
        public bool WaitForDisconnected(int msec = 30000) {return CPP.ReturnBool("$q->waitForDisconnected(msec)");}
    }
}
