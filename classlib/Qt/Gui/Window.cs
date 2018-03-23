using Qt.QSharp;

namespace Qt.Gui {
    [CPPEnum("Qt::ToolBarArea")]
    public enum ToolBarArea {NoToolBarArea = 0, LeftToolBarArea = 0x1, RightToolBarArea = 0x2, TopToolBarArea = 0x4, BottomToolBarArea = 0x8, AllToolBarAreas = 0xf}
    [CPPClass(
        "std::shared_ptr<QMainWindow> $q;"
    )]
    public class Window : Widget {
        private NativeWindow nativeWindow;
        public Window() : base(QSharpDerived.derived) {
            CPP.Add("$q = std::make_shared<QMainWindow>();");
            CPP.Add("Widget::$base($q.get());");
        }
        public void SetCentralWidget(Widget widget) {
            CPP.Add("$q->setCentralWidget($check(widget)->$q);");
        }
        public void SetMenuWidget(Widget widget) {
            CPP.Add("$q->setMenuWidget($check(widget)->$q);");
        }
        public void SetMenuBar(MenuBar menubar) {
            CPP.Add("$q->setMenuBar($check(menubar)->$q);");
        }
        public void AddToolBar(ToolBar toolbar) {
            CPP.Add("$q->addToolBar($check(toolbar)->$q);");
        }
        public void AddToolBar(ToolBarArea toolbararea, ToolBar toolbar) {
            CPP.Add("$q->addToolBar((Qt::ToolBarArea)toolbararea, $check(toolbar)->$q);");
        }
        public void OnInputEvents(InputEvents events) {
            GetNativeWindow().OnInputEvents(events);
        }
        public NativeWindow GetNativeWindow() {
            if (nativeWindow == null) {
                nativeWindow = (NativeWindow)CPP.ReturnObject("NativeWindow::$new($q->windowHandle())");
            }
            return nativeWindow;
        }
    }
}
