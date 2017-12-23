using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QObject")]  //for eventFilter()
    [CPPClass(
        "private: QWindow *$q = nullptr;" +
        "public: void $base(QWindow *$b) {$q = $b; init();}" +
        "private: std::shared_ptr<Qt::Gui::Screen> screen_ptr;" +
        "public: bool eventFilter(QObject *obj, QEvent *event);"
    )]
    /** Window represents the native Window object. */
    public class NativeWindow : OpenGLFunctions {
        /*
          Since C# does not support multiple-inheritance and OpenGLWindow needs Window and OpenGLFunctions
          therefore Window must derive from OpenGLFunctions to make it available to OpenGLWindow
        */
        protected NativeWindow(QSharpDerived derived) {}
        [CPPReplaceArgs("QWindow *$w")]
        private NativeWindow(NativeArg1 arg) {
            CPP.Add("$q = $w; init();");
        }
        private InputEvents events;
        protected InputEvents GetInputEvents() {return events;}
        private void init() {
            CPP.Add("screen_ptr = Qt::Gui::Screen::$new($q->screen()); $q->installEventFilter(this);");
        }
        public void OnInputEvents(InputEvents events) {
            this.events = events;
        }
        public float DevicePixelRatio() {
            return CPP.ReturnFloat("$q->devicePixelRatio()");
        }
        public int GetWidth() {
            return CPP.ReturnInt("$q->width()");
        }
        public int GetHeight() {
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
        public void Hide() {
            CPP.Add("$q->hide();");
        }
        public void SetSize(int x, int y) {
            CPP.Add("$q->resize(x, y);");
        }
    }
}
