using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum Direction { LeftToRight, RightToLeft, TopToBottom, BottomToTop }
    [CPPClass(
        "public: QBoxLayout *$q;" +
        "public: void $base(QBoxLayout *$d) {$q = $d; Layout::$base($q);}"
    )]
    public class BoxLayout : Layout {
        protected BoxLayout() {}
        public BoxLayout(Direction direction) {
            CPP.Add("$q = new QBoxLayout((QBoxLayout::Direction)direction);");
        }
        public void SetDirection(Direction direction) {
            CPP.Add("$q->setDirection((QBoxLayout::Direction)direction);");
        }
    }
}
