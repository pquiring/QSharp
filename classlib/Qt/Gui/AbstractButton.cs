using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void Clicked(bool selected);
    [CPPClass(
        "private: QAbstractButton *$q;" +
        "public: void $base(QAbstractButton *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class AbstractButton : Widget {
        protected AbstractButton(Derived derived) : base(Derived.derived) {}
        private Clicked delegateClicked;
        private void clicked(bool selected) {
            if (delegateClicked != null) delegateClicked(selected);
        }
        public void SetText(String text) {
            CPP.Add("$q->setText(text->qstring());");
        }
        public String GetText() {
            return CPP.ReturnString("std::make_shared<String>($q->text())");
        }
        public void OnClicked(Clicked handler) {
            delegateClicked = handler;
            CPP.Add("connect($q, &QAbstractButton::clicked, this, &AbstractButton::clicked);");
        }
    }
}
