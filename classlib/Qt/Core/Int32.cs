using Qt.QSharp;

namespace Qt.Core {
    public class Int32 {
        public static string ToString(int x, int radix = 10) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, radix))");
        }
        public static int ValueOf(String s, int radix = 10) {
            return CPP.ReturnInt("$check(s)->$value()->toInt(nullptr, radix)");
        }
    }
}
