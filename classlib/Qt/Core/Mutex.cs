using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QMutex $q;" +
        "public: Mutex() : $q(QMutex::Recursive) {}"
    )]
    public class Mutex {
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
