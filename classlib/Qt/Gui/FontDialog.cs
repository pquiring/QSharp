using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: std::shared_ptr<QFontDialog> $q;"
    )]
    public class FontDialog : Dialog {
        public FontDialog() : base(QSharpDerived.derived) {
            CPP.Add("$q = std::make_shared<QFontDialog>();");
            CPP.Add("Dialog::$base($q);");
        }
        public Font GetFont() {
            return (Font)CPP.ReturnObject("Font::$new($q->selectedFont())");
        }
        public void SetFont(Font font) {
            CPP.Add("$q->setCurrentFont(*font->$q);");
        }
    }
}
