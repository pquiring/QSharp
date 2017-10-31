std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::Object> y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(y.get()->ToString()->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, int32 y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(Qt::Core::Int32::ToString(y)->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, int64 y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(Qt::Core::Int64::ToString(y)->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, uint32 y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(Qt::Core::UInt32::ToString(y)->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, uint64 y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(Qt::Core::UInt64::ToString(y)->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, float y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(Qt::Core::Float::ToString(y)->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::String> s1, double y) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(s1->$q));
  s->$q->append(*(Qt::Core::Double::ToString(y)->$q));
  return s;
}


std::shared_ptr<Qt::Core::String> $add(std::shared_ptr<Qt::Core::Object> x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(x.get()->ToString()->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(int32 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(Qt::Core::Int32::ToString(x)->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(int64 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(Qt::Core::Int64::ToString(x)->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(uint32 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(Qt::Core::UInt32::ToString(x)->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(uint64 x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(Qt::Core::UInt64::ToString(x)->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(float x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(Qt::Core::Float::ToString(x)->$q));
  s->$q->append(*(s2->$q));
  return s;
}

std::shared_ptr<Qt::Core::String> $add(double x, std::shared_ptr<Qt::Core::String> s2) {
  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();
  s->$q->append(*(Qt::Core::Double::ToString(x)->$q));
  s->$q->append(*(s2->$q));
  return s;
}


