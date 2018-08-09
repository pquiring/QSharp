using Qt.QSharp;

namespace Qt.Core {
    public class Int64 {
        public static string ToString(long x, int radix = 10) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, radix))");
        }
        public static long ValueOf(String s, int radix = 10) {
            return CPP.ReturnLong("$check(s)->$value()->toLongLong(nullptr, radix)");
        }
    }
}
