using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "QGroupBox *$q;" +
        "void $base(QGroupBox *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class GroupBox : Widget {
        protected GroupBox(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public GroupBox() {
            CPP.Add("$q = new QGroupBox();");
            CPP.Add("Widget::$base($q);");
        }
        public GroupBox(String title) {
            CPP.Add("$q = new QGroupBox($check(title)->qstring());");
            CPP.Add("Widget::$base($q);");
        }
        public String GetTitle() {
            return CPP.ReturnString("new Qt::Core::String($q->title())");
        }
        public void SetTitle(String title) {
            CPP.Add("$q->setTitle($check(title)->qstring());");
        }
    }
}
