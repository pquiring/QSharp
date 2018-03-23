using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "$QOpenGLWindow *$q;"
    )]
    public class OpenGLWindow : NativeWindow {
        public OpenGLWindow() : base(QSharpDerived.derived) {
            CPP.Add("$q = new $QOpenGLWindow(this);");
            CPP.Add("NativeWindow::$base($q);");
        }
        /** This function is called during window creation. */
        public virtual void InitializeGL() { }
        /** This function is called to print the window.  This is where gl..() functions should be called. */
        public virtual void PaintGL() { }
        /** This function is called when the window is resized. */
        public virtual void ResizeGL(int w, int h) { }

        public void Update() {
            CPP.Add("$q->update();");
        }
    }
}
