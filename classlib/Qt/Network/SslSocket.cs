using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass("QSslSocket* $d() {return dynamic_cast<QSslSocket*>($q.get());}")]
    public class SslSocket : TcpSocket {
        public SslSocket() {
            CPP.Add("$base(new QSslSocket());");
        }
        [CPPReplaceArgs("QSslSocket *$s")]
        private SslSocket(NativeArg1 arg) {
            CPP.Add("$base($s);");
        }
        public new void Connect(String host, int port) {CPP.Add("$d()->connectToHostEncrypted($check(host)->qstring(), port);");}
    }
}
