using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ClickedEvent();
    [CPPClass(
        "QAbstractButton *$q;" +
        "void $base(QAbstractButton *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class AbstractButton : Widget {
        protected AbstractButton(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public void SetText(String text) {
            CPP.Add("$q->setText($check(text)->qstring());");
        }
        public String GetText() {
            return CPP.ReturnString("Qt::Core::String::$new($q->text())");
        }

        private ClickedEvent clicked;
        private void SlotClicked(bool selected) {
            try {
                if (clicked != null) clicked();
            } catch {}
        }
        public void OnClicked(ClickedEvent handler) {
            clicked = handler;
            CPP.Add("QObject::connect($q, &QAbstractButton::clicked, [=] (bool selected) {this->SlotClicked(selected);});");
        }
    }
}
