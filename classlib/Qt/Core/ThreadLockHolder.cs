using Qt.QSharp;

//this is an internal class and should not be used

namespace Qt.Core {
    [CPPClass(
        "public: ThreadLockHolder(std::shared_ptr<ThreadLock> threadlock) {this->threadlock = threadlock; threadlock->Lock();}\r\n" +
        "public: ThreadLockHolder() {}\r\n"
    )]
    public class ThreadLockHolder {
        private ThreadLock threadlock;
        private bool done;
        public bool IsDone() {
            return done;
        }
        public void Signal() {
            done = true;
        }
        public ThreadLock GetThreadLock() {
            return threadlock;
        }
        public void SetThreadLock(ThreadLock threadlock) {
            this.threadlock = threadlock;
        }
    }
}
