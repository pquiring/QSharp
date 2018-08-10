struct $LockHolder {
  $LockHolder(Qt::Core::ThreadLock *lock) {this->lock = lock; $check(lock)->Lock();}
  Qt::Core::ThreadLock *lock;
  bool signal = true;
  bool Condition() {return signal;}
  void Signal() {signal = false;}
  ~$LockHolder() {lock->Unlock();}
};
