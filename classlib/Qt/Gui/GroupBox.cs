using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QGroupBox *$q;" +
        "public: void $base(QGroupBox *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class GroupBox : Widget {
        protected GroupBox(Derived derived) : base(Derived.derived) {}
        public GroupBox() {
            CPP.Add("$q = new QGroupBox();");
            CPP.Add("Widget::$base($q);");
        }
        public GroupBox(String title) {
            CPP.Add("$q = new QGroupBox(title->qstring());");
            CPP.Add("Widget::$base($q);");
        }
        public String GetTitle() {
            return CPP.ReturnString("std::make_shared<String>($q->title())");
        }
        public void SetTitle(String title) {
            CPP.Add("$q->setTitle(title->qstring());");
        }
    }
}
