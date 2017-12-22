using Qt.Core;

namespace Qt.QSharp {
    [CPPNonClassHPP(
        "template<typename T>\r\n" +
        "std::shared_ptr<Qt::QSharp::FixedArray<T>> $check(std::shared_ptr<Qt::QSharp::FixedArray<T>> array, int offset, int length)" +
        "{if (offset + length > $check(array)->Length) $abe(); return array;}\r\n"
    )]
    [CPPClass(
        "public: T *t;\r\n" +
        "public: T& operator[](int pos) {if (pos < 0 || pos > Length) $abe(); return t[pos];}\r\n" +
        "public: T* data() {return t;}\r\n" +
        "public: T& at(int pos) {if (pos < 0 || pos > Length) $abe(); return t[pos];}\r\n" +
        "public: int $get_Length() {return Length;}\r\n"
    )]
    public class FixedArray<T> : IEnumerable<T> {
        public FixedArray(int size) {
            Length = size;
            alloced = true;
            CPP.Add("if (size < 0) $abe(); t = (T*)new T[size]; std::memset(t, 0, size * sizeof(T));");
        }
        [CPPReplaceArgs("void *buf, int size")]
        private FixedArray(NativeArg1 arg, int size) {
            Length = size;
            alloced = false;
            CPP.Add("t = (T*)buf;");
        }
        [CPPReplaceArgs("void *buf, int size, bool copy")]
        private FixedArray(NativeArg2 arg, int size, bool copy) {
            Length = size;
            alloced = copy;
            CPP.Add("if (copy) {t = (T*)new T[size]; std::memcpy(t, buf, size * sizeof(T));} else {t = (T*)buf;}");
        }
        [CPPReplaceArgs("std::shared_ptr<Qt::Core::Object> objRef, void *buf, int size")]
        private FixedArray(Object objRef, NativeArg3 arg, int size) {
            Length = size;
            alloced = false;
            this.objRef = objRef;
            CPP.Add("t = (T*)buf;");
        }
        [CPPReplaceArgs("std::shared_ptr<Qt::Core::Object> objRef, void *buf, int size, bool copy")]
        private FixedArray(Object objRef, NativeArg4 arg, int size, bool copy) {
            Length = size;
            alloced = copy;
            this.objRef = objRef;
            CPP.Add("if (copy) {t = (T*)new T[size]; std::memcpy(t, buf, size * sizeof(T));} else {t = (T*)buf;}");
        }
        [CPPReplaceArgs("std::initializer_list<T> list")]
        private FixedArray(NativeArg5 arg) {
            alloced = true;
            CPP.Add("int size = (int)list.size(); t = (T*)new T[size]; Length = size; T* ptr = (T*)list.begin(); for(int idx=0;idx<size;idx++) {t[idx] = ptr[idx];}");
        }
        public int Length;
        public int Size() {return Length;}
        public Object objRef;
        public bool alloced;
        public T Get(int idx) {
            return (T)CPP.ReturnObject("at(idx)");
        }
        public IEnumerator<T> GetEnumerator() {
            return new FixedArrayEnumerator<T>(this);
        }
        ~FixedArray() {
            if (alloced) {
                CPP.Add("delete[] t;");
            }
        }
    }

    public class FixedArrayEnumerator<T> : IEnumerator<T> {
        public FixedArrayEnumerator(FixedArray<T> array) {
            this.array = array;
        }
        private FixedArray<T> array;
        private int idx = -1;
        public bool MoveNext() {
            if (idx == array.Size()-1) return false;
            idx++;
            return true;
        }
        public T Current {
            get {
                if (idx == -1 || idx == array.Size()) return default(T);
                return array.Get(idx);
            }
        }
        public void Reset() {
            idx = -1;
        }
    }
}
