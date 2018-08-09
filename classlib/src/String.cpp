//String $addstr()

Qt::Core::String* $addstr(Qt::Core::String* s1, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, Qt::Core::Object* y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(y->ToString());
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, char16 y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(y);
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, int32 y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(Qt::Core::Int32::ToString(y));
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, int64 y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(Qt::Core::Int64::ToString(y));
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, uint32 y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(Qt::Core::UInt32::ToString(y));
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, uint64 y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(Qt::Core::UInt64::ToString(y));
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, float y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(Qt::Core::Float::ToString(y));
  return s;
}

Qt::Core::String* $addstr(Qt::Core::String* s1, double y) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(s1);
  s->Append(Qt::Core::Double::ToString(y));
  return s;
}


Qt::Core::String* $addstr(Qt::Core::Object* x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(x->ToString());
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(int32 x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(Qt::Core::Int32::ToString(x));
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(int64 x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(Qt::Core::Int64::ToString(x));
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(uint32 x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(Qt::Core::UInt32::ToString(x));
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(uint64 x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(Qt::Core::UInt64::ToString(x));
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(float x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(Qt::Core::Float::ToString(x));
  s->Append(s2);
  return s;
}

Qt::Core::String* $addstr(double x, Qt::Core::String* s2) {
  Qt::Core::String* s = new Qt::Core::String();
  s->Append(Qt::Core::Double::ToString(x));
  s->Append(s2);
  return s;
}

//QStringList <-> String[] conversions

Qt::QSharp::FixedArray1D<Qt::Core::String*>* $QStringListToStringArray(QStringList list) {
  int cnt = list.count();
  Qt::QSharp::FixedArray1D<Qt::Core::String*>* array = new Qt::QSharp::FixedArray1D<Qt::Core::String*>(cnt);
  for(int idx=0;idx<cnt;idx++) {array->at(idx) = new Qt::Core::String(list[idx]);}
  return array;
}

QStringList $StringArrayToQStringList(Qt::QSharp::FixedArray1D<Qt::Core::String*>* array) {
  QStringList list;
  int cnt = array->Length;
  for(int idx=0;idx<cnt;idx++) {list.append(array->at(idx)->qstring());}
  return list;
}
