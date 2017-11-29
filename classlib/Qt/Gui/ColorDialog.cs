using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QColorDialog *$q;" +
        "public: void $base(QColorDialog *$d) {$q = $d; Dialog::$base($q);}"
    )]
    public class ColorDialog : Dialog {
        public ColorDialog() : base(Derived.derived) {
            CPP.Add("$q = new QColorDialog();");
            CPP.Add("Dialog::$base($q);");
        }
        public int GetColor() {
            return CPP.ReturnInt("$q->currentColor().rgb()");
        }
        public void SetColor(int clr) {
            CPP.Add("$q->setCurrentColor(QColor((QRgb)clr));");
        }
    }
}
