using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP("void $npe() {throw new Qt::Core::NullPointerException();}")]
    public class NullPointerException : Exception {
        public NullPointerException() {}
        public NullPointerException(String msg) {
            this.msg = msg;
        }
    }
}