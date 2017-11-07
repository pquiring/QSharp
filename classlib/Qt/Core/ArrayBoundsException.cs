using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP("void $abe() {throw ArrayBoundsException::$new();}\r\n")]
    public class ArrayBoundsException : Exception {
        public ArrayBoundsException() {}
        public ArrayBoundsException(String msg) {
            this.msg = msg;
        }
    }
}