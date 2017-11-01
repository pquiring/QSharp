using Qt.QSharp;

namespace Qt.Core {
    public class UInt64 {
        public static string ToString(ulong x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
    }
}
