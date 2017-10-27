using Qt.QSharp;

namespace Qt.Core {
    public class Console {
        public static void Write(String str) {
            CPP.Add("printf(\"%s\", str->cstring());\r\n");
        }
        public static void WriteLine(String str) {
            CPP.Add("printf(\"%s\\r\\n\", str->cstring());\r\n");
        }
    }
};
