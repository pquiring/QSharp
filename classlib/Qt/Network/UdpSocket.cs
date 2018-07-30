using Qt.QSharp;

namespace Qt.Network {
    [CPPClass("QUdpSocket* $d() {return dynamic_cast<QUdpSocket*>($q.get());}")]
    public class UdpSocket : AbstractSocket {
        public UdpSocket() {
            CPP.Add("$base(new QUdpSocket());");
        }
    }
}
