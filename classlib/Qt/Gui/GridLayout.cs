using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum Alignment {
        AlignDefault = 0,
        AlignLeft = 0x1, AlignRight = 0x2, AlignHCenter = 0x4, AlignJustify = 0x8,
        AlignTop = 0x20, AlignBottom = 0x40, AlignVCenter = 0x80, AlignBaseline = 0x100,
        AlignCenter = AlignVCenter | AlignHCenter,
        AlignAbsolute = 0x10, AlignLeading = AlignLeft, AlignTrailing = AlignRight,
        AlignHorizontal_Mask = AlignLeft | AlignRight | AlignHCenter | AlignJustify | AlignAbsolute,
        AlignVertical_Mask = AlignTop | AlignBottom | AlignVCenter | AlignBaseline
    }
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
