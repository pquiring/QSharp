using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP("void $abe() {throw Qt::Core::ArrayBoundsException::$new();}")]
    public class ArrayBoundsException : Exception {
        public ArrayBoundsException() {}
        public ArrayBoundsException(String msg) {
            this.msg = msg;
        }
    }
}