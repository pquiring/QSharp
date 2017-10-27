namespace Qt.Core {
    public class Exception : System.Exception {
        public String msg;
        public Exception() {}
        public Exception(String msg) {
            this.msg = msg;
        }
    }
}
