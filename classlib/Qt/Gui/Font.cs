using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPEnum("QFont::Weight")]
    public enum FontWeight {Light, Normal, DemiBold, Bold, Black}
    [CPPClass(
        "public: std::shared_ptr<QFont> $q;"
    )]
    public class Font {
        [CPPReplaceArgs("QFont $f")]
        private Font(NativeArg1 arg) {
            CPP.Add("$q = std::make_shared<QFont>($f)");
        }
        public Font(String family, int pointSize = -1, FontWeight weight = FontWeight.Normal, bool italic = false) {
            CPP.Add("$q = std::make_shared<QFont>($check(family)->qstring(), pointSize, (QFont::Weight)weight, italic);");
        }
    }
}
