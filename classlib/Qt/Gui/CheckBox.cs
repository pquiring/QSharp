using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void StateChanged(int state);
    [CPPClass(
        "QCheckBox *$q;" +
        "void $base(QCheckBox *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class CheckBox : AbstractButton {
        public CheckBox() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QCheckBox();");
            CPP.Add("AbstractButton::$base($q);");
        }
        public CheckBox(String text) : base(QSharpDerived.derived) {
            CPP.Add("$q = new QCheckBox();");
            CPP.Add("AbstractButton::$base($q);");
            SetText(text);
        }
        private StateChanged delegateStateChanged;
        private void SlotStateChanged(int state) {
            try {
                if (delegateStateChanged != null) delegateStateChanged(state);
            } catch {}
        }
        public CheckState GetState() {return (CheckState)CPP.ReturnInt("$q->checkState()");}
        public void SetState(CheckState state) {CPP.Add("$q->setCheckState((Qt::CheckState)state)");}
        public void SetTriState(bool state = true) {CPP.Add("$q->setTristate(state)");}
        public bool IsTriState() {return CPP.ReturnBool("$q->isTristate()");}
        public void OnStateChanged(StateChanged handler) {
            delegateStateChanged = handler;
            CPP.Add("QObject::connect($q, &QCheckBox::stateChanged, [=] (int state) {this->SlotStateChanged(state);});");
        }
    }
}
