using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: std::shared_ptr<QColorDialog> $q;"
    )]
    public class ColorDialog : Dialog {
        public ColorDialog() : base(Derived.derived) {
            CPP.Add("$q = std::make_shared<QColorDialog>();");
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
