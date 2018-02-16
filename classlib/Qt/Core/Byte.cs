using Qt.QSharp;

namespace Qt.Core {
    public class Byte {
        public static string ToString(byte x) {
            return CPP.ReturnString("Qt::Core::String::$new(std::to_string(x))");
        }
    }
}
