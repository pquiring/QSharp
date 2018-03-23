using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QFrame *$q;" +
        "void $base(QFrame *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class Frame : Widget {
        protected Frame(QSharpDerived derived) : base(QSharpDerived.derived) {}
    }
}
