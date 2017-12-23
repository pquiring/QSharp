using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum ToolBarArea {NoToolBarArea = 0, LeftToolBarArea = 0x1, RightToolBarArea = 0x2, TopToolBarArea = 0x4, BottomToolBarArea = 0x8, AllToolBarAreas = 0xf}
    [CPPExtends("QMainWindow")]
    public class Window : Widget {
        private NativeWindow nativeWindow;
        public Window() : base(QSharpDerived.derived) {
            CPP.Add("Widget::$base(this);");
        }
        public void SetCentralWidget(Widget widget) {
            CPP.Add("setCentralWidget($check(widget)->$q);");
        }
        public void SetMenuWidget(Widget widget) {
            CPP.Add("setMenuWidget($check(widget)->$q);");
        }
        public void SetMenuBar(MenuBar menubar) {
            CPP.Add("setMenuBar($check(menubar)->$q);");
        }
        public void AddToolBar(ToolBar toolbar) {
            CPP.Add("addToolBar($check(toolbar)->$q);");
        }
        public void AddToolBar(ToolBarArea toolbararea, ToolBar toolbar) {
            CPP.Add("addToolBar((Qt::ToolBarArea)toolbararea, $check(toolbar)->$q);");
        }
        public void OnInputEvents(InputEvents events) {
            GetNativeWindow().OnInputEvents(events);
        }
        public NativeWindow GetNativeWindow() {
            if (nativeWindow == null) {
                nativeWindow = (NativeWindow)CPP.ReturnObject("NativeWindow::$new(windowHandle())");
            }
            return nativeWindow;
        }
    }
}
