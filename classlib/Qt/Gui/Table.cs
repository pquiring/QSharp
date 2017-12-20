using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: QTableWidget *$q;" +
        "public: void $base(QTableWidget *$d) {$q = $d; Widget::$base($q);}"
    )]
    public class Table : Widget {
        protected Table(QSharpDerived derived) : base(QSharpDerived.derived) {}
        public Table(int rows, int columns) {
            CPP.Add("$q = new QTableWidget(rows, columns);");
            CPP.Add("Widget::$base($q);");
        }
        public void AddRow() {
            CPP.Add("int row = $q->rowCount();");
            CPP.Add("$q->setRowCount(row+1);");
        }
        public void AddRow(String[] text) {
            CPP.Add("int row = $q->rowCount();");
            CPP.Add("$q->setRowCount(row+1);");
            for(int col=0;col<text.Length;col++) {
                CPP.Add("$q->setItem(row, col, new QTableWidgetItem(text->at(col)->qstring()));");
            }
        }
        public void InsertRow(int row, String[] text) {
            CPP.Add("$q->insertRow(row);");
            for(int col=0;col<text.Length;col++) {
                CPP.Add("$q->setItem(row, col, new QTableWidgetItem(text->at(col)->qstring()));");
            }
        }
        public void RemoveRow(int index) {
            CPP.Add("$q->removeRow(index);");
        }
        public int RowCount() {
            return CPP.ReturnInt("$q->rowCount()");
        }
        public int ColumnCount() {
            return CPP.ReturnInt("$q->columnCount()");
        }
        public void AddColumn() {
            CPP.Add("int col = $q->columnCount();");
            CPP.Add("$q->setColumnCount(col+1);");
        }
        public void RemoveColumn(int index) {
            CPP.Add("$q->removeColumn(index);");
        }
        public String GetData(int row, int column) {
            return CPP.ReturnString("String::$new($q->item(row, column)->text())");
        }
        public void SetData(int row, int column, String data) {
            CPP.Add("$q->setItem(row, column, new QTableWidgetItem(data->qstring()));");
        }
        public String GetSelectedItem() {
            return CPP.ReturnString("String::$new($q->currentItem()->text())");
        }
        public int GetSelectedRow() {
            return CPP.ReturnInt("$q->currentRow()");
        }
        public int GetSelectedColumn() {
            return CPP.ReturnInt("$q->currentColumn()");
        }
        public String[] GetSelectedItems() {
            CPP.Add("QList<QTableWidgetItem*> list = $q->selectedItems();");
            CPP.Add("std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<String>>> array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("array = Qt::QSharp::FixedArray<std::shared_ptr<String>>::$new(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {array->at(idx) = String::$new(list[idx]->text());}");
            return (String[])CPP.ReturnObject("array");
        }
        public int[][] GetSelectedIndexes() {
            CPP.Add("QModelIndexList list = $q->selectionModel()->selectedIndexes();");
            CPP.Add("std::shared_ptr<Qt::QSharp::FixedArray<std::shared_ptr<Qt::QSharp::FixedArray<int32>>>> array;");
            CPP.Add("int cnt = list.count();");
            CPP.Add("array = Qt::QSharp::FixedArray<std::shared_ptr<Qt::QSharp::FixedArray<int32>>>::$new(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {");
            CPP.Add("  std::shared_ptr<Qt::QSharp::FixedArray<int>> row;");
            CPP.Add("  row = Qt::QSharp::FixedArray<int32>::$new(2);");
            CPP.Add("  row->at(0) = list[idx].row();");
            CPP.Add("  row->at(1) = list[idx].column();");
            CPP.Add("  array->at(idx) = row;");
            CPP.Add("}");
            return (int[][])CPP.ReturnObject("array");
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
            CPP.Add("$q->connect($q, &QTableWidget::itemSelectionChanged, this, &Table::SlotChanged);");
        }
    }
}
