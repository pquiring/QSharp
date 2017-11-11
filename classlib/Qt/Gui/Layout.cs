using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "public: QLayout *$q;" +
        "public: bool $del;" +
        "public: Layout() {$del = false;}" +
        "public: Layout(Layout *b) {$del = false;}" +
        "public: void $base(QLayout *w) {$q = w;}"
    )]
    public abstract class Layout {
        public void AddWidget(Widget w) {
            CPP.Add("$q->addWidget(w->$q);");
        }
    }
}
