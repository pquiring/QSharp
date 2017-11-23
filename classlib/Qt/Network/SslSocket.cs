using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QSslSocket> $q;" +
        "public: SslSocket() {}" +
        "public: SslSocket(QSslSocket *$s) {$q.reset($s); TcpSocket::$base($q);}"
    )]
    public class SslSocket : TcpSocket {
        public SslSocket() {
            CPP.Add("$q = std::make_shared<QSslSocket>();");
            CPP.Add("TcpSocket::$base($q);");
        }
        public new void Connect(String host, int port) {CPP.Add("$q->connectToHostEncrypted(host->qstring(), port);");}
    }
}
