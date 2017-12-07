using Qt.QSharp;

namespace Qt.Core {
    public class Float {
        public static string ToString(float x) {
            return CPP.ReturnString("String::$new(std::to_string(x))");
        }
    }
}
