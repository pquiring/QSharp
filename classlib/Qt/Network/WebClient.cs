using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QNetworkAccessManager> $q;"
    )]
    public class WebClient {
        public WebClient() {
            CPP.Add("$q = std::make_shared<QNetworkAccessManager>();");
        }
        public void Connect(String host, int port = 80, bool secure = false) {
            if (!secure) {
                CPP.Add("$q->connectToHost(host->qstring(), port);");
            } else {
                CPP.Add("$q->connectToHostEncrypted(host->qstring(), port);");
            }
        }
        public WebReply Process(WebRequest request) {
            switch (request.GetMethod()) {
                case WebMethod.Get: return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->get(*request->$q));");
                case WebMethod.Post: return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->post(*request->$q, *request->GetData()->$q));");
                case WebMethod.Put: return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->put(*request->$q, *request->GetData()->$q));");
                case WebMethod.Head: return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->head(*request->$q));");
                case WebMethod.Delete: return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->deleteResource(*request->$q));");
            }
            return null;
        }
        public WebReply Get(WebRequest request) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->get(*request->$q));");
        }
        public WebReply Post(WebRequest request) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->post(*request->$q, *request->GetData()->$q));");
        }
        public WebReply Post(WebRequest request, IOStream io) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->post(*request->$q, io->$q.get()));");
        }
        public WebReply Post(WebRequest request, ByteArray array) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->post(*request->$q, *array->$q));");
        }
        public WebReply Put(WebRequest request) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->put(*request->$q, *request->GetData()->$q));");
        }
        public WebReply Put(WebRequest request, IOStream io) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->put(*request->$q, io->$q.get()));");
        }
        public WebReply Put(WebRequest request, ByteArray array) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->put(*request->$q, *array->$q));");
        }
        public WebReply Delete(WebRequest request) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->deleteResource(*request->$q));");
        }
        public WebReply Head(WebRequest request) {
            return (WebReply)CPP.ReturnObject("std::make_shared<WebReply>($q->head(*request->$q));");
        }
    }
}
