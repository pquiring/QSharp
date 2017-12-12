using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "public: QVBoxLayout *$q;" +
        "public: void $base(QVBoxLayout *$d) {$q = $d; BoxLayout::$base($q);}"
    )]
    public class VBoxLayout : BoxLayout {
        public VBoxLayout() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QVBoxLayout(); BoxLayout::$base($q);");
        }
    }
}
