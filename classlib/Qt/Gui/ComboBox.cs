using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "QComboBox *$q;" +
        "void $base(QComboBox *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class ComboBox : Widget {
        protected ComboBox(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public ComboBox() {
            CPP.Add("$q = new QComboBox();");
            CPP.Add("Widget::$base($q);");
        }
        public void AddItem(String text) {
            CPP.Add("$q->addItem($check(text)->qstring());");
        }
        public void InsertItem(int index, String text) {
            CPP.Add("$q->insertItem(index, $check(text)->qstring());");
        }
        public void RemoveItem(int index) {
            CPP.Add("$q->removeItem(index);");
        }
        public int Count() {
            return CPP.ReturnInt("$q->count()");
        }
        public String GetSelectedText() {
            return CPP.ReturnString("new Qt::Core::String($q->currentText())");
        }
        public int GetSelectedIndex() {
            return CPP.ReturnInt("$q->currentIndex()");
        }
        public void Clear() {
            CPP.Add("$q->clear();");
        }

        private ChangedEvent changed;
        private void SlotChanged(int idx) {
            if (changed != null) changed();
        }
        public void OnChanged(ChangedEvent changed) {
            this.changed = changed;
            CPP.Add("QObject::connect($q, (void (QComboBox::*)(int))&QComboBox::currentIndexChanged, [=] (int idx) {this->SlotChanged(idx);});");
        }
    }
}
