using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass("QColorDialog* $d() {return dynamic_cast<QColorDialog*>($q);}")]
    public class ColorDialog : Dialog {
        public ColorDialog() : base(QSharpDerived.derived) {
            CPP.Add("Dialog::$base(new QColorDialog());");
        }
        public int GetColor() {
            return CPP.ReturnInt("$d()->currentColor().rgb()");
        }
        public void SetColor(int clr) {
            CPP.Add("$d()->setCurrentColor(QColor((QRgb)clr));");
        }
    }
}
