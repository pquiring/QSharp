//String $add()

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::Object> y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append($deref(y)->ToString());
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, int32 y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(Qt::Core::Int32::ToString(y));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, int64 y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(Qt::Core::Int64::ToString(y));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, uint32 y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(Qt::Core::UInt32::ToString(y));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, uint64 y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(Qt::Core::UInt64::ToString(y));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, float y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(Qt::Core::Float::ToString(y));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, double y) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(s1);
  s->Append(Qt::Core::Double::ToString(y));
  return s;
}


std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::Object> x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append($deref(x)->ToString());
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(int32 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(Qt::Core::Int32::ToString(x));
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(int64 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(Qt::Core::Int64::ToString(x));
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(uint32 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(Qt::Core::UInt32::ToString(x));
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(uint64 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(Qt::Core::UInt64::ToString(x));
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(float x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(Qt::Core::Float::ToString(x));
  s->Append(s2);
  return s;
}

std::shared_ptr<Qt::Core::String> $add(double x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = Qt::Core::String::$new();
  s->Append(Qt::Core::Double::ToString(x));
  s->Append(s2);
  return s;
}

//QStringList <-> String[] conversions

std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<Qt::Core::String>>> $QStringListToStringArray(QStringList list) {
  std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<Qt::Core::String>>> array;
  int cnt = list.count();
  array = std::make_shared<Qt::QSharp::FixedArray<std::shared_ptr<Qt::Core::String>>>(cnt);
  for(int idx=0;idx<cnt;idx++) {array->at(idx) = Qt::Core::String::$new(list[idx]);}
  return array;
}

QStringList $StringArrayToQStringList(std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<Qt::Core::String>>> array) {
  QStringList list;
  int cnt = array->Length;
  for(int idx=0;idx<cnt;idx++) {list.append(array->at(idx)->qstring());}
  return list;
}
