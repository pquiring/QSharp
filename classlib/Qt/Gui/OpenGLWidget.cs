using Qt.Core;
using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: $QOpenGLWidget *$q;"
    )]
    public class OpenGLWidget : Widget {
        public OpenGLWidget() : base(QSharpDerived.derived) {
            CPP.Add("$q = new $QOpenGLWidget(this);");
            CPP.Add("Widget::$base($q);");
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
