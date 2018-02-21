using Qt.QSharp;

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
        "public: QLayout *$q;" +
        "public: void $base(QLayout *$d) {$q = $d;}"
    )]
    public abstract class Layout {
        public void AddWidget(Widget widget) {
            CPP.Add("$q->addWidget($check(widget)->$q);");
        }
        public void RemoveWidget(Widget widget) {
            CPP.Add("$q->removeWidget($check(widget)->$q);");
        }
        public void SetEnabled(bool state) {
            CPP.Add("$q->setEnabled(state);");
        }
        public bool IsEnabled() {
            return CPP.ReturnBool("$q->isEnabled();");
        }
        public void AddLayout(Layout layout) {
            CPP.Add("$q->addItem($check(layout)->$q);");
        }
        public void RemoveLayout(Layout layout) {
            CPP.Add("$q->removeItem($check(layout)->$q);");
        }
        public Alignment GetAlignment() {
            return (Alignment)CPP.ReturnInt("(int)$q->alignment()");
        }
        public void SetAlignment(Alignment alignment) {
            CPP.Add("$q->setAlignment((Qt::AlignmentFlag)alignment);");
        }
    }
}
