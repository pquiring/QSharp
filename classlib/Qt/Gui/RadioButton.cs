using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QRadioButton *$q;" +
        "public: RadioButton() : AbstractButton(true) {AbstractButton::$base($q);}" +
        "public: void $base(QRadioButton *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class RadioButton : AbstractButton {
        public RadioButton() {
            CPP.Add("$q = new QRadioButton();");
        }
        public RadioButton(String text) {
            CPP.Add("$q = new QRadioButton();");
            SetText(text);
        }
    }
}
