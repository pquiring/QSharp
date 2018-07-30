using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::mutex $m;" +
        "std::qt_ptr<std::unique_lock<std::mutex>> $q;" +
        "std::condition_variable $w;" +
        "std::unique_lock<std::mutex> *$value() {return $q.get();}"
    )]
    /**
        ThreadLock provides thread concurrent locking with wait conditions.
        The locking is non recursive.
     */
    public class ThreadLock {
        private int count = 0;
        public ThreadLock() {
            CPP.Add("$q = new std::unique_lock<std::mutex>($m, std::defer_lock);");
        }
        /**
        Locks this ThreadLock blocking until this thread has an exclusive lock.
        If this thread already has a lock, it will deadlock.
        */
        public void Lock() {
            if (OwnsLock()) {
                count++;
            } else {
                CPP.Add("$q->lock();");
                count++;
            }
        }
        /**
        Try to lock this ThreadLock returning immediately.
        */
        public bool TryLock() {
            if (OwnsLock()) {
                count++;
                return true;
            }
            bool ret = CPP.ReturnBool("$q->try_lock()");
            if (ret) count++;
            return ret;
        }
        /**
        Unlocks this ThreadLock.
        */
        public void Unlock() {
            count--;
            if (count == 0) {
                CPP.Add("$q->unlock();");
            }
        }
        /**
        Checks if current thread owns the lock.
        */
        public bool OwnsLock() {
            return CPP.ReturnBool("$q->owns_lock()");
        }
        /**
        Wait must be called when the ThreadLock is locked, otherwise behavior is undefined.
        This function will call Unlock() and wait for a WakeOne() or WakeAll() event to wake this thread or until the timeout occurs.
        After this thread is woken or the timeout occurs Lock() is invoked again before returning.
        */
        public void Wait(int timeout = -1) {
            if (timeout == -1) {
                CPP.Add("$w.wait_for(*$q.get(), std::chrono::system_clock::duration::max());");
            } else {
                CPP.Add("$w.wait_for(*$q.get(), std::chrono::milliseconds(timeout));");
            }
        }
        /**
        WakeOne must be called when the ThreadLock is locked, otherwise behavior is undefined.
        This will cause one thread that called Wait() to return once it can perform a Lock().
        */
        public void WakeOne() {
            CPP.Add("$w.notify_one();");
        }
        /**
        WakeAll must be called when the ThreadLock is locked, otherwise behavior is undefined.
        This will cause all threads that called Wait() to return once it can perform a Lock().
        */
        public void WakeAll() {
            CPP.Add("$w.notify_all();");
        }
    }
}
