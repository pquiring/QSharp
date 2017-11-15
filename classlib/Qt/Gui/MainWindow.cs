using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QMainWindow *$q;" +
        "public: void $base(QMainWindow *$d) {$q = $d;}"
    )]
    public class MainWindow : Widget {
        public MainWindow() : base(Derived.derived) {
            CPP.Add("$q = new QMainWindow();");
            CPP.Add("Widget::$base($q);");
        }
        public void SetCentralWidget(Widget w) {
            CPP.Add("$q->setCentralWidget(w->$q);");
        }
        public void SetMenuWidget(Widget w) {
            CPP.Add("$q->setMenuWidget(w->$q);");
        }
    }
}
