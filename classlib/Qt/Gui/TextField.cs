using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QLineEdit *$q;"
    )]
    public class TextField : Widget {
        public TextField() {
            CPP.Add("$q = new QLineEdit();");
            CPP.Add("Widget::$base($q);");
        }
        public TextField(String text) {
            CPP.Add("$q = new QLineEdit();");
            CPP.Add("Widget::$base($q);");
            SetText(text);
        }
        public bool IsReadOnly() {
            return CPP.ReturnBool("$q->isReadOnly()");
        }
        public void SetReadOnly(bool state) {
            CPP.Add("$q->setReadOnly(state);");
        }
        public String GetText() {
            return CPP.ReturnString("String::$new($q->text())");
        }
        public void SetText(String text) {
            CPP.Add("$q->setText(text->qstring());");
        }
        public bool IsPasswordMode() {
            return CPP.ReturnBool("($q->echoMode() == QLineEdit::Password)");
        }
        public void SetPasswordMode(bool passwordMode) {
            CPP.Add("$q->setEchoMode(passwordMode ? QLineEdit::Password : QLineEdit::Normal);");
        }
    }
}
