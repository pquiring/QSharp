namespace Qt { namespace Network {

class $QWebReply : public QNetworkReply {
  public: qint64 readData(char *data, qint64 maxlen) {return 0;}
  public: void abort() {}
};

} } //namespace Qt::Network
