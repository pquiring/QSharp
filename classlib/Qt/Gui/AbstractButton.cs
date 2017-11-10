using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QAbstractButton *$q;\r\n" +
        "public: AbstractButton() {}\r\n" +
        "public: AbstractButton(AbstractButton *b) : Widget(this) {}\r\n" +
        "public: void $base(QAbstractButton *b) {$q = b; Widget::$base(b);}\r\n"
    )]
    public abstract class AbstractButton : Widget {
        public void SetText(String text) {
            CPP.Add("$q->setText(text->qstring());");
        }
        public String GetText() {
            return CPP.ReturnString("std::make_shared<String>($q->text())");
        }
    }
}
