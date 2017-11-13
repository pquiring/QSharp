using Qt.QSharp;

namespace Qt.Gui {
    public enum CheckState {Unchecked, PartiallyChecked, Checked}

    [CPPExtends("QObject")]  //for connect
    [CPPClass(
        "public: QWidget *$q;" +
        "public: Widget() {$q = new QWidget();}" +
        "public: Widget(bool derived) {}" +
        "public: void $base(QWidget *$d) {$q = $d;}"
    )]
    public class Widget {
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
    }
}
