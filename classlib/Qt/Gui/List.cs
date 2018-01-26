using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QListWidget *$q;" +
        "public: void $base(QListWidget *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class List : Widget {
        protected List(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public List() {
            CPP.Add("$q = new QListWidget();");
            CPP.Add("Widget::$base($q);");
        }
        public void AddItem(String text) {
            CPP.Add("$q->addItem($check(text)->qstring());");
        }
        public void InsertItem(int index, String text) {
            CPP.Add("$q->insertItem(index, $check(text)->qstring());");
        }
        public void RemoveItem(int index) {
            CPP.Add("$q->removeItemWidget($q->item(index));");
        }
        public int Count() {
            return CPP.ReturnInt("$q->count()");
        }
        public String GetSelectedItem() {
            return CPP.ReturnString("String::$new($q->currentItem()->text())");
        }
        public int GetSelectedIndex() {
            return CPP.ReturnInt("$q->currentRow()");
        }
        public String[] GetSelectedItems() {
            CPP.Add("QList<QListWidgetItem*> list = $q->selectedItems();");
            CPP.Add("std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<String>>> array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("array = Qt::QSharp::FixedArray<std::shared_ptr<String>>::$new(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {array->at(idx) = String::$new(list[idx]->text());}");
            return (String[])CPP.ReturnObject("array");
        }
        public int[] GetSelectedIndexes() {
            CPP.Add("QModelIndexList list = $q->selectionModel()->selectedIndexes();");
            CPP.Add("std::shared_ptr<Qt::QSharp::FixedArray<int>> array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("array = Qt::QSharp::FixedArray<int>::$new(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {array->at(idx) = list[idx].row();}");
            return (int[])CPP.ReturnObject("array");
        }
        public void SelectAll() {
            CPP.Add("$q->selectAll();");
        }

        private ChangedEvent changed;
        private void SlotChanged() {
            if (changed != null) changed();
        }
        public void OnChanged(ChangedEvent changed) {
            this.changed = changed;
            CPP.Add("QObject::connect($q, &QListWidget::itemSelectionChanged, [=] () {this->SlotChanged();});");
        }
    }
}
