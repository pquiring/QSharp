using Qt.QSharp;

//this is an internal class and should not be used

namespace Qt.Core {
    [CPPClass(
        "public: MutexHolder(std::shared_ptr<Mutex> mutex) {this->mutex = mutex; mutex->Lock();}\r\n" +
        "public: MutexHolder() {}\r\n"
    )]
    public class MutexHolder {
        private Mutex mutex;
        private bool signal = true;
        public bool Condition() {
            return signal;
        }
        public void Signal() {
            signal = false;
        }
        public Mutex GetMutex() {
            return mutex;
        }
        public void SetMutex(Mutex mutex) {
            this.mutex = mutex;
        }
        ~MutexHolder() {
            CPP.Add("mutex->Unlock();");
        }
    }
}
