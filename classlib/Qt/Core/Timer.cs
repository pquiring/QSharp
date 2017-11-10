using Qt.QSharp;

namespace Qt.Core {
    public delegate void TimerEvent();
    [CPPExtends("QTimer")]
    public class Timer {
        private TimerEvent handler;
        private void Timeout() {
            if (handler != null) handler();
        }
        public void Start(int ms) {
            CPP.Add("start(ms);");
        }
        public void Stop() {
            CPP.Add("stop();");
        }
        public void SetSingleShot(bool once) {
            CPP.Add("setSingleShot(once);");
        }
        public bool IsSingleShot() {
            return CPP.ReturnBool("isSingleShot()");
        }
        public void Connect(TimerEvent handler) {
            this.handler = handler;
            CPP.Add("connect(this, &QTimer::timeout, this, &Timer::Timeout);");
        }
    }
}
