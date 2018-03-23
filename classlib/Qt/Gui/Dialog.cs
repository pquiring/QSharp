using Qt.QSharp;

namespace Qt.Gui {
    public class DialogResult {
        public const int Rejected = 0;
        public const int Accepted = 1;
    }
    public delegate void AcceptedEvent();
    public delegate void FinishedEvent(int result);
    public delegate void RejectedEvent();
    [CPPClass(
        "std::shared_ptr<QDialog> $q;" +
        "void $base(std::shared_ptr<QDialog> $d) {$q = $d; Widget::$base($q.get());}"
    )]
    public class Dialog : Widget {
        private NativeWindow nativeWindow;
        protected Dialog(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public Dialog() : base(QSharpDerived.derived) {
            CPP.Add("$q = std::make_shared<QDialog>();");
            CPP.Add("Widget::$base($q.get());");
        }
        public bool IsModal() {
            return CPP.ReturnBool("$q->isModal()");
        }
        public void SetModal(bool modal) {
            CPP.Add("$q->setModal(modal);");
        }
        /** Returns DialogResult or ButtonType for MessageDialog. */
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
            if (nativeWindow == null) {
                nativeWindow = (NativeWindow)CPP.ReturnObject("NativeWindow::$new($q->windowHandle())");
            }
            return nativeWindow;
        }
        public void OnInputEvents(InputEvents events) {
            GetNativeWindow().OnInputEvents(events);
        }

        private AcceptedEvent accepted;
        private void SlotAccepted() {
            try {
                if (accepted != null) accepted();
            } catch {}
        }
        public void OnAccepted(AcceptedEvent accepted) {
            this.accepted = accepted;
            CPP.Add("QObject::connect($q.get(), &QDialog::accepted, [=] () {this->SlotAccepted();});");
        }

        private FinishedEvent finished;
        private void SlotFinished(int result) {
            try {
                if (finished != null) finished(result);
            } catch {}
        }
        public void OnFinished(FinishedEvent finished) {
            this.finished = finished;
            CPP.Add("QObject::connect($q.get(), &QDialog::finished, [=] (int result) {this->SlotFinished(result);});");
        }

        private RejectedEvent rejected;
        private void SlotRejected() {
            try {
                if (rejected != null) accepted();
            } catch {}
        }
        public void OnRejected(RejectedEvent rejected) {
            this.rejected = rejected;
            CPP.Add("QObject::connect($q.get(), &QDialog::rejected, [=] () {this->SlotRejected();});");
        }
    }
}
