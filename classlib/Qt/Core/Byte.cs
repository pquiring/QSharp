using Qt.QSharp;

namespace Qt.Core {
    public class Byte {
        public static string ToString(byte x) {
            return CPP.ReturnString("String::$new(std::to_string(x))");
        }
    }
}
