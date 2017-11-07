using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: QUdpSocket *$q;\r\n"
    )]
    public class UdpSocket : AbstractSocket {
        public UdpSocket() {
            CPP.Add("$q = new QUdpSocket();\r\n");
            CPP.Add("$base($q);\r\n");
        }
    }
}
