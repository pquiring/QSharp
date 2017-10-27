using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: QWindow *$q = nullptr;\r\n" +
        "private: bool $del = false;\r\n" +
        "private: std::shared_ptr<Qt::Gui::Screen> screen_ptr;\r\n" +
        "private: void $$init() {screen_ptr = std::make_shared<Qt::Gui::Screen>($q->screen());}\r\n" +
        "public: Window() {$q = new QWindow(); $$init(); $del = true;}\r\n" +
        "public: Window(QWindow *w) {$q = w; $$init(); $del = false;}\r\n"
    )]
    public class Window : OpenGLFunctions {
        /*
          Since C# does not support multiple-inheritance and OpenGLWindow needs Window and OpenGLFunctions
          therefore Window must derive from OpenGLFunctions to make it available to OpenGLWindow
        */
        public float DevicePixelRatio() {
            return CPP.ReturnFloat("$q->devicePixelRatio()");
        }
        public int Width() {
            return CPP.ReturnInt("$q->width()");
        }
        public int Height() {
            return CPP.ReturnInt("$q->height()");
        }
        public Screen GetScreen() {
            return (Screen)CPP.ReturnObject("screen_ptr");
        }
        public void SetFormat(SurfaceFormat format) {
            CPP.Add("$q->setFormat(*format.get());");
        }
        public void Show() {
            CPP.Add("$q->show();");
        }
        public void Resize(int x, int y) {
            CPP.Add("$q->resize(x, y);");
        }

        ~Window() {
            CPP.Add("if ($del) delete $q;");
        }
    }
}
