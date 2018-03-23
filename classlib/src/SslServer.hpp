namespace Qt { namespace Network {

struct $QSslServer : public QTcpServer {
  void incomingConnection(qintptr socket) {
    QSslSocket *sslsocket = new QSslSocket();
    sslsocket->setSocketDescriptor(socket);
    addPendingConnection(sslsocket);
    sslsocket->startServerEncryption();
  }
};

} } //namespace Qt::Network
