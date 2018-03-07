using Qt.QSharp;

namespace Qt.Core {
    /** Array functions */
    public class Arrays {
        public static byte[] CopyOf(byte[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            byte[] copy = new byte[length];
            CPP.Add("std::memcpy(copy->data(), array->data() + pos, length);");
            return copy;
        }
        public static short[] CopyOf(short[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            short[] copy = new short[length];
            CPP.Add("std::memcpy(copy->data(), array->data() + pos, length * 2);");
            return copy;
        }
        public static int[] CopyOf(int[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            int[] copy = new int[length];
            CPP.Add("std::memcpy(copy->data(), array->data() + pos, length * 4);");
            return copy;
        }
        public static long[] CopyOf(long[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            long[] copy = new long[length];
            CPP.Add("std::memcpy(copy->data(), array->data() + pos, length * 8);");
            return copy;
        }
        public static float[] CopyOf(float[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            float[] copy = new float[length];
            CPP.Add("std::memcpy(copy->data(), array->data() + pos, length * 4);");
            return copy;
        }
        public static double[] CopyOf(double[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            double[] copy = new double[length];
            CPP.Add("std::memcpy(copy->data(), array->data() + pos, length * 8);");
            return copy;
        }

        public static void Copy(byte[] dest, int destPos, byte[] src, int srcPos, int length = -1) {
            if (length == -1) {
                length = src.Length;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length);");
        }
        public static void Copy(short[] dest, int destPos, short[] src, int srcPos, int length = -1) {
            if (length == -1) {
                length = src.Length;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 2);");
        }
        public static void Copy(int[] dest, int destPos, int[] src, int srcPos, int length = -1) {
            if (length == -1) {
                length = src.Length;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 4);");
        }
        public static void Copy(long[] dest, int destPos, long[] src, int srcPos, int length = -1) {
            if (length == -1) {
                length = src.Length;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 8);");
        }
        public static void Copy(float[] dest, int destPos, float[] src, int srcPos, int length = -1) {
            if (length == -1) {
                length = src.Length;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 4);");
        }
        public static void Copy(double[] dest, int destPos, double[] src, int srcPos, int length = -1) {
            if (length == -1) {
                length = src.Length;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 8);");
        }

    }
}
