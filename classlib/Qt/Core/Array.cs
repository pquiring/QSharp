using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QVector<T> *$q;\r\n" +
        "public: Array() {$q = new QVector<T>();}\r\n"
    )]
    public class Array<T> : IEnumerable<T> {
        public void Add(T t) {
            CPP.Add("$q->append(t);");
        }
        public void Insert(int idx, T t) {
            CPP.Add("$q->insert(idx, t);");
        }
        public T Get(int idx) {
            return (T)CPP.ReturnObject("$q->at(idx)");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool Contains(T t) {return CPP.ReturnBool("$q->contains(t)");}
        public void Remove(int idx) {CPP.Add("$q->removeAt(idx);");}
        public IEnumerator<T> GetEnumerator() {
            return new ArrayEnumerator<T>(this);
        }
        ~Array() {
            CPP.Add("delete $q;");
        }
    }
    public class ArrayEnumerator<T> : IEnumerator<T> {
        public ArrayEnumerator(Array<T> array) {this.array = array;}
        private Array<T> array;
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