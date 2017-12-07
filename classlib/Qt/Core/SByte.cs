using Qt.QSharp;

namespace Qt.Core {
    public class SByte {
        public static string ToString(sbyte x) {
            return CPP.ReturnString("String::$new(std::to_string(x))");
        }
    }
}
