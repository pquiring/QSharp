using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QTcpSocket> $q;" +
        "public: TcpSocket() {}" +
        "public: TcpSocket(QTcpSocket *$s) {$q.reset($s); $base($q);}"
    )]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$q = std::make_shared<QTcpSocket>();");
            CPP.Add("$base($q);");
        }
    }
}
