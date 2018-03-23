namespace Qt { namespace Network {

struct $QWebReply : public QNetworkReply {
  qint64 readData(char *data, qint64 maxlen) {return 0;}
  void abort() {}
};

} } //namespace Qt::Network
