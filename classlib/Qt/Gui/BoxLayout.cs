using Qt.QSharp;

namespace Qt.Gui {
    public enum Direction { LeftToRight, RightToLeft, TopToBottom, BottomToTop }
    [CPPClass(
        "public: QBoxLayout *$q;" +
        "public: bool $del;" +
        "public: BoxLayout() {$del = true;}" +
        "public: BoxLayout(BoxLayout *b) {$del = false;}" +
        "public: void $base(QBoxLayout *w) {$q = w;}"
    )]
    public class BoxLayout : Layout {
        protected BoxLayout() {}
        public BoxLayout(Direction direction) {
            CPP.Add("$q = new QBoxLayout((QBoxLayout::Direction)direction);");
            CPP.Add("Layout::$base($q);");
        }
        public void SetDirection(Direction direction) {
            CPP.Add("$q->setDirection((QBoxLayout::Direction)direction);");
        }
    }
}
