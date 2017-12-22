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
            CPP.Add("$q->addChild(item->$q);");
        }
        public void InsertChild(int index, TreeItem item) {
            CPP.Add("$q->insertChild(index, item->$q);");
        }
        public void RemoveChild(int index) {
            CPP.Add("$q->removeChild($q->child(index));");
        }
        public int GetChildCount() {
            return CPP.ReturnInt("$q->childCount()");
        }
        public TreeItem GetChild(int index) {
            return (TreeItem)CPP.ReturnObject("TreeItem::$new($q->child(index))");
        }
        public TreeItem GetParent() {
            return (TreeItem)CPP.ReturnObject("TreeItem::$new($q->parent())");
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
            return (TreeItem)CPP.ReturnObject("TreeItem::$new($q->currentItem())");
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
