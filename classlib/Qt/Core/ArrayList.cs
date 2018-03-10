using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::unique_ptr<QList<T>> $q;"
    )]
    /** Stores an array of objects in a linked-list style. */
    public class ArrayList<T> : IEnumerable<T> {
        public delegate int ArrayListSortCompare(T t1, T t2);
        public ArrayList() {
            CPP.Add("$q = std::make_unique<QList<T>>();");
        }
        public void Add(T t) {
            CPP.Add("$q->append(t);");
        }
        public void Insert(int idx, T t) {
            CPP.Add("$q->insert(idx, t);");
        }
        public T Get(int idx) {
            return (T)CPP.ReturnObject("$q->value(idx)");
        }
        public void Set(int idx, T t) {
            CPP.Add("$q->replace(idx, t);");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool IsEmpty() {return Size() == 0;}
        public bool Contains(T t) {return CPP.ReturnBool("$q->contains(t)");}
        public int IndexOf(T t) {return CPP.ReturnInt("$q->indexOf(t)");}
        public int LastIndexOf(T t) {return CPP.ReturnInt("$q->lastIndexOf(t)");}
        public void RemoveAt(int idx) {CPP.Add("$q->removeAt(idx);");}
        public void Remove(T t) {CPP.Add("$q->removeOne(t);");}
        public void Clear() {CPP.Add("$q->clear();");}
        public void Sort(ArrayListSortCompare cmp) {
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
            return new ArrayListEnumerator<T>(this);
        }
    }

    public class ArrayListEnumerator<T> : IEnumerator<T> {
        public ArrayListEnumerator(ArrayList<T> list) {this.list = list;}
        private ArrayList<T> list;
        private int idx = -1;
        public bool MoveNext() {
            if (idx == list.Size()-1) return false;
            idx++;
            return true;
        }
        public T Current {
            get {
                if (idx == -1 || idx == list.Size()) return default(T);
                return list.Get(idx);
            }
        }
        public void Reset() {
            idx = -1;
        }
    }
}
