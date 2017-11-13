using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QLayout *$q;" +
        "public: void $base(QLayout *$d) {$q = $d;}"
    )]
    public abstract class Layout {
        public void AddWidget(Widget w) {
            CPP.Add("$q->addWidget(w->$q);");
        }
        public void RemoveWidget(Widget w) {
            CPP.Add("$q->removeWidget(w->$q);");
        }
        public void SetEnabled(bool state) {
            CPP.Add("$q->setEnabled(state);");
        }
        public bool IsEnabled() {
            return CPP.ReturnBool("$q->isEnabled();");
        }
    }
}
