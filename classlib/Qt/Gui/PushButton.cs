using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QPushButton *$q;" +
        "public: void $base(QPushButton *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class PushButton : AbstractButton {
        public PushButton() : base(Derived.derived) {
            CPP.Add("$q = new QPushButton();");
            CPP.Add("AbstractButton::$base($q);");
        }
        public PushButton(String text) : base(Derived.derived) {
            CPP.Add("$q = new QPushButton();");
            CPP.Add("AbstractButton::$base($q);");
            SetText(text);
        }
    }
}
