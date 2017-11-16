using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QComboBox *$q;" +
        "public: void $base(QComboBox *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class ComboBox : Widget {
        protected ComboBox(Derived derived) : base(Derived.derived) {}
        public ComboBox() {
            CPP.Add("$q = new QComboBox();");
        }
        public void AddItem(String text) {
            CPP.Add("$q->addItem(text->qstring());");
        }
        public void InsertItem(int index, String text) {
            CPP.Add("$q->insertItem(index, text->qstring());");
        }
        public void RemoveItem(int index) {
            CPP.Add("$q->removeItem(index);");
        }
        public int Count() {
            return CPP.ReturnInt("$q->count()");
        }
        public String GetSelectedText() {
            CPP.Add("std::shared_ptr<String> text;");
            CPP.Add("text.reset(new String($q->currentText()));");
            return CPP.ReturnString("text");
        }
        public int GetSelectedIndex() {
            return CPP.ReturnInt("$q->currentIndex()");
        }
    }
}
