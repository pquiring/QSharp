using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "std::shared_ptr<QSurfaceFormat> $q;" +
        "QSurfaceFormat* $value() {return $q.get();}"
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
