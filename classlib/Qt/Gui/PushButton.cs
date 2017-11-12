using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QPushButton *$q;" +
        "public: PushButton() : AbstractButton(true) {$q = new QPushButton(); AbstractButton::$base($q);}" +
        "public: void $base(QPushButton *$d) {$q = $d; AbstractButton::$base($q);}"
    )]
    public class PushButton : AbstractButton {
    }
}
