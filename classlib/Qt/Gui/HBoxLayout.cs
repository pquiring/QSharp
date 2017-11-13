using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "public: QHBoxLayout *$q;" +
        "public: void $base(QHBoxLayout *$d) {$q = $d; BoxLayout::$base($q);}"
    )]
    public class HBoxLayout : BoxLayout {
        public HBoxLayout() : base(Derived.derived) {
            CPP.Add("$q = new QHBoxLayout();");
            CPP.Add("BoxLayout::$base($q);");
        }
    }
}
