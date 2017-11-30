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
        public bool IsReadOnly() {
            return CPP.ReturnBool("$q->isReadOnly()");
        }
        public void SetReadOnly(bool state) {
            CPP.Add("$q->setReadOnly(state);");
        }
        public String GetText() {
            return CPP.ReturnString("std::make_shared<String>($q->text())");
        }
        public void SetText(String text) {
            CPP.Add("$q->setText(text->qstring());");
        }
    }
}
