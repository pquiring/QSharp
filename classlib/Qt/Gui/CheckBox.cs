using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void StateChanged(int state);
    [CPPClass(
        "private: QCheckBox *$q;" +
        "public: void $base(QCheckBox *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class CheckBox : AbstractButton {
        public CheckBox() : base(Derived.derived) {
            CPP.Add("$q = new QCheckBox();");
            CPP.Add("AbstractButton::$base($q);");
        }
        public CheckBox(String text) : base(Derived.derived) {
            CPP.Add("$q = new QCheckBox();");
            CPP.Add("AbstractButton::$base($q);");
            SetText(text);
        }
        private StateChanged delegateStateChanged;
        private void stateChanged(int state) {
            if (delegateStateChanged != null) delegateStateChanged(state);
        }
        public CheckState GetState() {return (CheckState)CPP.ReturnInt("$q->checkState()");}
        public void SetState(CheckState state) {CPP.Add("$q->setCheckState((Qt::CheckState)state)");}
        public void SetTriState(bool state = true) {CPP.Add("$q->setTristate(state)");}
        public bool IsTriState() {return CPP.ReturnBool("$q->isTristate()");}
        public void OnStateChanged(StateChanged handler) {
            delegateStateChanged = handler;
            CPP.Add("connect($q, &QCheckBox::stateChanged, this, &CheckBox::stateChanged);");
        }
    }
}
