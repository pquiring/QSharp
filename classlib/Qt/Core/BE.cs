namespace Qt.Core {

public class BE {

    public static int getuint8(byte[] data, int offset) {
        return ((int)data[offset]) & 0xff;
    }

    public static int getuint16(byte[] data, int offset) {
        int ret;
        ret = ((int) data[offset] & 0xff) << 8;
        ret += ((int)data[offset + 1]) & 0xff;
        return ret;
    }

    public static int getuint32(byte[] data, int offset) {
        int ret;
        ret = ((int) data[offset] & 0xff) << 24;
        ret += ((int) data[offset + 1] & 0xff) << 16;
        ret += ((int) data[offset + 2] & 0xff) << 8;
        ret += ((int) data[offset + 3] & 0xff);
        return ret;
    }

    public static long getuint64(byte[] data, int offset) {
        long ret;
        ret = ((long) data[offset] & 0xff) << 56;
        ret += ((long) data[offset + 1] & 0xff) << 48;
        ret += ((long) data[offset + 2] & 0xff) << 40;
        ret += ((long) data[offset + 3] & 0xff) << 32;
        ret += ((long) data[offset + 4] & 0xff) << 24;
        ret += ((long) data[offset + 5] & 0xff) << 16;
        ret += ((long) data[offset + 6] & 0xff) << 8;
        ret += ((long) data[offset + 7] & 0xff);
        return ret;
    }

    public static void setuint8(byte[] data, int offset, int value) {
        data[offset+0] = (byte)(value & 0xff);
    }

    public static void setuint16(byte[] data, int offset, int value) {
        data[offset+1] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+0] = (byte)(value & 0xff);
    }

    public static void setuint32(byte[] data, int offset, int value) {
        data[offset+3] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+2] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+1] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+0] = (byte)(value & 0xff);
    }

    public static void setuint64(byte[] data, int offset, long value) {
        data[offset+7] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+6] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+5] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+4] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+3] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+2] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+1] = (byte)(value & 0xff);
        value >>= 8;
        data[offset+0] = (byte)(value & 0xff);
    }

    //these funcs are no different (just conv functions)

    public static String getString(byte[] data, int offset, int len) {
        return LE.getString(data, offset, len);
    }

    public static void setString(byte[] data, int offset, int len, String str) {
        LE.setString(data, offset, len, str);
    }

    public static short[] byteArray2shortArray(byte[] inArray, short[] outArray) {
        int len = inArray.Length / 2;
        if (outArray == null) outArray = new short[len];
        int p = 0;
        short val;
        for (int a = 0; a < len; a++) {
            val  = (short)((((short)inArray[p++]) & 0xff) << 8);
            val += (short)((inArray[p++]) & 0xff);
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
            val    = (((int)inArray[p++]) & 0xff) << 24;
            val += (((int)inArray[p++]) & 0xff) << 16;
            val += (((int)inArray[p++]) & 0xff) << 8;
            val += (((int)inArray[p++]) & 0xff);
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
            outArray[p++] = (byte) ((val & 0xff00) >> 8);
            outArray[p++] = (byte) (val & 0xff);
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
            outArray[p++] = (byte) ((val & 0xff000000) >> 8);
            outArray[p++] = (byte) ((val & 0xff0000) >> 8);
            outArray[p++] = (byte) ((val & 0xff00) >> 8);
            outArray[p++] = (byte) (val & 0xff);
        }
        return outArray;
    }

}

}
