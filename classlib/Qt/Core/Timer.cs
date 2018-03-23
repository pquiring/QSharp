using Qt.QSharp;

namespace Qt.Core {
    public delegate void TimerEvent();
    [CPPClass("std::shared_ptr<QTimer> $q;")]
    public class Timer {
        public Timer() {
            CPP.Add("$q = std::make_shared<QTimer>();");
            CPP.Add("QObject::connect($q.get(), &QTimer::timeout, [=] () {this->SlotTimeout();});");
        }
        private TimerEvent handler;
        private void SlotTimeout() {
            try {
                if (handler != null) handler();
            } catch {}
        }
        public void Start(int ms) {
            CPP.Add("$q->start(ms);");
        }
        public void Stop() {
            CPP.Add("$q->stop();");
        }
        public void SetSingleShot(bool once) {
            CPP.Add("$q->setSingleShot(once);");
        }
        public bool IsSingleShot() {
            return CPP.ReturnBool("$q->isSingleShot()");
        }
        public void OnEvent(TimerEvent handler) {
            this.handler = handler;
        }
    }
}
