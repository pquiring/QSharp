using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: std::shared_ptr<QTcpSocket> $q;"
    )]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$q = std::make_shared<QTcpSocket>();");
            CPP.Add("$base($q);");
        }
    }
}
