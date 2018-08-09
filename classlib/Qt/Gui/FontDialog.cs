using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QFontDialog *$q;"
    )]
    public class FontDialog : Dialog {
        public FontDialog() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QFontDialog();");
            CPP.Add("Dialog::$base($q);");
        }
        public Font GetFont() {
            return (Font)CPP.ReturnObject("new Font($q->selectedFont())");
        }
        public void SetFont(Font font) {
            CPP.Add("$q->setCurrentFont(*$check(font)->$q);");
        }
    }
}
