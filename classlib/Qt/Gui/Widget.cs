using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum CheckState {Unchecked, PartiallyChecked, Checked}
    public enum Orientation {Horizontal	= 1, Vertical}

    [CPPExtends("QObject")]  //for connect
    [CPPClass(
        "public: QWidget *$q;" +
        "public: void $base(QWidget *$d) {$q = $d;}"
    )]
    public class Widget {
        public Widget() {
            CPP.Add("$q = new QWidget();");
        }
        protected Widget(Derived derived) {}
        public int GetWidth() {return CPP.ReturnInt("$q->width()");}
        public int GetHeight() {return CPP.ReturnInt("$q->height()");}
        public int GetX() {return CPP.ReturnInt("$q->x()");}
        public int GetY() {return CPP.ReturnInt("$q->y()");}
        public void SetSize(int x, int y) {CPP.Add("$q->resize(x,y);");}
        public void Show() {CPP.Add("$q->show();");}
        public void Hide() {CPP.Add("$q->hide();");}
        public void SetLayout(Layout layout) {
            CPP.Add("$q->setLayout(layout->$q);");
        }
        public void SetEnabled(bool state) {
            CPP.Add("$q->setEnabled(state);");
        }
        public bool IsEnabled() {
            return CPP.ReturnBool("$q->isEnabled();");
        }
        public String GetWindowTitle() {
            return CPP.ReturnString("std::make_shared<String>($q->windowTitle())");
        }
        public void SetWindowTitle(String title) {
            CPP.Add("$q->setWindowTitle(title->qstring());");
        }
    }
}
