template <class K, class V>
Qt::QSharp::FixedArray<K>* std::Map<K,V>::keys() {
  int size = pairs.size();
  Qt::QSharp::FixedArray<K>* ks = new Qt::QSharp::FixedArray<K>(size);
  for(int a=0;a<size;a++) {
    Pair *p = pairs.get(a);
    ks->at(a) = p->k;
  }
  return ks;
}

template <class K, class V>
Qt::QSharp::FixedArray<V>* std::Map<K,V>::values() {
  int size = pairs.size();
  Qt::QSharp::FixedArray<V>* vs = new Qt::QSharp::FixedArray<V>(size);
  for(int a=0;a<size;a++) {
    Pair *p = pairs.get(a);
    vs->at(a) = p->v;
  }
  return vs;
}
