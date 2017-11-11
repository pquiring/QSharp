using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QObject")]  //for connect
    [CPPClass(
        "public: QWidget *$q;" +
        "public: bool $del;" +
        "public: Widget() {$q = new QWidget(); $del = true;}" +
        "public: Widget(Widget *w) {$del = false;}" +
        "public: void $base(QWidget *w) {$q = w;}"
    )]
    public class Widget {
        public int GetWidth() {return CPP.ReturnInt("$q->width()");}
        public int GetHeight() {return CPP.ReturnInt("$q->height()");}
        public int GetX() {return CPP.ReturnInt("$q->x()");}
        public int GetY() {return CPP.ReturnInt("$q->y()");}
        public void SetSize(int x, int y) {CPP.Add("$q->resize(x,y);");}
        public void Show() {CPP.Add("$q->show();");}
        public void Hide() {CPP.Add("$q->hide();");}
        ~Widget() {
            CPP.Add("if ($del) delete $q;");
        }
    }
}
