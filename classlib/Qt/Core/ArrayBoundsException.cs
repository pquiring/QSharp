using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP(
      "void $abe() {throw Qt::Core::ArrayBoundsException::$new();}" +
      "void $abe(int idx, int size) {throw Qt::Core::ArrayBoundsException::$new(idx, size);}"
    )]
    public class ArrayBoundsException : Exception {
        public ArrayBoundsException() {}
        public ArrayBoundsException(int idx, int size) {}
        public ArrayBoundsException(String msg) {
            this.msg = msg;
        }
    }
}