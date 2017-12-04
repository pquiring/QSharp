using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QOpenGLWindow")]
    [CPPClass(
        "public: void initializeGL() {InitializeGL();}" +
        "public: void paintGL() {PaintGL();}" +
        "public: void paintOverGL() {PaintOverGL();}" +
        "public: void paintUnderGL() {PaintUnderGL();}" +
        "public: void resizeGL(int x, int y) {ResizeGL(x, y);}"
    )]

    public class OpenGLWindow : NativeWindow {
        public OpenGLWindow() : base(Derived.derived) {
            CPP.Add("NativeWindow::$base(this);");
        }
        /** This function is called during window creation. */
        public virtual void InitializeGL() { }
        /** This function is called to print the window.  This is where gl..() functions should be called. */
        public virtual void PaintGL() { }
        public virtual void PaintOverGL() { }
        public virtual void PaintUnderGL() { }
        /** This function is called when the window is resized. */
        public virtual void ResizeGL(int w, int h) { }

        public void Update() {
            CPP.Add("update();");
        }
    }
}
