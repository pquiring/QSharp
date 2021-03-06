using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QDial *$q;" +
        "void $base(QDial *$d) {$q = $d; AbstractSlider::$base($q);}"
    )]
    public class Dial : AbstractSlider {
        public Dial() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QDial();");
            CPP.Add("AbstractSlider::$base($q);");
        }
    }
}
