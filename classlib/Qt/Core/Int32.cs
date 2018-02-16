using Qt.QSharp;

namespace Qt.Core {
    public class Int32 {
        public static string ToString(int x) {
            return CPP.ReturnString("Qt::Core::String::$new(std::to_string(x))");
        }
        public static string ToString(int x, int radix) {
            char[] chs = new char[33];
            int pos = 32;
            int len = 0;
            bool negative = false;
            if (x < 0) {
                negative = true;
                x *= -1;
            }
            while (x > 0) {
                int digit = x % radix;
                x /= radix;
                if (digit > 9) {
                    digit -= 10;
                    chs[pos--] = (char)('a' + digit);
                } else {
                    chs[pos--] = (char)('0' + digit);
                }
                len++;
            }
            if (len == 0) {
                chs[pos--] = '0';
                len++;
            }
            if (negative) {
                chs[pos--] = '-';
                len++;
            }
            return new String(chs, pos+1, len);
        }
    }
}
