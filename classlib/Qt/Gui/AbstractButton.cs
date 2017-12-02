using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ClickedEvent();
    [CPPClass(
        "private: QAbstractButton *$q;" +
        "public: void $base(QAbstractButton *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class AbstractButton : Widget {
        protected AbstractButton(Derived derived) : base(Derived.derived) {}
        public void SetText(String text) {
            CPP.Add("$q->setText(text->qstring());");
        }
        public String GetText() {
            return CPP.ReturnString("std::make_shared<String>($q->text())");
        }

        private ClickedEvent clicked;
        private void SlotClicked(bool selected) {
            if (clicked != null) clicked();
        }
        public void OnClicked(ClickedEvent handler) {
            clicked = handler;
            CPP.Add("connect($q, &QAbstractButton::clicked, this, &AbstractButton::SlotClicked);");
        }
    }
}
