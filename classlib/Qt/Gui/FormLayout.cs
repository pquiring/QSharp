using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "QFormLayout *$q;" +
        "void $base(QFormLayout *$d) {$q = $d; Layout::$base($q);}"
    )]
    public class FormLayout : Layout {
        protected FormLayout(QSharpDerived derived) {}
        public FormLayout() {
            CPP.Add("$q = new QFormLayout();");
            CPP.Add("Layout::$base($q);");
        }
        public void AddRow(Widget label, Widget field) {
            CPP.Add("$q->addRow($check(label)->$q, $check(field)->$q);");
        }
        public void AddRow(Widget label, Layout field) {
            CPP.Add("$q->addRow($check(label)->$q, $check(field)->$q);");
        }
        public void AddRow(String label, Widget field) {
            CPP.Add("$q->addRow($check(label)->qstring(), $check(field)->$q);");
        }
        public void AddRow(String label, Layout field) {
            CPP.Add("$q->addRow($check(label)->qstring(), $check(field)->$q);");
        }
        public void AddRow(Widget label) {
            CPP.Add("$q->addRow($check(label)->$q);");
        }
        public void AddRow(Layout label) {
            CPP.Add("$q->addRow($check(label)->$q);");
        }

        public void InsertRow(int row, Widget label, Widget field) {
            CPP.Add("$q->insertRow(row, $check(label)->$q, $check(field)->$q);");
        }
        public void InsertRow(int row, Widget label, Layout field) {
            CPP.Add("$q->insertRow(row, $check(label)->$q, $check(field)->$q);");
        }
        public void InsertRow(int row, String label, Widget field) {
            CPP.Add("$q->insertRow(row, $check(label)->qstring(), $check(field)->$q);");
        }
        public void InsertRow(int row, String label, Layout field) {
            CPP.Add("$q->insertRow(row, $check(label)->qstring(), $check(field)->$q);");
        }
        public void InsertRow(int row, Widget label) {
            CPP.Add("$q->insertRow(row, $check(label)->$q);");
        }
        public void InsertRow(int row, Layout label) {
            CPP.Add("$q->insertRow(row, $check(label)->$q);");
        }
    }
}
