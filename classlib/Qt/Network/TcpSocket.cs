using Qt.QSharp;

namespace Qt.Network {
    [CPPClass("QTcpSocket* $d() {return dynamic_cast<QTcpSocket*>($q.get());}")]
    public class TcpSocket : AbstractSocket {
        public TcpSocket() {
            CPP.Add("$base(new QTcpSocket());");
        }
        [CPPReplaceArgs("QTcpSocket *$s")]
        private TcpSocket(NativeArg1 arg) {
            CPP.Add("$base($s);");
        }
    }
}
