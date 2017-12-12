using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ToggledEvent(bool selected);
    [CPPClass(
        "private: QPushButton *$q;" +
        "public: void $base(QPushButton *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class ToggleButton : AbstractButton {
        public ToggleButton() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QPushButton();");
            CPP.Add("$q->setCheckable(true);");
            CPP.Add("AbstractButton::$base($q);");
        }
        public ToggleButton(String text) : base(QSharpDerived.derived) {
            CPP.Add("$q = new QPushButton();");
            CPP.Add("$q->setCheckable(true);");
            CPP.Add("AbstractButton::$base($q);");
            SetText(text);
        }
        public bool IsSelected() {
            return CPP.ReturnBool("$q->isChecked()");
        }

        private ToggledEvent toggled;
        private void SlotToggled(bool selected) {
            if (toggled != null) toggled(selected);
        }
        public void OnToggled(ToggledEvent handler) {
            toggled = handler;
            CPP.Add("connect($q, &QAbstractButton::toggled, this, &ToggleButton::SlotToggled);");
        }
    }
}
