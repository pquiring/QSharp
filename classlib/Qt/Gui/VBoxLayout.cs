using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "public: QVBoxLayout *$q;" +
        "public: bool $del;" +
        "public: VBoxLayout() {$q = new QVBoxLayout(); $del = true;}" +
        "public: VBoxLayout(VBoxLayout *b) {$del = false;}" +
        "public: void $base(QVBoxLayout *w) {$q = w;}"
    )]
    public class VBoxLayout : BoxLayout {
        public VBoxLayout() {
            CPP.Add("BoxLayout::$base($q);");
        }
    }
}
