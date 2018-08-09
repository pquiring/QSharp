using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<std::Vector<T>> $q;"
    )]
    /** Stores an array of objects in a resizable continuous memory (QVector). */
    public class Array<T> : IEnumerable<T> {
        public delegate int ArraySortCompare(T t1, T t2);
        public Array() {
            CPP.Add("$q = new std::Vector<T>();");
        }
        public void Add(T t) {
            CPP.Add("$q->add(t);");
        }
        public void Add(T[] t) {
            for(int idx=0;idx<t.Length;idx++) {
                CPP.Add("$q->add(t->at(idx));");
            }
        }
        public void Insert(int idx, T t) {
            CPP.Add("$q->add(idx, t);");
        }
        public void Insert(int idx, T[] t) {
            for(int tidx=0;tidx<t.Length;tidx++) {
                CPP.Add("$q->add(idx, t->at(tidx));");
            }
        }
        public T Get(int idx) {
            return (T)CPP.ReturnObject("$q->get(idx)");
        }
        public void Set(int idx, T t) {
            CPP.ReturnObject("$q->set(idx, t);");
        }
        /** Returns unsafe backing buffer. */
        public T[] GetBuffer() {
            return (T[])CPP.ReturnObject("new Qt::QSharp::FixedArray1D<T>($q->get(), $q->size())");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool IsEmpty() {return Size() == 0;}
        public int Count {get {return Size();}}
        public bool Contains(T t) {return CPP.ReturnBool("$q->indexOf(t) != -1");}
        public int IndexOf(T t) {return CPP.ReturnInt("$q->indexOf(t)");}
        public int LastIndexOf(T t) {return CPP.ReturnInt("$q->lastIndexOf(t)");}
        public void RemoveAt(int idx) {CPP.Add("$q->removeAt(idx);");}
        public void Remove(T t) {CPP.Add("$q->removeAt(t);");}
        public void Clear() {CPP.Add("$q->clear();");}
        public void Sort(ArraySortCompare cmp) {
            int length = Size();
            T o1, o2;
            for(int i1=0;i1<length;i1++) {
                for(int i2=i1+1;i2<length;i2++) {
                    o1 = Get(i1);
                    o2 = Get(i2);
                    if (cmp(o1, o2) > 0) {
                        //swap values
                        Set(i1, o2);
                        Set(i2, o1);
                    }
                }
            }
        }
        public T[] ToArray() {
            int size = Size();
            T[] array = new T[size];
            for(int a=0;a<size;a++) {
                array[a] = Get(a);
            }
            return array;
        }
        public IEnumerator<T> GetEnumerator() {
            return new ArrayEnumerator<T>(this);
        }
    }
    public class ArrayEnumerator<T> : IEnumerator<T> {
        public ArrayEnumerator(Array<T> array) {this.array = array;}
        private Array<T> array;
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
