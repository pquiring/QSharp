using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ClickedEvent();
    [CPPClass(
        "private: QAbstractButton *$q;" +
        "public: void $base(QAbstractButton *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class AbstractButton : Widget {
        protected AbstractButton(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public void SetText(String text) {
            CPP.Add("$q->setText($check(text)->qstring());");
        }
        public String GetText() {
            return CPP.ReturnString("String::$new($q->text())");
        }

        private ClickedEvent clicked;
        private void SlotClicked(bool selected) {
            try {
                if (clicked != null) clicked();
            } catch {}
        }
        public void OnClicked(ClickedEvent handler) {
            clicked = handler;
            CPP.Add("connect($q, &QAbstractButton::clicked, this, &AbstractButton::SlotClicked);");
        }
    }
}
