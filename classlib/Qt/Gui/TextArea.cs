using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QTextEdit *$q;"
    )]
    public class TextArea : AbstractScrollArea {
        public TextArea() {
            CPP.Add("$q = new QTextEdit();");
            CPP.Add("AbstractScrollArea::$base($q);");
        }
        public bool IsReadOnly() {
            return CPP.ReturnBool("$q->isReadOnly()");
        }
        public void SetReadOnly(bool state) {
            CPP.Add("$q->setReadOnly(state);");
        }
        public String GetText() {
            return CPP.ReturnString("Qt::Core::String::$new($q->toPlainText())");
        }
        public void SetText(String text) {
            CPP.Add("$q->setPlainText($check(text)->qstring());");
        }
        public String GetHtml() {
            return CPP.ReturnString("Qt::Core::String::$new($q->toHtml())");
        }
        public void SetHtml(String html) {
            CPP.Add("$q->setHtml($check(html)->qstring());");
        }
    }
}
