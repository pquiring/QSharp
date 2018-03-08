using Qt.QSharp;

namespace Qt.Core {
    public delegate int SortCompare(Object o1, Object o2);
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

        public static void Copy(byte[] src, int srcPos, byte[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length);");
        }
        public static void Copy(short[] src, int srcPos, short[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 2);");
        }
        public static void Copy(int[] src, int srcPos, int[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 4);");
        }
        public static void Copy(long[] src, int srcPos, long[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 8);");
        }
        public static void Copy(float[] src, int srcPos, float[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 4);");
        }
        public static void Copy(double[] src, int srcPos, double[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * 8);");
        }

        public static void Sort(byte[] array) {
            int length = array.Length;
            byte tmp;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    if (array[i1] > array[i2]) {
                        //swap values
                        tmp = array[i1];
                        array[i1] = array[i2];
                        array[i2] = tmp;
                    }
                }
            }
        }
        public static void Sort(short[] array) {
            int length = array.Length;
            short tmp;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    if (array[i1] > array[i2]) {
                        //swap values
                        tmp = array[i1];
                        array[i1] = array[i2];
                        array[i2] = tmp;
                    }
                }
            }
        }
        public static void Sort(int[] array) {
            int length = array.Length;
            int tmp;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    if (array[i1] > array[i2]) {
                        //swap values
                        tmp = array[i1];
                        array[i1] = array[i2];
                        array[i2] = tmp;
                    }
                }
            }
        }
        public static void Sort(long[] array) {
            int length = array.Length;
            long tmp;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    if (array[i1] > array[i2]) {
                        //swap values
                        tmp = array[i1];
                        array[i1] = array[i2];
                        array[i2] = tmp;
                    }
                }
            }
        }
        public static void Sort(float[] array) {
            int length = array.Length;
            float tmp;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    if (array[i1] > array[i2]) {
                        //swap values
                        tmp = array[i1];
                        array[i1] = array[i2];
                        array[i2] = tmp;
                    }
                }
            }
        }
        public static void Sort(double[] array) {
            int length = array.Length;
            double tmp;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    if (array[i1] > array[i2]) {
                        //swap values
                        tmp = array[i1];
                        array[i1] = array[i2];
                        array[i2] = tmp;
                    }
                }
            }
        }
        public static void Sort(Object[] array, SortCompare cmp) {
            int length = array.Length;
            Object o1, o2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    o1 = array[i1];
                    o2 = array[i2];
                    if (cmp(o1, o2) > 0) {
                        array[i1] = o2;
                        array[i2] = o1;
                    }
                }
            }
        }

    }
}
