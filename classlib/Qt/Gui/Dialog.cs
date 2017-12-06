using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum DialogResult {Rejected, Accepted}
    public delegate void AcceptedEvent();
    public delegate void FinishedEvent(int result);
    public delegate void RejectedEvent();
    [CPPClass(
        "private: std::shared_ptr<QDialog> $q;" +
        "public: void $base(std::shared_ptr<QDialog> $d) {$q = $d; Widget::$base($q.get());}"
    )]
    public class Dialog : Widget {
        protected Dialog(Derived derived) : base(Derived.derived) {}
        public Dialog() : base(Derived.derived) {
            CPP.Add("$q = std::make_shared<QDialog>();");
            CPP.Add("Widget::$base($q.get());");
        }
        public bool IsModal() {
            return CPP.ReturnBool("$q->isModal()");
        }
        public void SetModal(bool modal) {
            CPP.Add("$q->setModal(modal);");
        }
        public int Exec() {
            return CPP.ReturnInt("$q->exec();");
        }
        public int GetResult() {
            return CPP.ReturnInt("$q->result()");
        }
        public void SetResult(int result) {
            CPP.Add("$q->setResult(result);");
        }
        public void Accept() {
            CPP.Add("$q->accept();");
        }
        public void Done(int result) {
            CPP.Add("$q->done(result);");
        }
        public void Open() {
            CPP.Add("$q->open();");
        }
        public void Reject() {
            CPP.Add("$q->reject();");
        }
        public NativeWindow GetNativeWindow() {
            CPP.Add("std::shared_ptr<NativeWindow> window;");
            CPP.Add("window = std::make_shared<NativeWindow>($q->windowHandle());");
            return (NativeWindow)CPP.ReturnObject("window");
        }

        private AcceptedEvent accepted;
        private void SlotAccepted() {
            if (accepted != null) accepted();
        }
        public void OnAccepted(AcceptedEvent accepted) {
            this.accepted = accepted;
            CPP.Add("$q->connect($q.get(), &QDialog::accepted, this, &Dialog::SlotAccepted);");
        }

        private FinishedEvent finished;
        private void SlotFinished(int result) {
            if (finished != null) finished(result);
        }
        public void OnFinished(FinishedEvent finished) {
            this.finished = finished;
            CPP.Add("$q->connect($q.get(), &QDialog::finished, this, &Dialog::SlotFinished);");
        }

        private RejectedEvent rejected;
        private void SlotRejected() {
            if (rejected != null) accepted();
        }
        public void OnRejected(RejectedEvent rejected) {
            this.rejected = rejected;
            CPP.Add("$q->connect($q.get(), &QDialog::rejected, this, &Dialog::SlotRejected);");
        }
    }
}
