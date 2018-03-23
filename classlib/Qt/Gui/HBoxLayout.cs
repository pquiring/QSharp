using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QHBoxLayout *$q;" +
        "void $base(QHBoxLayout *$d) {$q = $d; BoxLayout::$base($q);}"
    )]
    public class HBoxLayout : BoxLayout {
        public HBoxLayout() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QHBoxLayout();");
            CPP.Add("BoxLayout::$base($q);");
        }
    }
}
