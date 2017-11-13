using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QRadioButton *$q;" +
        "public: void $base(QRadioButton *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class RadioButton : AbstractButton {
        public RadioButton() : base(Derived.derived) {
            CPP.Add("$q = new QRadioButton();");
            CPP.Add("AbstractButton::$base($q);");
        }
        public RadioButton(String text) : base(Derived.derived) {
            CPP.Add("$q = new QRadioButton();");
            CPP.Add("AbstractButton::$base($q);");
            SetText(text);
        }
    }
}
