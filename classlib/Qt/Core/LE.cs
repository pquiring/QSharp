namespace Qt.Core {

public class LE {

    public static int getuint8(byte[] data, int offset) {
        int ret;
        ret    = (int)data[offset] & 0xff;
        return ret;
    }

    public static int getuint16(byte[] data, int offset) {
        int ret;
        ret    = data[offset] & 0xff;
        ret += (data[offset+1] & 0xff) << 8;
        return ret;
    }

    public static int getuint32(byte[] data, int offset) {
        int ret;
        ret  = (int)data[offset] & 0xff;
        ret += ((int)data[offset+1] & 0xff) << 8;
        ret += ((int)data[offset+2] & 0xff) << 16;
        ret += ((int)data[offset+3] & 0xff) << 24;
        return ret;
    }

    public static long getuint64(byte[] data, int offset) {
        long ret;
        ret  = (long)data[offset] & 0xff;
        ret += ((long)data[offset+1] & 0xff) << 8;
        ret += ((long)data[offset+2] & 0xff) << 16;
        ret += ((long)data[offset+3] & 0xff) << 24;
        ret += ((long)data[offset+4] & 0xff) << 32;
        ret += ((long)data[offset+5] & 0xff) << 40;
        ret += ((long)data[offset+6] & 0xff) << 48;
        ret += ((long)data[offset+7] & 0xff) << 56;
        return ret;
    }

    public static String getString(byte[] data, int offset, int len) {
        String ret = "";
        while (len > 0) {
            if (data[offset]==0) break;
            ret += (char)data[offset++];
            len--;
        }
        return ret;
    }

    public static void setuint8(byte[] data, int offset, int num) {
        data[offset] = (byte)(num & 0xff);
    }

    public static void setuint16(byte[] data, int offset, int num) {
        data[offset+0] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+1] = (byte)(num & 0xff);
    }

    public static void setuint32(byte[] data, int offset, int num) {
        data[offset+0] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+1] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+2] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+3] = (byte)(num & 0xff);
    }

    public static void setuint64(byte[] data, int offset, long num) {
        data[offset+0] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+1] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+2] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+3] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+4] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+5] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+6] = (byte)(num & 0xff);
        num >>= 8;
        data[offset+7] = (byte)(num & 0xff);
    }

    public static void setString(byte[] data, int offset, int len, String str) {
        int pos = 0;
        while (len > 0) {
            if (pos >= str.Length) {
                data[offset++] = 0;
            } else {
                data[offset++] = (byte)str.CharAt(pos++);
            }
            len--;
        }
    }

    public static short[] byteArray2shortArray(byte[] inArray, short[] outArray) {
        int len = inArray.Length / 2;
        if (outArray == null) outArray = new short[len];
        int p = 0;
        short val;
        for (int a = 0; a < len; a++) {
            val  = (short)(inArray[p++] & 0xff);
            val += (short)((inArray[p++] & 0xff) << 8);
            outArray[a] = val;
        }
        return outArray;
    }
    public static int[] byteArray2intArray(byte[] inArray, int[] outArray) {
        int len = inArray.Length / 4;
        if (outArray == null) outArray = new int[len];
        int p = 0;
        int val;
        for (int a = 0; a < len; a++) {
            val  = (inArray[p++] & 0xff);
            val += (inArray[p++] & 0xff) << 8;
            val += (inArray[p++] & 0xff) << 16;
            val += (inArray[p++] & 0xff) << 24;
            outArray[a] = val;
        }
        return outArray;
    }
    public static byte[] shortArray2byteArray(short[] inArray, byte[] outArray) {
        int len = inArray.Length;
        if (outArray == null) outArray = new byte[len * 2];
        int p = 0;
        ushort val;
        for (int a = 0; a < len; a++) {
            val = (ushort)inArray[a];
            outArray[p++] = (byte) (val & 0xff);
            outArray[p++] = (byte) (val >> 8);
        }
        return outArray;
    }
    public static byte[] intArray2byteArray(int[] inArray, byte[] outArray) {
        int len = inArray.Length;
        if (outArray == null) outArray = new byte[len * 4];
        int p = 0;
        uint val;
        for (int a = 0; a < len; a++) {
            val = (uint)inArray[a];
            outArray[p++] = (byte) (val & 0xff);
            val >>= 8;
            outArray[p++] = (byte) (val & 0xff);
            val >>= 8;
            outArray[p++] = (byte) (val & 0xff);
            val >>= 8;
            outArray[p++] = (byte) (val);
        }
        return outArray;
    }
}

}
