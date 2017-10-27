using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: QScreen *$q = nullptr;\r\n" +
        "public: Screen(QScreen *s) {$q = s;}"
    )]
    [CPPOmitConstructors()]
    public class Screen {
        public int RefreshRate() {
            return CPP.ReturnInt("$q->refreshRate()");
        }
    }
}
