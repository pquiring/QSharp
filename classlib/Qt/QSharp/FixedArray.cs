using Qt.Core;

namespace Qt.QSharp {
    [CPPNonClassHPP(
        "template<typename T>" +
        "void $checkArray(std::shared_ptr<Qt::QSharp::FixedArray<T>> array, int offset, int length)" +
        "{if (offset + length > array->Length) $abe();}"
    )]
    [CPPClass(
        "public: T *t;\r\n" +
        "public: FixedArray(int size) {if (size < 0) $abe(); t = (T*)new T[size]; std::memset(t, 0, size * sizeof(T)); Length = size; alloced = true; objRef = nullptr;}\r\n" +
        "public: FixedArray(void *buf, int size) {t = (T*)buf; Length = size; alloced = false;}\r\n" +
        "public: FixedArray(void *buf, int size, bool copy) {if (copy) {t = (T*)new T[size]; std::memcpy(t, buf, size * sizeof(T));} else {t = (T*)buf;} Length = size; alloced = copy;}\r\n" +
        "public: FixedArray(std::shared_ptr<Qt::Core::Object> objRef, void *buf, int size) {t = (T*)buf; Length = size; alloced = false; this->objRef = objRef;}\r\n" +
        "public: FixedArray(std::shared_ptr<Qt::Core::Object> objRef, void *buf, int size, bool copy) {if (copy) {t = (T*)new T[size]; std::memcpy(t, buf, size * sizeof(T));} else {t = (T*)buf;} Length = size; alloced = copy; this->objRef = objRef;}\r\n" +
        "public: FixedArray(std::initializer_list<T> list) {int size = (int)list.size(); t = (T*)new T[size]; Length = size; T* ptr = (T*)list.begin(); for(int idx=0;idx<size;idx++) {t[idx] = ptr[idx];} alloced = true; }\r\n" +
        "public: T& operator[](int pos) {if (pos < 0 || pos > Length) $abe(); return t[pos];}\r\n" +
        "public: T* data() {return t;}\r\n" +
        "public: T& at(int pos) {if (pos < 0 || pos > Length) $abe(); return t[pos];}\r\n" +
        "public: int $get_Length() {return Length;}\r\n"
    )]
    public class FixedArray<T> : IEnumerable<T> {
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
    };

    public class FixedArrayEnumerator<T> : IEnumerator<T> {
        public FixedArrayEnumerator(FixedArray<T> array) {
            this.array = array;
        }
        private FixedArray<T> array;
        private int idx = -1;
        public bool MoveNext() {
            if (idx == array.Size()) return false;
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
