using Qt.QSharp;

namespace Qt.Core {
    public class Byte {
        public static string ToString(byte x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
