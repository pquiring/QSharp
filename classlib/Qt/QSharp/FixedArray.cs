using Qt.Core;

namespace Qt.QSharp {
    [CPPClass(
        "std::qt_ptr<FixedData<T>> t;\r\n" +
        "T& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "T& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "bool operator==(const FixedArray1D<T> &o) {return t == o.t;}\r\n" +
        "bool operator!=(const FixedArray1D<T> &o) {return t != o.t;}\r\n" +
        "bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "FixedArray1D<T>* operator->() {return this;}\r\n" +
        "operator std::gc_ptr<Qt::Core::String>() {return Qt::Core::String::$new(\"FixedArray1D\");}\r\n" +
        "std::gc_ptr<Qt::Core::String> ToString() {return Qt::Core::String::$new(\"FixedArray1D\");}\r\n" +
        "T* data() {return t.get()->t;}\r\n" +
        "FixedArray1D() {$init();}\r\n" +
        "FixedArray1D(nullptr_t np) {$init();}\r\n" +
        "FixedArray1D(const FixedArray1D &o) {$init(); t = o.t;}\r\n" +
        "template<typename T2>\r\n" +
        "FixedArray1D(const FixedArray1D<T2> &o) {\r\n" +
        "  $init();\r\n" +
        "  if (o.t == nullptr) return;" +
        "  t = new FixedData<T>(o.t->length);\r\n" +
        "  for(int a=0;a<t->length;a++) {t->t[a] = o.t->t[a];}" +
        "}\r\n"
    )]
    public class FixedArray1D<T> : IEnumerable<T> {
        public int Length {get {return CPP.ReturnInt("t->length");}}
        public FixedArray1D(int size) {
            CPP.Add("if (size < 0) $abe(); t = new FixedData<T>(size);");
        }
        [CPPReplaceArgs("T *buf, int size")]
        private FixedArray1D(NativeArg1 arg, int size) {
            CPP.Add("t = new FixedData<T>(buf, size);");
        }
        [CPPReplaceArgs("T *buf, int size, bool copy")]
        private FixedArray1D(NativeArg2 arg, int size, bool copy) {
            CPP.Add("t = new FixedData<T>(buf, size, copy);");
        }
        [CPPReplaceArgs("std::gc_ptr<Qt::Core::Object> objRef, T *buf, int size")]
        private FixedArray1D(Object obj, NativeArg3 arg, int size) {
            CPP.Add("t = new FixedData<T>(objRef, buf, size);");
        }
        [CPPReplaceArgs("std::gc_ptr<Qt::Core::Object> objRef, T *buf, int size, bool copy")]
        private FixedArray1D(Object obj, NativeArg4 arg, int size, bool copy) {
            CPP.Add("t = new FixedData<T>(objRef, buf, size, copy);");
        }
        [CPPReplaceArgs("std::initializer_list<T> list")]
        private FixedArray1D(NativeArg5 arg) {
            CPP.Add("t = new FixedData<T>(list);");
        }
        public int Size() {return Length;}
        public T Get(int idx) {
            return (T)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<T> GetEnumerator() {
            return (IEnumerator<T>)new FixedArray1DEnumerator<T>(this);  //BUG : unneeded typecast
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
        "std::qt_ptr<FixedData<std::gc_ptr<FixedArray1D<T>>>> t;\r\n" +
        "std::gc_ptr<FixedArray1D<T>>& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "std::gc_ptr<FixedArray1D<T>>& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "bool operator==(const std::gc_ptr<FixedArray2D<T>> &o) {return t == o.t;}\r\n" +
        "bool operator!=(const std::gc_ptr<FixedArray2D<T>> &o) {return t != o.t;}\r\n" +
        "bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "FixedArray2D<T>* operator->() {return this;}\r\n" +
        "operator std::gc_ptr<Qt::Core::String>() {return Qt::Core::String::$new(\"FixedArray2D\");}\r\n" +
        "std::gc_ptr<Qt::Core::String> ToString() {return Qt::Core::String::$new(\"FixedArray2D\");}\r\n" +
        "FixedArray2D() {$init();}\r\n" +
        "FixedArray2D(nullptr_t np) {$init();}\r\n" +
        "FixedArray2D(const std::gc_ptr<FixedArray2D<T>> &o) {$init(); t = o.t;}\r\n" +
        "template<typename T2>\r\n" +
        "FixedArray2D(const std::gc_ptr<FixedArray2D<T2>> &o) {\r\n" +
        "  $init();\r\n" +
        "  if (o.t == nullptr) return;" +
        "  t = new FixedData<std::gc_ptr<FixedArray1D<T>>>(o.t->length);\r\n" +
        "  for(int a=0;a<t->length;a++) {t->t[a] = o.t->t[a];}" +
        "}\r\n"
    )]
    public class FixedArray2D<T> : IEnumerable<FixedArray1D<T>> {
        public int Length {get {return CPP.ReturnInt("t->length");}}
        public FixedArray2D(int size) {
            CPP.Add("if (size < 0) $abe(); t = new FixedData<std::gc_ptr<FixedArray1D<T>>>(size);\r\n");
        }
        [CPPReplaceArgs("std::initializer_list<FixedArray1D<T>> list")]
        private FixedArray2D(NativeArg5 arg) {
            CPP.Add("t = new FixedData<std::gc_ptr<FixedArray1D<T>>>(list);");
        }
        public int Size() {return Length;}
        public FixedArray1D<T> Get(int idx) {
            return (FixedArray1D<T>)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<FixedArray1D<T>> GetEnumerator() {
            return (IEnumerator<FixedArray1D<T>>)new FixedArray2DEnumerator<T>(this);  //BUG : unneeded typecast
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
        "std::qt_ptr<FixedData<std::gc_ptr<FixedArray2D<T>>>> t;\r\n" +
        "std::gc_ptr<FixedArray2D<T>>& operator[](int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "std::gc_ptr<FixedArray2D<T>>& at(int pos) {if (pos < 0 || pos > Size()) $abe(); return (*t.get())[pos];}\r\n" +
        "bool operator==(const std::gc_ptr<FixedArray3D<T>> &o) {return t == o.t;}\r\n" +
        "bool operator!=(const std::gc_ptr<FixedArray3D<T>> &o) {return t != o.t;}\r\n" +
        "bool operator==(nullptr_t np) {return t == nullptr;}\r\n" +
        "bool operator!=(nullptr_t np) {return t != nullptr;}\r\n" +
        "FixedArray3D<T>* operator->() {return this;}\r\n" +
        "operator std::gc_ptr<Qt::Core::String>() {return Qt::Core::String::$new(\"FixedArray3D\");}\r\n" +
        "std::gc_ptr<Qt::Core::String> ToString() {return Qt::Core::String::$new(\"FixedArray3D\");}\r\n" +
        "FixedArray3D() {$init();}\r\n" +
        "FixedArray3D(nullptr_t np) {$init();}\r\n" +
        "FixedArray3D(const std::gc_ptr<FixedArray3D<T>> &o) {$init();t = o.t;}\r\n" +
        "template<typename T2>\r\n" +
        "FixedArray3D(const std::gc_ptr<FixedArray3D<T2>> &o) {\r\n" +
        "  $init();\r\n" +
        "  if (o.t == nullptr) return;" +
        "  t = new FixedData<std::gc_ptr<FixedArray2D<T>>>(o.t->length);\r\n" +
        "  for(int a=0;a<t->length;a++) {t->t[a] = o.t->t[a];}" +
        "}\r\n"
    )]
    public class FixedArray3D<T> : IEnumerable<FixedArray2D<T>> {
        public int Length {get {return CPP.ReturnInt("t->length");}}
        public FixedArray3D(int size) {
            CPP.Add("if (size < 0) $abe(); t = new FixedData<<std::gc_ptr<FixedArray2D<T>>(size);");
        }
        [CPPReplaceArgs("std::initializer_list<FixedArray2D<T>> list")]
        private FixedArray3D(NativeArg5 arg) {
            CPP.Add("t = new FixedData<std::gc_ptr<FixedArray2D<T>>>(list);");
        }
        public int Size() {return Length;}
        public FixedArray2D<T> Get(int idx) {
            return (FixedArray2D<T>)CPP.ReturnObject("at(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("at(idx) = t;");
        }
        public IEnumerator<FixedArray2D<T>> GetEnumerator() {
            return (IEnumerator<FixedArray2D<T>>)new FixedArray3DEnumerator<T>(this);  //BUG : unneeded typecast
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

    //just to insert HPP code after FixedArrays
    public class FixedChecks {}
}
