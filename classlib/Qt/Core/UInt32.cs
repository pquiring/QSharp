using Qt.QSharp;

namespace Qt.Core {
    public class UInt32 {
        public static string ToString(uint x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
