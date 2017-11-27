using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass(
        "public: std::shared_ptr<QNetworkReply> $q;" +
        "public: WebReply() {}" +  //invalid
        "public: WebReply(QNetworkReply *reply) {$q.reset(reply); IOStream::$base((std::shared_ptr<QIODevice>)$q);} "
    )]
    public abstract class WebReply : IOStream {
        protected WebReply() {}
        private ByteArray data;
        private Map<String, String> args = new Map<String, String>();
        private void GetArgs(String[] kv) {
            for(int a=0;a<kv.Length;a++) {
                int idx = kv[a].IndexOf('=');
                if (idx == -1) continue;
                String key = kv[a].Substring(0, idx);
                String value = kv[a].Substring(idx+1);
                args.Set(key, value);
            }
        }
        public String GetHeader(String header) {
            return CPP.ReturnString("std::make_shared<String>($q->rawHeader(QByteArray(header->cstring())))");
        }
        public String GetHeaders() {
            CPP.Add("QList<QByteArray> list = $q->rawHeaderList();");
            CPP.Add("QByteArray array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("for(int i=0;i<cnt;i++) {array.append(list[i]); array.append(\"\\r\\n\");}");
            return CPP.ReturnString("std::make_shared<String>(array)");
        }
        public new ByteArray ReadAll() {
            data = base.ReadAll();
            GetArgs(new String(data).Split('&'));
            return data;
        }
        public void SetData(ByteArray array) {
            data = array;
            GetArgs(new String(data).Split('&'));
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
    }
}
