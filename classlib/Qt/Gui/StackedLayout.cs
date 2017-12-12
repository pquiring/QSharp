using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QStackedLayout *$q;" +
        "public: void $base(QStackedLayout *$d) {$q = $d; Layout::$base($q);}"
    )]
    public class StackedLayout : Layout {
        protected StackedLayout(QSharpDerived derived) {}
        public StackedLayout() {
            CPP.Add("$q = new QStackedLayout();");
            CPP.Add("Layout::$base($q);");
        }
        public void InsertWidget(int idx, Widget w) {
            CPP.Add("$q->insertWidget(idx, w->$q);");
        }
    }
}
