using Qt.QSharp;

namespace Qt.Core {
    public class Double {
        public static string ToString(double x, int precision = 6) {
            return CPP.ReturnString("Qt::Core::String::$new(QString::number(x, 'g', precision))");
        }
        public static double ValueOf(String s) {
            return CPP.ReturnDouble("$check(s)->$value()->toDouble()");
        }
    }
}
