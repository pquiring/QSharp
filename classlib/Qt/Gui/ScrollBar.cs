using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QScrollBar *$q;" +
        "public: void $base(QScrollBar *$d) {$q = $d; AbstractSlider::$base($q);}"
    )]
    public class ScrollBar : AbstractSlider {
        public ScrollBar() : base(Derived.derived) {
            CPP.Add("$q = new QScrollBar();");
            CPP.Add("AbstractSlider::$base($q);");
        }
        public ScrollBar(Orientation orientation) : base(Derived.derived) {
            CPP.Add("$q = new QScrollBar((Qt::Orientation)orientation);");
            CPP.Add("AbstractSlider::$base($q);");
        }
    }
}
