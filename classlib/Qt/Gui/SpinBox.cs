using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ValueChanged(int value);
    [CPPClass(
        "private: QSpinBox *$q;" +
        "public: void $base(QSpinBox *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class SpinBox : Widget {
        protected SpinBox(QSharpDerived derived) : base(QSharpDerived.derived) {}
        private ValueChanged delegateValueChanged;
        private void SlotValueChanged(int value) {
            try {
                if (delegateValueChanged != null) delegateValueChanged(value);
            } catch {}
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

        public void OnValueChanged(ValueChanged handler) {
            delegateValueChanged = handler;
            CPP.Add("QObject::connect($q, QOverload<int>::of(&QSpinBox::valueChanged), [=] (int value) {this->SlotValueChanged(value);});");
        }
    }
}
