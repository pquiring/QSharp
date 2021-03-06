using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    public enum WebMethod {Get, Post, Put, Delete, Head, Unknown}
    [CPPClass(
        "std::qt_ptr<QNetworkRequest> $q;" +
        "QNetworkRequest *$value() {return $q.get();}"
    )]
    public class WebRequest {
        private ByteArray data;
        private WebMethod method = WebMethod.Get;
        private Url url;
        private Map<String, String> args = new Map<String, String>((String k1, String k2) => {return 0;});
        public WebRequest(Url url) {
            SetUrl(url);
        }
        public WebRequest(WebMethod method, Url url) {
            this.method = method;
            SetUrl(url);
        }
        public void SetUrl(Url url) {
            this.url = url;
            CPP.Add("$q = new QNetworkRequest(*$check(url)->$value());");
            GetArgs();
        }
        public Url GetUrl() {
            return url;
        }
        private void GetArgs(String[] kv) {
            for(int a=0;a<kv.Length;a++) {
                int idx = kv[a].IndexOf('=');
                if (idx == -1) continue;
                String key = kv[a].Substring(0, idx);
                String value = kv[a].Substring(idx+1);
                args.Set(key, value);
            }
        }
        private void GetArgs() {
            GetArgs(url.GetQuery().Split('&'));
            if (data != null) {
                GetArgs(new String(data).Split('&'));
            }
        }
        public String GetHeader(String header) {
            return CPP.ReturnString("new Qt::Core::String($q->rawHeader(QByteArray($check(header)->cstring())))");
        }
        public void SetHeader(String header, String value) {
            CPP.Add("$q->setRawHeader(QByteArray($check(header)->cstring()), QByteArray($check(value)->cstring()));");
        }
        public void SetData(ByteArray array) {
            data = array;
            GetArgs();
        }
        public void SetData(String str) {
            data = new ByteArray(str);
            GetArgs();
        }
        public ByteArray GetData() {
            return data;
        }
        public String GetParameter(String name) {
            if (args.Contains(name)) {
                return args.Get(name);
            }
            return null;
        }
        public WebMethod GetMethod() {
            return method;
        }
        public void SetMethod(WebMethod method) {
            this.method = method;
        }
        public static WebMethod GetMethod(String method) {
            method = method.ToUpperCase();
            switch (method) {
                case "GET": return WebMethod.Get;
                case "POST": return WebMethod.Post;
                case "PUT": return WebMethod.Put;
                case "DELETE": return WebMethod.Delete;
                case "HEAD": return WebMethod.Head;
            }
            return WebMethod.Unknown;
        }
    }
}
