using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QVector<T> *$q;\r\n" + 
        "public: Array() {$q = new QVector<T>();}\r\n"
    )]
    public class Array<T> {
        public void Add(T t) {
            CPP.Add("$q->append(t);");
        }
        public void Insert(T t, int idx) {
            CPP.Add("$q->insert(idx, t);");
        }
        public T Get(int idx) {
            return (T)CPP.ReturnObject("$q->at(idx)");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool Contains(T t) {return CPP.ReturnBool("$q->contains(t)");}
        public void Remove(int idx) {CPP.Add("$q->removeAt(idx);");}
        ~Array() {
            CPP.Add("delete $q;");
        }
    }
}
