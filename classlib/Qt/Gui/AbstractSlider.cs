using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void SliderMoved(int value);
    [CPPClass(
        "private: QAbstractSlider *$q;" +
        "public: void $base(QAbstractSlider *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class AbstractSlider : Widget {
        protected AbstractSlider(Derived derived) : base(Derived.derived) {}
        private SliderMoved delegateSliderMoved;
        private void SliderMoved(int value) {
            if (delegateSliderMoved != null) delegateSliderMoved(value);
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
            CPP.Add("connect($q, &QAbstractSlider::sliderMoved, this, &AbstractSlider::SliderMoved);");
        }
    }
}
