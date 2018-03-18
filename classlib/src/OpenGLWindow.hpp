namespace Qt { namespace Gui {

class $QOpenGLWindow : public QOpenGLWindow {
  private: OpenGLWindow *window;
  public: $QOpenGLWindow(OpenGLWindow *window) {
    this->window = window;
  }
  public: void initializeGL() {window->InitializeGL();}
  public: void paintGL() {window->PaintGL();}
  public: void resizeGL(int x, int y) {window->ResizeGL(x, y);}
};

} }  //namespace Qt::Gui
