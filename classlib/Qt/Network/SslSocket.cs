using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QSslSocket> $q;"
    )]
    public class SslSocket : TcpSocket {
        public SslSocket() {
            CPP.Add("$q = std::make_shared<QSslSocket>();");
            CPP.Add("TcpSocket::$base($q);");
        }
        [CPPReplaceArgs("QSslSocket *$s")]
        private SslSocket(NativeArg1 arg) {
            CPP.Add("$q.reset($s); TcpSocket::$base($q);");
        }
        public new void Connect(String host, int port) {CPP.Add("$q->connectToHostEncrypted($check(host)->qstring(), port);");}
    }
}
