using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void TriggeredEvent(bool selected);
    [CPPClass(
        "public: QAction *$q;"
    )]
    public class MenuItem {
        private MenuItem() {}
        [CPPReplaceArgs("QAction *$a")]
        private MenuItem(NativeArg1 arg) {
            CPP.Add("$q = $a;");
        }

        private TriggeredEvent triggered;
        private void SlotTriggered(bool selected) {
            try {
                if (triggered != null) triggered(selected);
            } catch {}
        }
        public void OnTriggered(TriggeredEvent handler) {
            triggered = handler;
            CPP.Add("QObject::connect($q, &QAction::triggered, [=] (bool selected) {this->SlotTriggered(selected);});");
        }
    }
}
