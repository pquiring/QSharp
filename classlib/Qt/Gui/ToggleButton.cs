using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ToggledEvent(bool selected);
    [CPPClass(
        "QPushButton *$q;" +
        "void $base(QPushButton *$d) {$q = $d; AbstractButton::$base($q);}"
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
            try {
                if (toggled != null) toggled(selected);
            } catch {}
        }
        public void OnToggled(ToggledEvent handler) {
            toggled = handler;
            CPP.Add("QObject::connect($q, &QAbstractButton::toggled, [=] (bool selected) {this->SlotToggled(selected);});");
        }
    }
}
