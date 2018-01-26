namespace Qt::Network {

class $QSslServer : public QTcpServer {
  public: void incomingConnection(qintptr socket) {
    QSslSocket *sslsocket = new QSslSocket();
    sslsocket->setSocketDescriptor(socket);
    addPendingConnection(sslsocket);
    sslsocket->startServerEncryption();
  }
};

}
