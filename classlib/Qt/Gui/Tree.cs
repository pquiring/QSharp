using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QTreeWidgetItem *$q;" +
        "public: void $base(QTreeWidgetItem *$d) {$q = $d;}"
    )]
    public class TreeItem {
        public TreeItem() {
            CPP.Add("$q = nullptr;");
        }
        public TreeItem(String text) {
            CPP.Add("$q = new QTreeWidgetItem(QStringList(text->qstring()));");
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
            CPP.Add("std::shared_ptr<TreeItem> child = std::make_shared<TreeItem>();");
            CPP.Add("child->$base($q->child(index));");
            return (TreeItem)CPP.ReturnObject("child");
        }
        public TreeItem GetParent() {
            CPP.Add("std::shared_ptr<TreeItem> parent = std::make_shared<TreeItem>();");
            CPP.Add("parent->$base($q->parent());");
            return (TreeItem)CPP.ReturnObject("parent");
        }
    }
    [CPPClass(
        "private: QTreeWidget *$q;" +
        "public: void $base(QTreeWidget *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class Tree : Widget {
        protected Tree(Derived derived) : base(Derived.derived) {}
        public Tree() {
            CPP.Add("$q = new QTreeWidget();");
            CPP.Add("Widget::$base($q);");
        }
        public TreeItem GetRoot() {
            CPP.Add("std::shared_ptr<TreeItem> root = std::make_shared<TreeItem>();");
            CPP.Add("root->$base($q->invisibleRootItem());");
            return (TreeItem)CPP.ReturnObject("root");
        }
        public TreeItem GetSelectedItem() {
            CPP.Add("std::shared_ptr<TreeItem> item = std::make_shared<TreeItem>();");
            CPP.Add("item->$base($q->currentItem());");
            return (TreeItem)CPP.ReturnObject("item");
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
