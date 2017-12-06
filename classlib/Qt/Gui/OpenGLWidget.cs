using Qt.Core;
using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QOpenGLWidget")]
    [CPPClass(
        "public: void initializeGL() {InitializeGL();}" +
        "public: void paintGL() {PaintGL();}" +
        "public: void resizeGL(int x, int y) {ResizeGL(x, y);}"
    )]
    public class OpenGLWidget : Widget {
        public OpenGLWidget() : base(Derived.derived) {
            CPP.Add("Widget::$base(this);");
        }
        /** This function is called during window creation. */
        public virtual void InitializeGL() { }
        /** This function is called to print the window.  This is where gl..() functions should be called. */
        public virtual void PaintGL() { }
        /** This function is called when the window is resized. */
        public virtual void ResizeGL(int w, int h) { }

        public void Update() {
            CPP.Add("update();");
        }
    }
}