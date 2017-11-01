using Qt.QSharp;

namespace Qt.Core {
    public class SByte {
        public static string ToString(sbyte x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
