using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum ToolBarArea {NoToolBarArea = 0, LeftToolBarArea = 0x1, RightToolBarArea = 0x2, TopToolBarArea = 0x4, BottomToolBarArea = 0x8, AllToolBarAreas = 0xf}
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
        public void SetMenuBar(MenuBar menubar) {
            CPP.Add("$q->setMenuBar(menubar->$q);");
        }
        public void AddToolBar(ToolBar toolbar) {
            CPP.Add("$q->addToolBar(toolbar->$q);");
        }
        public void AddToolBar(ToolBarArea toolbararea, ToolBar toolbar) {
            CPP.Add("$q->addToolBar((Qt::ToolBarArea)toolbararea, toolbar->$q);");
        }
        public NativeWindow GetNativeWindow() {
            CPP.Add("std::shared_ptr<Window> window;");
            CPP.Add("window = std::make_shared<Window>($q->windowHandle());");
            return (NativeWindow)CPP.ReturnObject("window");
        }
    }
}
