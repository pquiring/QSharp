using Qt.QSharp;

namespace Qt.Core {
    public class Float {
        public static string ToString(float x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
