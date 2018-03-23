using Qt.QSharp;

namespace Qt.Gui {
    public delegate void SliderMoved(int value);
    [CPPClass(
        "QAbstractSlider *$q;" +
        "void $base(QAbstractSlider *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class AbstractSlider : Widget {
        protected AbstractSlider(QSharpDerived derived) : base(QSharpDerived.derived) {}
        private SliderMoved delegateSliderMoved;
        private void SlotSliderMoved(int value) {
            try {
                if (delegateSliderMoved != null) delegateSliderMoved(value);
            } catch {}
        }
        public Orientation GetOrientation() {
            return (Orientation)CPP.ReturnInt("$q->orientation()");
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
        public int GetSliderPosition() {
            return CPP.ReturnInt("$q->sliderPosition()");
        }
        public void SetSliderPosition(int sliderPosition) {
            CPP.Add("$q->setSliderPosition(sliderPosition);");
        }
        public int GetValue() {
            return CPP.ReturnInt("$q->value()");
        }
        public void SetValue(int value) {
            CPP.Add("$q->setValue(value);");
        }

        public void OnSliderMoved(SliderMoved handler) {
            delegateSliderMoved = handler;
            CPP.Add("QObject::connect($q, &QAbstractSlider::sliderMoved, [=] (int value) {this->SlotSliderMoved(value);});");
        }
    }
}
