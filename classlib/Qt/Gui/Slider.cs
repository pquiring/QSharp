using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QSlider *$q;" +
        "void $base(QSlider *$d) {$q = $d; AbstractSlider::$base($q);}"
    )]
    public class Slider : AbstractSlider {
        public Slider() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QSlider();");
            CPP.Add("AbstractSlider::$base($q);");
        }
        public Slider(Orientation orientation) : base(QSharpDerived.derived) {
            CPP.Add("$q = new QSlider((Qt::Orientation)orientation);");
            CPP.Add("AbstractSlider::$base($q);");
        }
    }
}
