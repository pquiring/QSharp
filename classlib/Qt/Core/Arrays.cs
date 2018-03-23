using Qt.QSharp;

namespace Qt.Core {
    /** Array functions */
    public class Arrays<T> {
        public delegate int SortCompare(T o1, T o2);
        public static T[] CopyOf(T[] array, int pos, int length = -1) {
            if (length == -1) {
                length = array.Length - pos;
            }
            if (length < 0) return null;
            if (pos == 0 && length == array.Length) return array;
            T[] copy = new T[length];
            CPP.Add("if (sizeof(T) <= 8) {");
            CPP.Add("  std::memcpy(copy->data(), array->data() + pos, length * sizeof(T));");
            CPP.Add("} else {");
            for(int a=0;a<length;a++) {
                copy[a] = array[pos++];
            }
            CPP.Add("}");
            return copy;
        }

        public static void Copy(T[] src, int srcPos, T[] dest, int destPos, int length = -1) {
            if (length == -1) {
                length = src.Length - srcPos;
            }
            if (dest.Length - destPos < length) throw new ArrayBoundsException("dest bounds");
            if (src.Length - srcPos < length) throw new ArrayBoundsException("src bounds");
            CPP.Add("if (sizeof(T) <= 8) {");
            CPP.Add("std::memmove(dest->data() + destPos, src->data() + srcPos, length * sizeof(T));");
            CPP.Add("} else {");
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
            CPP.Add("}");
        }

        public static void Fill(T[] array, T value) {
            int length = array.Length;
            for(int pos=0;pos<length;pos++) {
                array[pos] = value;
            }
        }
        public static void Fill(T[] array, int pos, int length, T value) {
            length += pos;
            for(;pos<length;pos++) {
                array[pos] = value;
            }
        }

        public static void Sort(T[] array, SortCompare cmp) {
            int length = array.Length;
            T o1, o2;
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
