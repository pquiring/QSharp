using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "std::shared_ptr<QTcpSocket> $q;" +
        "void $base(std::shared_ptr<QTcpSocket> as) {$q = as; AbstractSocket::$base((std::shared_ptr<QAbstractSocket>)as);}"
    )]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$q = std::make_shared<QTcpSocket>();");
            CPP.Add("AbstractSocket::$base((std::shared_ptr<QAbstractSocket>)$q);");
        }
        [CPPReplaceArgs("QTcpSocket *$s")]
        private TcpSocket(NativeArg1 arg) {
            CPP.Add("$q.reset($s); AbstractSocket::$base($q);");
        }
    }
}
