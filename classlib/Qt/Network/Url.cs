using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QUrl> $q;"
    )]
    public class Url {
        public Url(String url) {
            CPP.Add("$q = std::make_shared<QUrl>(url->qstring());");
        }
        public String GetQuery() {
            return CPP.ReturnString("String::$new($q->query())");
        }
        public void SetQuery(String query) {
            if (query == null)
                CPP.Add("$q->setQuery(QString())");
            else
                CPP.Add("$q->setQuery(query->qstring())");
        }
        public String GetHost() {
            return CPP.ReturnString("String::$new($q->host())");
        }
        public void SetHost(String host) {
            CPP.Add("$q->setHost(host->qstring())");
        }
        public String GetProtocol() {
            return CPP.ReturnString("String::$new($q->scheme())");
        }
        public void SetProtcol(String protocol) {
            CPP.Add("$q->setScheme(protocol->qstring())");
        }
        public int GetPort() {
            return CPP.ReturnInt("$q->port()");
        }
        public void SetPort(int port) {
            CPP.Add("$q->setPort(port);");
        }
    }
}
