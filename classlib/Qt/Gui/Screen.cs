using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QScreen *$q = nullptr;"
    )]
    public class Screen {
        [CPPReplaceArgs("QScreen *$s")]
        private Screen(NativeArg1 arg) {
            CPP.Add("$q = $s;");
        }
        public int RefreshRate() {
            return CPP.ReturnInt("$q->refreshRate()");
        }
    }
}
