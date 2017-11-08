using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: std::shared_ptr<QUdpSocket> $q;\r\n"
    )]
    public class UdpSocket : AbstractSocket {
        public UdpSocket() {
            CPP.Add("$q = std::make_shared<QUdpSocket>();\r\n");
            CPP.Add("$base($q);\r\n");
        }
    }
}
