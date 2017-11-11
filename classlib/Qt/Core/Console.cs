using Qt.QSharp;

namespace Qt.Core {
    public class Console {
        /** Writes a String to stdout. */
        public static void Write(String str) {
            CPP.Add("std::printf(\"%s\", str->cstring());");
        }
        /** Writes a String and \r\n to stdout. */
        public static void WriteLine(String str) {
            CPP.Add("std::printf(\"%s\\r\\n\", str->cstring());");
        }
    }
};
