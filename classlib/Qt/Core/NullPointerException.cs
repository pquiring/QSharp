using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP("void $npe() {throw NullPointerException::$new();}\r\n")]
    public class NullPointerException : Exception {
        public NullPointerException() {}
        public NullPointerException(String msg) {
            this.msg = msg;
        }
    }
}