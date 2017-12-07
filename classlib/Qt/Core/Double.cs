using Qt.QSharp;

namespace Qt.Core {
    public class Double {
        public static string ToString(double x) {
            return CPP.ReturnString("String::$new(std::to_string(x))");
        }
    }
}
