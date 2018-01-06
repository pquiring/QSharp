using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QTreeWidgetItem *$q;" +
        "public: QTreeWidgetItem* $value() {return $q;}"
    )]
    public class TreeItem {
        public TreeItem(String text) {
            CPP.Add("$q = new QTreeWidgetItem(QStringList(text->qstring()));");
        }
        [CPPReplaceArgs("QTreeWidgetItem *$a")]
        private TreeItem(NativeArg1 arg) {
            CPP.Add("$q = $a");
        }
        public void AddChild(TreeItem item) {
            CPP.Add("$q->addChild($check(item)->$q);");
        }
        public void InsertChild(int index, TreeItem item) {
            CPP.Add("$q->insertChild(index, $check(item)->$q);");
        }
        public void RemoveChild(TreeItem item) {
            CPP.Add("$q->removeChild($check(item)->$q);");
        }
        public void RemoveChild(int index) {
            CPP.Add("$q->removeChild($q->child(index));");
        }
        public int GetChildCount() {
            return CPP.ReturnInt("$q->childCount()");
        }
        public TreeItem GetChild(int index) {
            CPP.Add("QTreeWidgetItem *item = $q->child(index);");
            return (TreeItem)CPP.ReturnObject("item == nullptr ? nullptr : TreeItem::$new(item)");
        }
        public TreeItem GetParent() {
            CPP.Add("QTreeWidgetItem *item = $q->parent();");
            return (TreeItem)CPP.ReturnObject("item == nullptr ? nullptr : TreeItem::$new(item)");
        }
        public String GetName() {
            return CPP.ReturnString("String::$new($q->text(0))");
        }
        public void SetName(String text) {
            CPP.Add("$q->setText(0, $check(text)->qstring());");
        }
    }
    [CPPClass(
        "private: QTreeWidget *$q;" +
        "public: void $base(QTreeWidget *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class Tree : Widget {
        protected Tree(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public Tree() {
            CPP.Add("$q = new QTreeWidget();");
            CPP.Add("Widget::$base($q);");
        }
        public TreeItem GetRoot() {
            return (TreeItem)CPP.ReturnObject("TreeItem::$new($q->invisibleRootItem())");
        }
        public TreeItem GetSelectedItem() {
            CPP.Add("QTreeWidgetItem *item = $q->currentItem();");
            return (TreeItem)CPP.ReturnObject("item == nullptr ? nullptr : TreeItem::$new(item)");
        }
        public void ExpandItem(TreeItem item) {
            CPP.Add("$q->expandItem(item->$value());");
        }
        public void CollapseItem(TreeItem item) {
            CPP.Add("$q->collapseItem(item->$value());");
        }
        public void ScrollToItem(TreeItem item) {
            CPP.Add("$q->scrollToItem(item->$value());");
        }
        public void Clear() {
            CPP.Add("$q->clear();");
        }
        public bool IsHeaderHidden() {
            return CPP.ReturnBool("$q->isHeaderHidden()");
        }
        public void SetHeaderHidden(bool hidden) {
            CPP.Add("$q->setHeaderHidden(hidden);");
        }

        private ChangedEvent changed;
        private void SlotChanged() {
            if (changed != null) changed();
        }
        public void OnChanged(ChangedEvent changed) {
            this.changed = changed;
            CPP.Add("$q->connect($q, &QTreeWidget::itemSelectionChanged, this, &Tree::SlotChanged);");
        }
    }
}
