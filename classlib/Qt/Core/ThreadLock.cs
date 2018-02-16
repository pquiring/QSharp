using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassHPP(
        "struct $ThreadLockHolder {" +
        "$ThreadLockHolder(std::shared_ptr<Qt::Core::ThreadLock> lock) {this->lock = lock; lock->Lock();}" +
        "std::shared_ptr<Qt::Core::ThreadLock> lock;" +
        "bool signal = true;" +
        "bool Condition() {return signal;}" +
        "void Signal() {signal = false;}" +
        "~$ThreadLockHolder() {lock->Unlock();}" +
        "};"
    )]
    [CPPClass(
        "private: std::shared_ptr<QMutex> $q;"
    )]
    public class ThreadLock {
        public ThreadLock() {
            CPP.Add("$q = std::make_shared<QMutex>(QMutex::Recursive);");
        }
        public void Lock() {
            CPP.Add("$q->lock();");
        }
        public bool TryLock(int ms) {
            return CPP.ReturnBool("$q->tryLock(ms)");
        }
        public void Unlock() {
            CPP.Add("$q->unlock();");
        }
    }
}
