using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: QProgressBar *$q;" +
        "public: void $base(QProgressBar *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class ProgressBar : Widget {
        protected ProgressBar(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public ProgressBar() {
            CPP.Add("$q = new QProgressBar();");
        }
        public void SetOrientation(Orientation orientation) {
            CPP.Add("$q->setOrientation((Qt::Orientation)orientation);");
        }
        public int GetMinimum() {
            return CPP.ReturnInt("$q->minimum()");
        }
        public void SetMinimum(int minimum) {
            CPP.Add("$q->setMinimum(minimum);");
        }
        public int GetMaximum() {
            return CPP.ReturnInt("$q->maximum()");
        }
        public void SetMaximum(int maximum) {
            CPP.Add("$q->setMaximum(maximum);");
        }
        public void SetRange(int min, int max) {
            CPP.Add("$q->setRange(min, max);");
        }
        public int GetValue() {
            return CPP.ReturnInt("$q->value()");
        }
        public void SetValue(int value) {
            CPP.Add("$q->setValue(value);");
        }
    }
}
