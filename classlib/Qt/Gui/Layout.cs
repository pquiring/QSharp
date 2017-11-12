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
    }
}
