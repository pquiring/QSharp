using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QTcpSocket> $q;" +
        "public: TcpSocket() {}" +
        "public: TcpSocket(QTcpSocket *$s) {$q.reset($s); AbstractSocket::$base($q);}" +
        "public: void $base(std::shared_ptr<QTcpSocket> as) {$q = as; AbstractSocket::$base((std::shared_ptr<QAbstractSocket>)as);}"
    )]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$q = std::make_shared<QTcpSocket>();");
            CPP.Add("AbstractSocket::$base($q);");
        }
    }
}
