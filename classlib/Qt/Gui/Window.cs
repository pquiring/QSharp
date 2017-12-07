using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum ToolBarArea {NoToolBarArea = 0, LeftToolBarArea = 0x1, RightToolBarArea = 0x2, TopToolBarArea = 0x4, BottomToolBarArea = 0x8, AllToolBarAreas = 0xf}
    [CPPExtends("QMainWindow")]
    public class Window : Widget {
        public Window() : base(Derived.derived) {
            CPP.Add("Widget::$base(this);");
        }
        public void SetCentralWidget(Widget w) {
            CPP.Add("setCentralWidget(w->$q);");
        }
        public void SetMenuWidget(Widget w) {
            CPP.Add("setMenuWidget(w->$q);");
        }
        public void SetMenuBar(MenuBar menubar) {
            CPP.Add("setMenuBar(menubar->$q);");
        }
        public void AddToolBar(ToolBar toolbar) {
            CPP.Add("addToolBar(toolbar->$q);");
        }
        public void AddToolBar(ToolBarArea toolbararea, ToolBar toolbar) {
            CPP.Add("addToolBar((Qt::ToolBarArea)toolbararea, toolbar->$q);");
        }
        public NativeWindow GetNativeWindow() {
            return (NativeWindow)CPP.ReturnObject("NativeWindow::$new(windowHandle())");
        }
    }
}
