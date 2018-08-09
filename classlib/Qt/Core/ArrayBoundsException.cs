using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassCPP(
      "void $abe() {throw new Qt::Core::ArrayBoundsException();}" +
      "void $abe(int idx, int size) {throw new Qt::Core::ArrayBoundsException(idx, size);}"
    )]
    public class ArrayBoundsException : Exception {
        public ArrayBoundsException() {}
        public ArrayBoundsException(int idx, int size) {}
        public ArrayBoundsException(String msg) {
            this.msg = msg;
        }
    }
}