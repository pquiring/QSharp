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
        public static sbyte[] CopyOf(sbyte[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            sbyte[] copy = new sbyte[length];
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
        public static ushort[] CopyOf(ushort[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            ushort[] copy = new ushort[length];
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
        public static uint[] CopyOf(uint[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            uint[] copy = new uint[length];
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
        public static ulong[] CopyOf(ulong[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            ulong[] copy = new ulong[length];
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
        public static Object[] CopyOf(Object[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            Object[] copy = new Object[length];
            for(int a=0;a<length;a++) {
                copy[a] = array[pos++];
            }
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
        public static void Copy(sbyte[] src, int srcPos, sbyte[] dest, int destPos, int length = -1) {
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
        public static void Copy(ushort[] src, int srcPos, ushort[] dest, int destPos, int length = -1) {
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
        public static void Copy(uint[] src, int srcPos, uint[] dest, int destPos, int length = -1) {
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
        public static void Copy(ulong[] src, int srcPos, ulong[] dest, int destPos, int length = -1) {
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
        public static void Copy(Object[] src, int srcPos, Object[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            if (src == dest && destPos > srcPos) {
                //copy backwards
                srcPos += length;
                destPos += length;
                for(int a=0;a<length;a++) {
                    dest[--destPos] = src[--srcPos];
                }
            } else {
                //copy forwards
                for(int a=0;a<length;a++) {
                    dest[destPos++] = src[srcPos++];
                }
            }
        }

        public static void Fill(byte[] array, byte value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(byte[] array, int pos, int length, byte value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(sbyte[] array, sbyte value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(sbyte[] array, int pos, int length, sbyte value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(short[] array, short value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(short[] array, int pos, int length, short value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(ushort[] array, ushort value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(ushort[] array, int pos, int length, ushort value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(int[] array, int value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(int[] array, int pos, int length, int value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(uint[] array, uint value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(uint[] array, int pos, int length, uint value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(long[] array, long value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(long[] array, int pos, int length, long value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(ulong[] array, ulong value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(ulong[] array, int pos, int length, ulong value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(float[] array, float value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(float[] array, int pos, int length, float value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(double[] array, double value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(double[] array, int pos, int length, double value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(Object[] array, Object value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(Object[] array, int pos, int length, Object value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }

        public static void Sort(byte[] array) {
            int length = array.Length;
            byte v1;
            byte v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(sbyte[] array) {
            int length = array.Length;
            sbyte v1;
            sbyte v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(short[] array) {
            int length = array.Length;
            short v1;
            short v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(ushort[] array) {
            int length = array.Length;
            ushort v1;
            ushort v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(int[] array) {
            int length = array.Length;
            int v1;
            int v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(uint[] array) {
            int length = array.Length;
            uint v1;
            uint v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(long[] array) {
            int length = array.Length;
            long v1;
            long v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(ulong[] array) {
            int length = array.Length;
            ulong v1;
            ulong v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(float[] array) {
            int length = array.Length;
            float v1;
            float v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
                    }
                }
            }
        }
        public static void Sort(double[] array) {
            int length = array.Length;
            double v1;
            double v2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    v1 = array[i1];
                    v2 = array[i2];
                    if (v1 > v2) {
                        array[i1] = v2;
                        array[i2] = v1;
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
