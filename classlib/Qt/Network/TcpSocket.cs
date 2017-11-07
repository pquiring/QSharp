using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass(
        "private: QTcpSocket *$q;\r\n"
    )]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$q = new QTcpSocket();\r\n");
            CPP.Add("$base($q);\r\n");
        }
    }
}
