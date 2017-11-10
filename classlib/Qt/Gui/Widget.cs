using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: QWidget *$q;" +
        "private: bool $del;" +
        "public: Widget() {$q = new QWidget(); $del = true;}" +
        "public: Widget(Widget *w) {$del = false;}" +
        "public: void $base(QWidget *w) {$q = w;}"
    )]
    public class Widget {
        int GetWidth() {return CPP.ReturnInt("$q->width()");}
        int GetHeight() {return CPP.ReturnInt("$q->height()");}
        int GetX() {return CPP.ReturnInt("$q->x()");}
        int GetY() {return CPP.ReturnInt("$q->y()");}
        ~Widget() {
            CPP.Add("if ($del) delete $q;");
        }
    }
}
