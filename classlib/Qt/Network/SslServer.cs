using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "public: void incomingConnection(qintptr socket) {" +
        "  QSslSocket *sslsocket = new QSslSocket();" +
        "  sslsocket->setSocketDescriptor(socket);" +
        "  addPendingConnection(sslsocket);" +
        "  sslsocket->startServerEncryption();" +
        "}"
    )]
    public class SslServer : TcpServer {
        public new SslSocket Accept() {
            return (SslSocket)base.Accept();
        }
    }
}
