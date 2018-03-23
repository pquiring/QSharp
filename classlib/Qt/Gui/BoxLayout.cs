using Qt.QSharp;

namespace Qt.Gui {
    [CPPEnum("QBoxLayout::Direction")]
    public enum Direction { LeftToRight, RightToLeft, TopToBottom, BottomToTop }
    [CPPClass(
        "QBoxLayout *$q;" +
        "void $base(QBoxLayout *$d) {$q = $d; Layout::$base($q);}"
    )]
    public class BoxLayout : Layout {
        protected BoxLayout(QSharpDerived derived) {}
        public BoxLayout(Direction direction) {
            CPP.Add("$q = new QBoxLayout((QBoxLayout::Direction)direction);");
            CPP.Add("Layout::$base($q);");
        }
        public void SetDirection(Direction direction) {
            CPP.Add("$q->setDirection((QBoxLayout::Direction)direction);");
        }
    }
}
