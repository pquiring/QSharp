namespace Qt { namespace Network {

class $QSslServer : public QTcpServer {
  public: void incomingConnection(qintptr socket) {
    QSslSocket *sslsocket = new QSslSocket();
    sslsocket->setSocketDescriptor(socket);
    addPendingConnection(sslsocket);
    sslsocket->startServerEncryption();
  }
};

} } //namespace Qt::Network
