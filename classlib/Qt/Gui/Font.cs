using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum FontWeight {Light, Normal, DemiBold, Bold, Black}
    [CPPClass(
        "public: std::shared_ptr<QFont> $q;"
    )]
    public class Font {
        public Font(String family, int pointSize = -1, FontWeight weight = FontWeight.Normal, bool italic = false) {
            CPP.Add("$q = std::make_shared<QFont>(family->qstring(), pointSize, (QFont::Weight)weight, italic);");
        }
    }
}
