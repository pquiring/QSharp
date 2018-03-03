using Qt.QSharp;

namespace Qt.Core {
    public class Float {
        public static string ToString(float x, int precision = 6) {
            return CPP.ReturnString("Qt::Core::String::$new(QString::number(x, 'g', precision))");
        }
        public static float ValueOf(String s) {
            return CPP.ReturnFloat("$check(s)->$value()->toFloat()");
        }
    }
}
