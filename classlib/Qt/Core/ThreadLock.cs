using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassHPP(
        "struct $ThreadLockHolder {" +
        "$ThreadLockHolder(std::shared_ptr<ThreadLock> lock) {this->lock = lock; lock->Lock();}" +
        "std::shared_ptr<ThreadLock> lock;" +
        "bool signal = true;" +
        "bool Condition() {return signal;}" +
        "void Signal() {signal = false;}" +
        "~$ThreadLockHolder() {lock->Unlock();}" +
        "};"
    )]
    [CPPClass(
        "private: QMutex $q;" +
        "public: ThreadLock() : $q(QMutex::Recursive) {}"
    )]
    public class ThreadLock {
        public void Lock() {
            CPP.Add("$q.lock();");
        }
        public bool TryLock(int ms) {
            return CPP.ReturnBool("$q.tryLock(ms)");
        }
        public void Unlock() {
            CPP.Add("$q.unlock();");
        }
    }
}
