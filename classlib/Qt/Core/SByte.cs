using Qt.QSharp;

namespace Qt.Core {
    public class SByte {
        public static string ToString(byte x, int radix = 10) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, radix))");
        }
        public static sbyte ValueOf(String s, int radix = 10) {
            return CPP.ReturnSByte("$check(s)->$value()->toInt(nullptr, radix)");
        }
    }
}
