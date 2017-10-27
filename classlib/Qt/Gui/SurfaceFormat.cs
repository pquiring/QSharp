using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QSurfaceFormat")]
    public class SurfaceFormat {
        public void SetSamples(int samples) {
            CPP.Add("setSamples(samples);");
        }
    }
}
