using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: std::shared_ptr<QTcpSocket> $q;\r\n"
    )]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$q = std::make_shared<QTcpSocket>();\r\n");
            CPP.Add("$base($q);\r\n");
        }
    }
}
