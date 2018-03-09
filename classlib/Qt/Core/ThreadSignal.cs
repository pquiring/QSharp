using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QMutex> $q;" +
        "private: std::shared_ptr<QWaitCondition> $w;" +
        "public: std::shared_ptr<QMutex> $value() {return $q;}"
    )]
    /**
        ThreadSignal provides thread locking with wait conditions.
        The locking is non recursive.
     */
    public class ThreadSignal {
        public ThreadSignal() {
            CPP.Add("$q = std::make_shared<QMutex>(QMutex::NonRecursive);");
            CPP.Add("$w = std::make_shared<QWaitCondition>();");
        }
        /**
        Locks this ThreadLock blocking until this thread has an exclusive lock.
        If this thread already has a lock, it will deadlock.
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
        */
        public void Unlock() {
            CPP.Add("$q->unlock();");
        }
        /**
        Wait must be called when the ThreadSignal is locked, otherwise behavior is undefined.
        This function will call Unlock() and wait for a WakeOne() or WakeAll() event to wake this thread or until the timeout occurs.
        After this thread is woken or the timeout occurs Lock() is invoked again before returning.
        */
        public void Wait(int timeout = -1) {
            CPP.Add("$w->wait($q.get(), timeout);");
        }
        /**
        WakeOne must be called when the ThreadSignal is locked, otherwise behavior is undefined.
        This will cause one thread that called Wait() to return once it can perform a Lock().
        */
        public void WakeOne() {
            CPP.Add("$w->wakeOne();");
        }
        /**
        WakeAll must be called when the ThreadSignal is locked, otherwise behavior is undefined.
        This will cause all threads that called Wait() to return once it can perform a Lock().
        */
        public void WakeAll() {
            CPP.Add("$w->wakeAll();");
        }
    }
}
