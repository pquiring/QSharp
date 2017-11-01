using Qt.QSharp;

namespace Qt.Core {
    public class Int32 {
        public static string ToString(int x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
