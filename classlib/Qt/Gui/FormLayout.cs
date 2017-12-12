using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QFormLayout *$q;" +
        "public: void $base(QFormLayout *$d) {$q = $d; Layout::$base($q);}"
    )]
    public class FormLayout : Layout {
        protected FormLayout(QSharpDerived derived) {}
        public FormLayout() {
            CPP.Add("$q = new QFormLayout();");
            CPP.Add("Layout::$base($q);");
        }
        public void AddRow(Widget label, Widget field) {
            CPP.Add("$q->addRow(label->$q, field->$q);");
        }
        public void AddRow(Widget label, Layout field) {
            CPP.Add("$q->addRow(label->$q, field->$q);");
        }
        public void AddRow(String label, Widget field) {
            CPP.Add("$q->addRow(label->qstring(), field->$q);");
        }
        public void AddRow(String label, Layout field) {
            CPP.Add("$q->addRow(label->qstring(), field->$q);");
        }
        public void AddRow(Widget label) {
            CPP.Add("$q->addRow(label->$q);");
        }
        public void AddRow(Layout label) {
            CPP.Add("$q->addRow(label->$q);");
        }

        public void InsertRow(int row, Widget label, Widget field) {
            CPP.Add("$q->insertRow(row, label->$q, field->$q);");
        }
        public void InsertRow(int row, Widget label, Layout field) {
            CPP.Add("$q->insertRow(row, label->$q, field->$q);");
        }
        public void InsertRow(int row, String label, Widget field) {
            CPP.Add("$q->insertRow(row, label->qstring(), field->$q);");
        }
        public void InsertRow(int row, String label, Layout field) {
            CPP.Add("$q->insertRow(row, label->qstring(), field->$q);");
        }
        public void InsertRow(int row, Widget label) {
            CPP.Add("$q->insertRow(row, label->$q);");
        }
        public void InsertRow(int row, Layout label) {
            CPP.Add("$q->insertRow(row, label->$q);");
        }
    }
}
