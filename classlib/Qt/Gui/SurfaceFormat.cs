using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: std::shared_ptr<QSurfaceFormat> $q;" +
        "public: QSurfaceFormat* $value() {return $q.get();}"
    )]
    public class SurfaceFormat {
        public SurfaceFormat() {
            CPP.Add("$q = std::make_shared<QSurfaceFormat>();");
        }
        public void SetSamples(int samples) {
            CPP.Add("$q->setSamples(samples);");
        }
    }
}
