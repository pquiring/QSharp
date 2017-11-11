using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "public: QHBoxLayout *$q;" +
        "public: bool $del;" +
        "public: HBoxLayout() {$q = new QHBoxLayout(); $del = true;}" +
        "public: HBoxLayout(HBoxLayout *b) {$del = false;}" +
        "public: void $base(QHBoxLayout *w) {$q = w;}"
    )]
    public class HBoxLayout : BoxLayout {
        public HBoxLayout() {
            CPP.Add("BoxLayout::$base($q);");
        }
    }
}
