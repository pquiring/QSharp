using Qt.QSharp;

namespace Qt.Core {
    public class Double {
        public static string ToString(double x, int precision = 6) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, 'g', precision))");
        }
        public static double ValueOf(String s) {
            return CPP.ReturnDouble("$check(s)->$value()->toDouble()");
        }
        public static double LongBitsToDouble(long bits) {
            double value = 0f;
            CPP.Add("union {int64 _bits; double _double;};");
            CPP.Add("_bits = bits;");
            CPP.Add("value = _double;");
            return value;
        }
        public static long DoubleToLongBits(double value) {
            long bits = 0;
            CPP.Add("union {int64 _bits; double _double;};");
            CPP.Add("_double = value;");
            CPP.Add("bits = _bits;");
            return bits;
        }
    }
}
