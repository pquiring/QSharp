using Qt.QSharp;

namespace Qt.Core {
    public class Double {
        public static string ToString(double x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
