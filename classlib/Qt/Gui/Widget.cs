using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ChangedEvent();

    [CPPEnum("Qt::CheckState")]
    public enum CheckState {Unchecked, PartiallyChecked, Checked}
    [CPPEnum("Qt::Orientation")]
    public enum Orientation {Horizontal	= 1, Vertical}
    [CPPEnum(new string[] {"QMessageBox::Icon", "QSystemTrayIcon::MessageIcon"})]
    public enum IconType {NoIcon, Information, Warning, Critical, Question}

    [CPPClass(
        "QWidget *$q;" +
        "void $base(QWidget *$d) {$q = $d;}"
    )]
    public class Widget : OpenGLFunctions {
        public Widget() {
            CPP.Add("$q = new QWidget();");
        }
        protected Widget(QSharpDerived derived) {}
        public int GetWidth() {return CPP.ReturnInt("$q->width()");}
        public int GetHeight() {return CPP.ReturnInt("$q->height()");}
        public int GetX() {return CPP.ReturnInt("$q->x()");}
        public int GetY() {return CPP.ReturnInt("$q->y()");}
        public void SetSize(int x, int y) {CPP.Add("$q->resize(x,y);");}
        public void Show() {CPP.Add("$q->show();");}
        public void Hide() {CPP.Add("$q->hide();");}
        public void SetLayout(Layout layout) {
            CPP.Add("$q->setLayout($check(layout)->$q);");
        }
        public void SetEnabled(bool state) {
            CPP.Add("$q->setEnabled(state);");
        }
        public bool IsEnabled() {
            return CPP.ReturnBool("$q->isEnabled();");
        }
        public String GetWindowTitle() {
            return CPP.ReturnString("new Qt::Core::String($q->windowTitle())");
        }
        public void SetWindowTitle(String title) {
            CPP.Add("$q->setWindowTitle($check(title)->qstring());");
        }
    }
}
