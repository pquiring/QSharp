using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::unique_ptr<QList<T>> $q;"
    )]
    /** Stores an array of objects in a linked-list style. */
    public class ArrayList<T> : IEnumerable<T> {
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
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool Contains(T t) {return CPP.ReturnBool("$q->contains(t)");}
        public void Remove(int idx) {CPP.Add("$q->removeAt(idx);");}

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
