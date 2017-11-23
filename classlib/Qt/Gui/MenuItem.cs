using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void TriggeredEvent(bool selected);
    [CPPClass(
        "public: QAction *$q;" +
        "public: MenuItem() {}" +
        "public: MenuItem(QAction *$a) {$q = $a;}"
    )]
    [CPPExtends("QObject")]  //for connect
    public class MenuItem {

        private TriggeredEvent triggered;
        private void SlotTriggered(bool selected) {
            if (triggered != null) triggered(selected);
        }
        public void OnTriggered(TriggeredEvent handler) {
            triggered = handler;
            CPP.Add("connect($q, &QAction::triggered, this, &MenuItem::SlotTriggered);");
        }
    }
}
