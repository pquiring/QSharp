using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QPushButton *$q;" +
        "public: void $base(QPushButton *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class Button : AbstractButton {
        public Button() : base(Derived.derived) {
            CPP.Add("$q = new QPushButton();");
            CPP.Add("AbstractButton::$base($q);");
        }
        public Button(String text) : base(Derived.derived) {
            CPP.Add("$q = new QPushButton();");
            CPP.Add("AbstractButton::$base($q);");
            SetText(text);
        }
    }
}
