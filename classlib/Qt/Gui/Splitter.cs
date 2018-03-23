using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QSplitter *$q;" +
        "void $base(QSplitter *$d) {$q = $d; Frame::$base($q);}"
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
        public void AddWidget(Widget widget) {
            CPP.Add("$q->addWidget($check(widget)->$q);");
        }
        public void InsertWidget(int index, Widget widget) {
            CPP.Add("$q->insertWidget(index, $check(widget)->$q);");
        }
    }
}
