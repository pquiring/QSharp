using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QGridLayout *$q;" +
        "public: void $base(QGridLayout *$d) {$q = $d; Layout::$base($q);}"
    )]
    public class GridLayout : Layout {
        protected GridLayout(Derived derived) {}
        public GridLayout() {
            CPP.Add("$q = new QGridLayout();");
            CPP.Add("Layout::$base($q);");
        }
        public void AddLayout(Layout layout, int row, int column, Alignment alignment = Alignment.AlignDefault) {
            CPP.Add("$q->addLayout(layout->$q, row, column, (Qt::Alignment)alignment);");
        }
        public void AddLayout(Layout layout, int row, int column, int rowSpan, int columnSpan, Alignment alignment = Alignment.AlignDefault) {
            CPP.Add("$q->addLayout(layout->$q, row, column, rowSpan, columnSpan, (Qt::Alignment)alignment);");
        }
        public void AddWidget(Widget widget, int row, int column, Alignment alignment = Alignment.AlignDefault) {
            CPP.Add("$q->addWidget(widget->$q, row, column, (Qt::Alignment)alignment);");
        }
        public void AddWidget(Widget widget, int row, int column, int rowSpan, int columnSpan, Alignment alignment = Alignment.AlignDefault) {
            CPP.Add("$q->addWidget(widget->$q, row, column, rowSpan, columnSpan, (Qt::Alignment)alignment);");
        }
    }
}
