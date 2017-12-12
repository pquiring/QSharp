using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "private: std::unique_ptr<QNetworkAccessManager> $q;"
    )]
    public class WebClient {
        public WebClient() {
            CPP.Add("$q = std::make_unique<QNetworkAccessManager>();");
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
                case WebMethod.Get: return (WebReply)CPP.ReturnObject("WebReply::$new($q->get(*request->$value()))");
                case WebMethod.Post: return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*request->$value(), *request->GetData()->$value()))");
                case WebMethod.Put: return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*request->$value(), *request->GetData()->$value()))");
                case WebMethod.Head: return (WebReply)CPP.ReturnObject("WebReply::$new($q->head(*request->$value()))");
                case WebMethod.Delete: return (WebReply)CPP.ReturnObject("WebReply::$new($q->deleteResource(*request->$value()))");
            }
            return null;
        }
        public WebReply Get(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->get(*request->$value()))");
        }
        public WebReply Post(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*request->$value(), *request->GetData()->$value()))");
        }
        public WebReply Post(WebRequest request, IOStream io) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*request->$value(), io->$q.get()))");
        }
        public WebReply Post(WebRequest request, ByteArray array) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*request->$value(), *array->$value()))");
        }
        public WebReply Put(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*request->$value(), *request->GetData()->$value()))");
        }
        public WebReply Put(WebRequest request, IOStream io) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*request->$value(), io->$q.get()))");
        }
        public WebReply Put(WebRequest request, ByteArray array) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*request->$value(), *array->$value()))");
        }
        public WebReply Delete(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->deleteResource(*request->$value()))");
        }
        public WebReply Head(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->head(*request->$value()))");
        }
    }
}
