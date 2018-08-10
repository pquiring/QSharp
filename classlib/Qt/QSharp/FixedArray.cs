using Qt.Core;

namespace Qt.QSharp {
    [CPPClass(
        "T *t = nullptr;\r\n" +
        "int length = 0;\r\n" +
        "bool alloced = true;\r\n" +
        "T& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return t[pos];}\r\n" +
        "T& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return t[pos];}\r\n" +
        "FixedArray operator=(const FixedArray<T> &o) {t = o.t; return *this;}\r\n" +
        "FixedArray operator=(std::initializer_list<T> list) {length = (int)list.size(); t = new T[length]; const T *src = list.begin(); for(int a=0;a<length;a++) {t[a] = src[a];} return *this;}\r\n" +
        "bool operator==(const FixedArray<T> &o) {return t == o.t;}\r\n" +
        "bool operator!=(const FixedArray<T> &o) {return t != o.t;}\r\n" +
        "bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "FixedArray<T>* operator->() {return this;}\r\n" +
        "operator Qt::Core::String*() {return new Qt::Core::String(\"FixedArray\");}\r\n" +
        "Qt::Core::String* ToString() {return new Qt::Core::String(\"FixedArray\");}\r\n" +
        "T* data() {return t;}\r\n" +
        "FixedArray() {$init(); alloced = false;}\r\n" +
        "FixedArray(nullptr_t np) {$init(); alloced = false;}\r\n" +
        "FixedArray(const FixedArray &o) {$init(); t = o.t; length = o.length; alloced = false;}\r\n" +
        "template<typename T2>\r\n" +
        "FixedArray(const FixedArray<T2> &o) {\r\n" +
        "  $init();\r\n" +
        "  if (o.t == nullptr) return;" +
        "  length = o.length;\r\n" +
        "  t = new T[length];\r\n" +
        "  for(int a=0;a<length;a++) {t[a] = o.t[a];}" +
        "}\r\n"
    )]
    public class FixedArray<T> : IEnumerable<T> {
        public int Length {get {return CPP.ReturnInt("length");}}
        public FixedArray(int size) {
            CPP.Add("$init(); if (size < 0) $abe(); t = new T[size]; length = size; alloced = true; std::memset(t, 0, sizeof(T) * length);");
        }
        [CPPReplaceArgs("T *buf, int size")]
        private FixedArray(NativeArg1 arg, int size) {
            CPP.Add("$init(); if (size < 0) $abe(); t = buf; length = size; alloced = false;");
        }
        [CPPReplaceArgs("T *buf, int size, bool copy")]
        private FixedArray(NativeArg2 arg, int size, bool copy) {
            CPP.Add("$init(); if (size < 0) $abe(); t = new T[size]; length = size; alloced = true; for(int a=0;a<length;a++) {t[a] = buf[a];}");
        }
        [CPPReplaceArgs("std::initializer_list<T> list")]
        private FixedArray(NativeArg5 arg) {
            CPP.Add("$init(); length = (int)list.size(); t = new T[length]; const T *src = list.begin(); for(int a=0;a<length;a++) {t[a] = src[a];}");
        }
        ~FixedArray() {
            CPP.Add("if (alloced) {delete[] t; t = nullptr; alloced = false;}");
        }
        public int Size() {return CPP.ReturnInt("length");}
        public T Get(int idx) {
            return (T)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<T> GetEnumerator() {
            return new FixedArrayEnumerator<T>(this);
        }
    }

    [CPPAddUsage("Qt::QSharp::FixedArray")]
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
