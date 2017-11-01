using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QList<T> *$q;\r\n" + 
        "public: List() {$q = new QList<T>();}\r\n"
    )]
    public class List<T> {
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
        ~List() {
            CPP.Add("delete $q;");
        }
    }
}
