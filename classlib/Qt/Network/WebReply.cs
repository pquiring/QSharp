using Qt.QSharp;
using Qt.Core;

namespace Qt.Network {
    [CPPClass("QNetworkReply* $d() {return dynamic_cast<QNetworkReply*>($q.get());}")]
    public class WebReply : IOStream {
        protected WebReply() {
            CPP.Add("$base(new $QWebReply());");
        }
        [CPPReplaceArgs("QNetworkReply *reply")]
        private WebReply(NativeArg1 arg) {
            CPP.Add("$base(reply);");
        }
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
            return CPP.ReturnString("Qt::Core::String::$new($d()->rawHeader(QByteArray($check(header)->cstring())))");
        }
        public String GetHeaders() {
            CPP.Add("QList<QByteArray> list = $d()->rawHeaderList();");
            CPP.Add("QByteArray array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("for(int i=0;i<cnt;i++) {array.append(list[i]); array.append(\"\\r\\n\");}");
            return CPP.ReturnString("Qt::Core::String::$new(array)");
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
