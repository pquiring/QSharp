using Qt.QSharp;

namespace Qt.Core {
    public class Int64 {
        public static string ToString(long x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
