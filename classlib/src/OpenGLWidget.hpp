namespace Qt { namespace Gui {

struct $QOpenGLWidget : public QOpenGLWidget {
  OpenGLWidget *widget;
  $QOpenGLWidget(OpenGLWidget *widget) {
    this->widget = widget;
  }
  void initializeGL() {widget->InitializeGL();}
  void paintGL() {widget->PaintGL();}
  void resizeGL(int x, int y) {widget->ResizeGL(x, y);}
};

} } //namespace Qt::Gui
