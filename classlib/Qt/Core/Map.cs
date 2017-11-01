using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QMap<K, V> *$q;\r\n" + 
        "public: Map() {$q = new QMap<K, V>();}\r\n"
    )]
    public class Map<K, V> {
        public void Set(K k, V v) {
            CPP.Add("$q->insert(k, v);");
        }
        public V Get(K k) {
            return (V)CPP.ReturnObject("$q->value(k)");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool Contains(K k) {return CPP.ReturnBool("$q->contains(k)");}
        public void Remove(K k) {CPP.Add("$q->remove(k);");}
        ~Map() {
            CPP.Add("delete $q;");
        }
    }
}
