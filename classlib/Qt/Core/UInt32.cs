using Qt.QSharp;

namespace Qt.Core {
    public class UInt32 {
        public static string ToString(uint x, int radix = 10) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, radix))");
        }
        public static uint ValueOf(String s, int radix = 10) {
            return CPP.ReturnUInt("$check(s)->$value()->toUInt(nullptr, radix)");
        }
    }
}
