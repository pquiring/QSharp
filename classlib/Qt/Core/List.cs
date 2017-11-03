using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QList<T> *$q;\r\n" +
        "public: List() {$q = new QList<T>();}\r\n"
    )]
    public class List<T> : IEnumerable<T> {
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
            return new ListEnumerator<T>(this);
        }

        ~List() {
            CPP.Add("delete $q;");
        }
    }

    public class ListEnumerator<T> : IEnumerator<T> {
        public ListEnumerator(List<T> list) {this.list = list;}
        private List<T> list;
        private int idx = -1;
        public bool MoveNext() {
            if (idx == list.Size()) return false;
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
