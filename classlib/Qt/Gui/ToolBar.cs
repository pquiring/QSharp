using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QToolBar *$q;"
    )]
    public class ToolBar : Widget {
        public ToolBar() {
            CPP.Add("$q = new QToolBar();");
            CPP.Add("Widget::$base($q);");
        }
        public void AddWidget(Widget widget) {
            CPP.Add("$q->addWidget(widget->$q);");
        }
        public bool IsFloatable() {
            return CPP.ReturnBool("$q->isFloatable();");
        }
        public void SetFloatable(bool floatable) {
            CPP.Add("$q->setFloatable(floatable);");
        }
        public bool IsMovable() {
            return CPP.ReturnBool("$q->isMovable();");
        }
        public void SetMovable(bool movable) {
            CPP.Add("$q->setMovable(movable);");
        }
    }
}
