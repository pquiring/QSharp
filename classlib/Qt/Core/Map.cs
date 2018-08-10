using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QMap<K, V>> $q;"
    )]
    public class Map<K, V> {
        public Map() {
            CPP.Add("$q = new QMap<K, V>();");
        }
        public void Set(K k, V v) {
            CPP.Add("$q->insert(k, v);");
        }
        public V Get(K k) {
            return (V)CPP.ReturnObject("$q->value(k)");
        }
        public int Size() {return CPP.ReturnInt("$q->size()");}
        public bool IsEmpty() {return Size() == 0;}
        public bool Contains(K k) {return CPP.ReturnBool("$q->contains(k)");}
        public void Remove(K k) {CPP.Add("$q->remove(k);");}
        public void Clear() {CPP.Add("$q->clear();");}
        public V[] Values() {
            V[] values = null;
            CPP.Add("QList<V> list = $q->values();");
            CPP.Add("int cnt = list.size();");
            CPP.Add("values = new Qt::QSharp::FixedArray<V>(cnt);");
            CPP.Add("for(int i=0;i<cnt;i++) {values->at(i) = list.at(i);}");
            return values;
        }
        public K[] Keys() {
            K[] keys = null;
            CPP.Add("QList<V> list = $q->keys();");
            CPP.Add("int cnt = list.size();");
            CPP.Add("keys = new Qt::QSharp::FixedArray<K>(cnt);");
            CPP.Add("for(int i=0;i<cnt;i++) {keys->at(i) = list.at(i);}");
            return keys;
        }
    }
}
