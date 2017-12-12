using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QDial *$q;" +
        "public: void $base(QDial *$d) {$q = $d; AbstractSlider::$base($q);}"
    )]
    public class Dial : AbstractSlider {
        public Dial() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QDial();");
            CPP.Add("AbstractSlider::$base($q);");
        }
    }
}
