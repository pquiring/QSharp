using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    [CPPClass("QLocalSocket* $d() {return dynamic_cast<QLocalSocket*>($q.get());}")]
    public class LocalSocket : IOStream {
        public LocalSocket() {
            CPP.Add("$base(new QLocalSocket());");
        }
        [CPPReplaceArgs("QLocalSocket *$s")]
        private LocalSocket(NativeArg1 arg) {
            CPP.Add("$base($s);");
        }
        public void Connect(String name) {
            CPP.Add("$d()->connectToServer($check(name)->qstring())");
        }
        public void Disconnect() {
            CPP.Add("$d()->disconnectFromServer();");
        }
    }
}
