using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QAbstractScrollArea *$q;" +
        "public: void $base(QAbstractScrollArea *$b) {$q = $b;}"
    )]
    public abstract class AbstractScrollArea : Frame {
        public AbstractScrollArea() : base(Derived.derived) {
            CPP.Add("Frame::$base($q);");
        }
    }
}
