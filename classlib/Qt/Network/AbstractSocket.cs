using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass("QAbstractSocket* $d() {return dynamic_cast<QAbstractSocket*>($q.get());}")]
    public class AbstractSocket : IOStream {
        public void Abort() {CPP.Add("$d()->abort();");}
        public void Bind(int port = 0) {CPP.Add("$d()->bind(port);");}
        public void Connect(String host, int port) {CPP.Add("$d()->connectToHost($check(host)->qstring(), port);");}
        public void Disconnect() {CPP.Add("$d()->disconnectFromHost();");}
        public bool WaitForConnected(int msec = 30000) {return CPP.ReturnBool("$d()->waitForConnected(msec)");}
        public bool WaitForDisconnected(int msec = 30000) {return CPP.ReturnBool("$d()->waitForDisconnected(msec)");}
    }
}
