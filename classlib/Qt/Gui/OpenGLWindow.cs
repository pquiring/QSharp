using Qt.QSharp;

namespace Qt.Gui {

    [CPPExtends("QOpenGLWindow")]
    [CPPClass(
        "public: void initializeGL() {InitializeGL();}\r\n" +
        "public: void paintGL() {PaintGL();}\r\n" +
        "public: void paintOverGL() {PaintOverGL();}\r\n" +
        "public: void paintUnderGL() {PaintUnderGL();}\r\n" +
        "public: void resizeGL(int x, int y) {ResizeGL(x, y);}\r\n" +
        "public: OpenGLWindow() : Qt::Gui::Window(this) {}\r\n"
    )]

    public class OpenGLWindow : Window {
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
