using Qt.Core;

namespace Qt.QSharp {
    [CPPClass(
        "public: std::shared_ptr<FixedData<T>> t;\r\n" +
        "public: T& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "public: T& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "public: bool operator==(FixedArray1D<T> o) {return t == o.t;}\r\n" +
        "public: bool operator!=(FixedArray1D<T> o) {return t != o.t;}\r\n" +
        "public: bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "public: bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "public: FixedArray1D<T>* operator->() {return this;}\r\n" +
        "public: T* data() {return t.get()->t;}\r\n" +
        "public: FixedArray1D() {$init();}\r\n" +
        "public: FixedArray1D(nullptr_t np) {$init();}\r\n" +
        "public: FixedArray1D(const FixedArray1D &o) {$init(); t = o.t;}\r\n" +
        "public: template<typename T2>\r\n" +
        "        FixedArray1D(const FixedArray1D<T2> &o) {\r\n" +
        "          $init();\r\n" +
        "          if (o.t == nullptr) return;" +
        "          t = std::make_shared<FixedData<T>>(o.t->length);\r\n" +
        "          for(int a=0;a<t->length;a++) {t->t[a] = o.t->t[a];}" +
        "        }\r\n"
    )]
    public class FixedArray1D<T> : IEnumerable<T> {
        public int Length {get {return CPP.ReturnInt("t->length");}}
        public FixedArray1D(int size) {
            CPP.Add("if (size < 0) $abe(); t = std::make_shared<FixedData<T>>(size);");
        }
        [CPPReplaceArgs("T *buf, int size")]
        private FixedArray1D(NativeArg1 arg, int size) {
            CPP.Add("t = std::make_shared<FixedData<T>>(buf, size);");
        }
        [CPPReplaceArgs("T *buf, int size, bool copy")]
        private FixedArray1D(NativeArg2 arg, int size, bool copy) {
            CPP.Add("t = std::make_shared<FixedData<T>>(buf, size, copy);");
        }
        [CPPReplaceArgs("std::shared_ptr<Qt::Core::Object> objRef, T *buf, int size")]
        private FixedArray1D(Object obj, NativeArg3 arg, int size) {
            CPP.Add("t = std::make_shared<FixedData<T>>(objRef, buf, size);");
        }
        [CPPReplaceArgs("std::shared_ptr<Qt::Core::Object> objRef, T *buf, int size, bool copy")]
        private FixedArray1D(Object obj, NativeArg4 arg, int size, bool copy) {
            CPP.Add("t = std::make_shared<FixedData<T>>(objRef, buf, size, copy);");
        }
        [CPPReplaceArgs("std::initializer_list<T> list")]
        private FixedArray1D(NativeArg5 arg) {
            CPP.Add("t = std::make_shared<FixedData<T>>(list);");
        }
        public int Size() {return Length;}
        public T Get(int idx) {
            return (T)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<T> GetEnumerator() {
            return new FixedArray1DEnumerator<T>(this);
        }
    }

    [CPPAddUsage("Qt::QSharp::FixedArray1D")]
    public class FixedArray1DEnumerator<T> : IEnumerator<T> {
        public FixedArray1DEnumerator(FixedArray1D<T> array) {
            this.array = array;
        }
        private FixedArray1D<T> array;
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

    [CPPClass(
        "public: std::shared_ptr<FixedData<FixedArray1D<T>>> t;\r\n" +
        "public: FixedArray1D<T>& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "public: FixedArray1D<T>& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "public: bool operator==(FixedArray2D<T> o) {return t == o.t;}\r\n" +
        "public: bool operator!=(FixedArray2D<T> o) {return t != o.t;}\r\n" +
        "public: bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "public: bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "public: FixedArray2D<T>* operator->() {return this;}\r\n" +
        "public: FixedArray2D() {$init();}\r\n" +
        "public: FixedArray2D(nullptr_t np) {$init();}\r\n" +
        "public: FixedArray2D(const FixedArray2D &o) {$init(); t = o.t;}\r\n" +
        "public: template<typename T2>\r\n" +
        "        FixedArray2D(const FixedArray2D<T2> &o) {\r\n" +
        "          $init();\r\n" +
        "          if (o.t == nullptr) return;" +
        "          t = std::make_shared<FixedData<FixedArray1D<T>>>(o.t->length);\r\n" +
        "          for(int a=0;a<t->length;a++) {t->t[a] = o.t->t[a];}" +
        "        }\r\n"
    )]
    public class FixedArray2D<T> : IEnumerable<FixedArray1D<T>> {
        public int Length {get {return CPP.ReturnInt("t->length");}}
        public FixedArray2D(int size) {
            CPP.Add("if (size < 0) $abe(); t = std::make_shared<FixedData<FixedArray1D<T>>>(size);\r\n");
        }
        public int Size() {return Length;}
        public FixedArray1D<T> Get(int idx) {
            return (FixedArray1D<T>)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<FixedArray1D<T>> GetEnumerator() {
            return new FixedArray2DEnumerator<T>(this);
        }
    }

    [CPPAddUsage("Qt::QSharp::FixedArray2D")]
    public class FixedArray2DEnumerator<T> : IEnumerator<FixedArray1D<T>> {
        public FixedArray2DEnumerator(FixedArray2D<T> array) {
            this.array = array;
        }
        private FixedArray2D<T> array;
        private int idx = -1;
        public bool MoveNext() {
            if (idx == array.Size()-1) return false;
            idx++;
            return true;
        }
        public FixedArray1D<T> Current {
            get {
                if (idx == -1 || idx == array.Size()) return default(FixedArray1D<T>);
                return array.Get(idx);
            }
        }
        public void Reset() {
            idx = -1;
        }
    }

    [CPPClass(
        "public: std::shared_ptr<FixedData<FixedArray2D<T>>> t;\r\n" +
        "public: FixedArray2D<T>& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "public: FixedArray2D<T>& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "public: bool operator==(FixedArray3D<T> o) {return t == o.t;}\r\n" +
        "public: bool operator!=(FixedArray3D<T> o) {return t != o.t;}\r\n" +
        "public: bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "public: bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "public: FixedArray3D<T>* operator->() {return this;}\r\n" +
        "public: FixedArray3D() {$init();}\r\n" +
        "public: FixedArray3D(nullptr_t np) {$init();}\r\n" +
        "public: FixedArray3D(const FixedArray3D &o) {$init();t = o.t;}\r\n" +
        "public: template<typename T2>\r\n" +
        "        FixedArray3D(const FixedArray3D<T2> &o) {\r\n" +
        "          $init();\r\n" +
        "          if (o.t == nullptr) return;" +
        "          t = std::make_shared<FixedData<FixedArray2D<T>>>(o.t->length);\r\n" +
        "          for(int a=0;a<t->length;a++) {t->t[a] = o.t->t[a];}" +
        "        }\r\n"
    )]
    public class FixedArray3D<T> : IEnumerable<FixedArray2D<T>> {
        public int Length {get {return CPP.ReturnInt("t->length");}}
        public FixedArray3D(int size) {
            CPP.Add("if (size < 0) $abe(); t = std::make_shared<FixedData<FixedArray2D<T>>(size);");
        }
        public int Size() {return Length;}
        public FixedArray2D<T> Get(int idx) {
            return (FixedArray2D<T>)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<FixedArray2D<T>> GetEnumerator() {
            return new FixedArray3DEnumerator<T>(this);
        }
    }

    [CPPAddUsage("Qt::QSharp::FixedArray3D")]
    public class FixedArray3DEnumerator<T> : IEnumerator<FixedArray2D<T>> {
        public FixedArray3DEnumerator(FixedArray3D<T> array) {
            this.array = array;
        }
        private FixedArray3D<T> array;
        private int idx = -1;
        public bool MoveNext() {
            if (idx == array.Size()-1) return false;
            idx++;
            return true;
        }
        public FixedArray2D<T> Current {
            get {
                if (idx == -1 || idx == array.Size()) return default(FixedArray2D<T>);
                return array.Get(idx);
            }
        }
        public void Reset() {
            idx = -1;
        }
    }
}
