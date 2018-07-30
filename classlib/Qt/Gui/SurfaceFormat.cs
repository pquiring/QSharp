using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "std::qt_ptr<QSurfaceFormat> $q;" +
        "QSurfaceFormat* $value() {return $q.get();}"
    )]
    public class SurfaceFormat {
        public SurfaceFormat() {
            CPP.Add("$q = new QSurfaceFormat();");
        }
        public void SetSamples(int samples) {
            CPP.Add("$q->setSamples(samples);");
        }
    }
}
