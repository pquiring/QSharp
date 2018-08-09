using Qt.QSharp;

namespace Qt.Core {
    public class Float {
        public static string ToString(float x, int precision = 6) {
            return CPP.ReturnString("new Qt::Core::String(QString::number(x, 'g', precision))");
        }
        public static float ValueOf(String s) {
            return CPP.ReturnFloat("$check(s)->$value()->toFloat()");
        }
        public static float IntBitsToFloat(int bits) {
            float value = 0f;
            CPP.Add("union {int32 _bits; float _float;};");
            CPP.Add("_bits = bits;");
            CPP.Add("value = _float;");
            return value;
        }
        public static int FloatToIntBits(float value) {
            int bits = 0;
            CPP.Add("union {int32 _bits; float _float;};");
            CPP.Add("_float = value;");
            CPP.Add("bits = _bits;");
            return bits;
        }
    }
}
