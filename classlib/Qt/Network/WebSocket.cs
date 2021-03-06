using Qt.Core;
using Qt.QSharp;

namespace Qt.Network {
    public delegate void TextMessageReceived(String msg);
    public delegate void BinaryMessageReceived(ByteArray msg);
    [CPPClass(
        "QWebSocket *$q;" +
        "void $base(QWebSocket *ws) {$q = ws;}"
    )]
    public class WebSocket {
        [CPPReplaceArgs("QWebSocket *$s")]
        private WebSocket(NativeArg1 arg) {
            CPP.Add("$q = $s;");
        }
        private TextMessageReceived textReceived;
        private BinaryMessageReceived binaryReceived;
        private void SlotTextMessageReceived(String msg) {
            try {
                if (textReceived != null) {
                    textReceived(msg);
                }
            } catch {}
        }
        private void SlotBinaryMessageReceived(ByteArray msg) {
            try {
                if (binaryReceived != null) {
                    binaryReceived(msg);
                }
            } catch {}
        }
        public void SendTextMessage(String msg) {
            CPP.Add("$q->sendTextMessage($check(msg)->qstring());");
        }
        public void SendBinaryMessage(ByteArray msg) {
            CPP.Add("$q->sendBinaryMessage(*$check(msg)->$value());");
        }
        public void OnTextMessage(TextMessageReceived handler) {
            textReceived = handler;
            CPP.Add("QObject::connect($q, &QWebSocket::textMessageReceived, [=] (QString msg) {this->SlotTextMessageReceived(new Qt::Core::String(msg));});");
        }
        public void OnBinaryMessage(BinaryMessageReceived handler) {
            binaryReceived = handler;
            CPP.Add("QObject::connect($q, &QWebSocket::binaryMessageReceived, [=] (QByteArray msg) {this->SlotBinaryMessageReceived(new Qt::Core::ByteArray(msg));});");
        }
    }
}
