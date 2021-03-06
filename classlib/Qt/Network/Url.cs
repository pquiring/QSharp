using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "std::qt_ptr<QUrl> $q;" +
        "QUrl *$value() {return $q.get();}"
    )]
    public class Url {
        public Url(String url) {
            CPP.Add("$q = new QUrl($check(url)->qstring());");
        }
        public String GetQuery() {
            return CPP.ReturnString("new Qt::Core::String($q->query())");
        }
        public void SetQuery(String query) {
            if (query == null)
                CPP.Add("$q->setQuery(QString())");
            else
                CPP.Add("$q->setQuery(query->qstring())");
        }
        public String GetHost() {
            return CPP.ReturnString("new Qt::Core::String($q->host())");
        }
        public void SetHost(String host) {
            CPP.Add("$q->setHost($check(host)->qstring())");
        }
        public String GetProtocol() {
            return CPP.ReturnString("new Qt::Core::String($q->scheme())");
        }
        public void SetProtcol(String protocol) {
            CPP.Add("$q->setScheme($check(protocol)->qstring())");
        }
        public int GetPort() {
            return CPP.ReturnInt("$q->port()");
        }
        public void SetPort(int port) {
            CPP.Add("$q->setPort(port);");
        }
    }
}
