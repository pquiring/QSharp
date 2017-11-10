using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: std::shared_ptr<QUdpSocket> $q;"
    )]
    public class UdpSocket : AbstractSocket {
        public UdpSocket() {
            CPP.Add("$q = std::make_shared<QUdpSocket>();");
            CPP.Add("$base($q);");
        }
    }
}
