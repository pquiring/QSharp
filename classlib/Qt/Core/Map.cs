using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<std::Map<K, V>> $q;"
    )]
    public class Map<K, V> {
        public delegate int SortCompare(K o1, K o2);
        private SortCompare sort;
        public Map(SortCompare cmp) {
            sort = cmp;
            CPP.Add("$q = new std::Map<K, V>([=] (K k1, K k2) {return sort(k1, k2);});");
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
            return (V[])CPP.ReturnObject("$q->values()");
        }
        public K[] Keys() {
            return (K[])CPP.ReturnObject("$q->keys()");
        }
    }
}
