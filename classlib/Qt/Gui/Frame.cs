using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QFrame *$q;" +
        "public: void $base(QFrame *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class Frame : Widget {
        protected Frame(QSharpDerived derived) : base(QSharpDerived.derived) {}
    }
}
