using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP("void $npe() {throw Qt::Core::NullPointerException::$new();}")]
    public class NullPointerException : Exception {
        public NullPointerException() {}
        public NullPointerException(String msg) {
            this.msg = msg;
        }
    }
}