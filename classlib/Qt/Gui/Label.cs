using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QLabel *$q;" +
        "public: void $base(QLabel *$d) {$q = $d; Frame::$base($q);}"
    )]
    public class Label : Frame {
        public Label() : base(Derived.derived) {
            CPP.Add("$q = new QLabel();");
            CPP.Add("Frame::$base($q);");
        }
        public Label(String text) : base(Derived.derived) {
            CPP.Add("$q = new QLabel(text->qstring());");
            CPP.Add("Frame::$base($q);");
        }
        public String GetText() {
            return CPP.ReturnString("std::make_shared<String>($q->text())");
        }
        public void SetText(String text) {
            CPP.Add("$q->setText(text->qstring());");
        }
    }
}
