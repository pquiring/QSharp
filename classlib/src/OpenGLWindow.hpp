namespace Qt { namespace Gui {

struct $QOpenGLWindow : public QOpenGLWindow {
  OpenGLWindow *window;
  $QOpenGLWindow(OpenGLWindow *window) {
    this->window = window;
  }
  void initializeGL() {window->InitializeGL();}
  void paintGL() {window->PaintGL();}
  void resizeGL(int x, int y) {window->ResizeGL(x, y);}
};

} }  //namespace Qt::Gui
