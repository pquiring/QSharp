using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QMutex> $q;" +
        "public: std::shared_ptr<QMutex> $value() {return $q;}"
    )]
    /**
        ThreadLock provides basic thread locking.
        The locking is recursive.
     */
    public class ThreadLock {
        public ThreadLock() {
            CPP.Add("$q = std::make_shared<QMutex>(QMutex::Recursive);");
        }
        /**
        Locks this ThreadLock blocking until this thread has an exclusive lock.
        If this thread already has a lock, it will increment the thread lock count.
        */
        public void Lock() {
            CPP.Add("$q->lock();");
        }
        /**
        Try to lock this ThreadLock blocking until this thread has an exclusive lock or the timeout expires.
        */
        public bool TryLock(int ms) {
            return CPP.ReturnBool("$q->tryLock(ms)");
        }
        /**
        Unlocks this ThreadLock.
        If this thread already has a lock, it will decrement the thread lock count.
        */
        public void Unlock() {
            CPP.Add("$q->unlock();");
        }
    }
}
