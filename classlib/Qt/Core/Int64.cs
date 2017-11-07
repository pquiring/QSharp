using Qt.QSharp;

namespace Qt.Core {
    public class Int64 {
        public static string ToString(long x) {
            return CPP.ReturnString("std::make_shared<String>(std::to_string(x))");
        }
        public static string ToString(long x, int radix) {
            char[] chs = new char[65];
            int pos = 64;
            int len = 0;
            bool negative = false;
            if (x < 0) {
                negative = true;
                x *= -1;
            }
            while (x > 0) {
                long digit = x % radix;
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
