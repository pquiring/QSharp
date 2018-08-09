using Qt.QSharp;

namespace Qt.Core {
    public class Byte {
        public static string ToString(byte x) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x))");
        }
        public static byte ValueOf(String s, int radix = 10) {
            return CPP.ReturnByte("$check(s)->$value()->toInt(nullptr, radix)");
        }
    }
}
