using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QMap<K, V>> $q;"
    )]
    public class Map<K, V> {
        public Map() {
            CPP.Add("$q = std::make_shared<QMap<K, V>>();");
        }
        public void Set(K k, V v) {
            CPP.Add("$q->insert(k, v);");
        }
        public V Get(K k) {
            return (V)CPP.ReturnObject("$q->value(k)");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool Contains(K k) {return CPP.ReturnBool("$q->contains(k)");}
        public void Remove(K k) {CPP.Add("$q->remove(k);");}
    }
}
