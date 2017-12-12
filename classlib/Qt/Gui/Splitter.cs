using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: QSplitter *$q;" +
        "public: void $base(QSplitter *$d) {$q = $d; Frame::$base($q);}"
    )]
    public class Splitter : Frame {
        public Splitter() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QSplitter();");
            CPP.Add("Frame::$base($q);");
        }
        public Orientation GetOrientation() {
            return (Orientation)CPP.ReturnInt("$q->orientation()");
        }
        public void SetOrientation(Orientation orientation) {
            CPP.Add("$q->setOrientation((Qt::Orientation)orientation);");
        }
        public void AddWidget(Widget w) {
            CPP.Add("$q->addWidget(w->$q);");
        }
        public void InsertWidget(int index, Widget w) {
            CPP.Add("$q->insertWidget(index, w->$q);");
        }
    }
}
