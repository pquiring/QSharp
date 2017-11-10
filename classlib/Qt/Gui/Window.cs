using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QObject")]  //for eventFilter()
    [CPPClass(
        "private: QWindow *$q = nullptr;" +
        "private: bool $del = false;" +
        "private: std::shared_ptr<Qt::Gui::Screen> screen_ptr;" +
        "private: void $$init() {screen_ptr = std::make_shared<Qt::Gui::Screen>($q->screen()); $q->installEventFilter(this);}" +
        "public: Window() {$q = new QWindow(); $del = true; $$init();}" +
        "public: Window(QWindow *$d) {$q = $d; $del = false; $$init();}" +
        "public: bool eventFilter(QObject *obj, QEvent *event);"
    )]
    public class Window : OpenGLFunctions {
        /*
          Since C# does not support multiple-inheritance and OpenGLWindow needs Window and OpenGLFunctions
          therefore Window must derive from OpenGLFunctions to make it available to OpenGLWindow
        */
        public virtual void KeyPressed(Key key) {}
        public virtual void KeyReleased(Key key) {}
        public virtual void KeyTyped(char key) {}
        public virtual void MousePressed(int x, int y, int button) {}
        public virtual void MouseReleased(int x, int y, int button) {}
        public virtual void MouseMoved(int x, int y, int button) {}
        public virtual void MouseWheel(int x, int y) {}
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
