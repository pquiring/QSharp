using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public delegate void ChangedEvent();
    [CPPClass(
        "private: QListWidget *$q;" +
        "public: void $base(QListWidget *$d) {$q = $d; Widget::$base($q);}"
    )]
    public abstract class ListWidget : Widget {
        protected ListWidget(Derived derived) : base(Derived.derived) {}
        public ListWidget() {
            CPP.Add("$q = new QListWidget();");
        }
        public void AddItem(String text) {
            CPP.Add("$q->addItem(text->qstring());");
        }
        public void InsertItem(int index, String text) {
            CPP.Add("$q->insertItem(index, text->qstring());");
        }
        public void RemoveItem(int index) {
            CPP.Add("$q->removeItemWidget($q->item(index));");
        }
        public int Count() {
            return CPP.ReturnInt("$q->count()");
        }
        public String GetSelectedItem() {
            CPP.Add("std::shared_ptr<String> text;");
            CPP.Add("text.reset(new String($q->currentItem()->text()));");
            return CPP.ReturnString("text");
        }
        public int GetSelectedIndex() {
            return CPP.ReturnInt("$q->currentRow()");
        }
        public String[] GetSelectedItems() {
            CPP.Add("QList<QListWidgetItem*> list = $q->selectedItems();");
            CPP.Add("std::shared_ptr<QVector<std::shared_ptr<String>>> array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("for(int i=0;i<cnt;i++) {array->append(std::make_shared<String>(list[i]->text()));}");
            return (String[])CPP.ReturnObject("array");
        }
        public int[] GetSelectedIndexes() {
            CPP.Add("QModelIndexList list = $q->selectionModel()->selectedIndexes();");
            CPP.Add("std::shared_ptr<QVector<int>> array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("for(int i=0;i<cnt;i++) {array->append(list[i].row());}");
            return (int[])CPP.ReturnObject("array");
        }
        public void SelectAll() {
            CPP.Add("$q->selectAll();");
        }

        private ChangedEvent changed;
        private void SlotChanged() {
            if (changed != null) changed();
        }
        public void OnRejected(ChangedEvent changed) {
            this.changed = changed;
            CPP.Add("$q->connect($q, &QListWidget::itemSelectionChanged, this, &ListWidget::SlotChanged);");
        }
    }
}
