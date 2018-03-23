using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "std::unique_ptr<QNetworkAccessManager> $q;"
    )]
    public class WebClient {
        public WebClient() {
            CPP.Add("$q = std::make_unique<QNetworkAccessManager>();");
        }
        public void Connect(String host, int port = 80, bool secure = false) {
            if (!secure) {
                CPP.Add("$q->connectToHost($check(host)->qstring(), port);");
            } else {
                CPP.Add("$q->connectToHostEncrypted($check(host)->qstring(), port);");
            }
        }
        public WebReply Process(WebRequest request) {
            switch (request.GetMethod()) {
                case WebMethod.Get: return (WebReply)CPP.ReturnObject("WebReply::$new($q->get(*$check(request)->$value()))");
                case WebMethod.Post: return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*$check(request)->$value(), *$check(request)->GetData()->$value()))");
                case WebMethod.Put: return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*$check(request)->$value(), *$check(request)->GetData()->$value()))");
                case WebMethod.Head: return (WebReply)CPP.ReturnObject("WebReply::$new($q->head(*$check(request)->$value()))");
                case WebMethod.Delete: return (WebReply)CPP.ReturnObject("WebReply::$new($q->deleteResource(*$check(request)->$value()))");
            }
            return null;
        }
        public WebReply Get(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->get(*$check(request)->$value()))");
        }
        public WebReply Post(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*$check(request)->$value(), *$check(request)->GetData()->$value()))");
        }
        public WebReply Post(WebRequest request, IOStream io) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*$check(request)->$value(), io->$q.get()))");
        }
        public WebReply Post(WebRequest request, ByteArray array) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->post(*$check(request)->$value(), *array->$value()))");
        }
        public WebReply Put(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*$check(request)->$value(), *$check(request)->GetData()->$value()))");
        }
        public WebReply Put(WebRequest request, IOStream io) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*$check(request)->$value(), io->$q.get()))");
        }
        public WebReply Put(WebRequest request, ByteArray array) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->put(*$check(request)->$value(), *array->$value()))");
        }
        public WebReply Delete(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->deleteResource(*$check(request)->$value()))");
        }
        public WebReply Head(WebRequest request) {
            return (WebReply)CPP.ReturnObject("WebReply::$new($q->head(*$check(request)->$value()))");
        }
    }
}
