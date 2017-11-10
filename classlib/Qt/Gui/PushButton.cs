using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QPushButton *$q;" +
        "private: bool $del;" +
        "public: PushButton() {$q = new QPushButton(); $del = true; AbstractButton::$base($q);}" +
        "public: PushButton(PushButton *w) {$del = false;}" +
        "public: void $base(QPushButton *w) {$q = w; AbstractButton::$base($q);}"
    )]
    public class PushButton : AbstractButton {
        ~PushButton() {
            CPP.Add("if ($del) delete $q;");
        }
    }
}
