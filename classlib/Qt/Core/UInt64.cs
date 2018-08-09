using Qt.QSharp;

namespace Qt.Core {
    public class UInt64 {
        public static string ToString(ulong x, int radix = 10) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, radix))");
        }
        public static ulong ValueOf(String s, int radix = 10) {
            return CPP.ReturnULong("$check(s)->$value()->toULongLong(nullptr, radix)");
        }
    }
}
