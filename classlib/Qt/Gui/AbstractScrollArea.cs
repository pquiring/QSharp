using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QAbstractScrollArea *$q;" +
        "void $base(QAbstractScrollArea *$b) {$q = $b;}"
    )]
    public abstract class AbstractScrollArea : Frame {
        public AbstractScrollArea() : base(QSharpDerived.derived) {
            CPP.Add("Frame::$base($q);");
        }
    }
}
