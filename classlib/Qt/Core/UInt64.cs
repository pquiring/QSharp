using Qt.QSharp;

namespace Qt.Core {
    public class UInt64 {
        public static string ToString(ulong x) {
            return CPP.ReturnString("Qt::Core::String::$new(std::to_string(x))");
        }
        public static string ToString(ulong x, uint radix) {
            char[] chs = new char[64];
            int pos = 63;
            int len = 0;
            while (x > 0) {
                ulong digit = x % radix;
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
            return new String(chs, pos+1, len);
        }
    }
}
