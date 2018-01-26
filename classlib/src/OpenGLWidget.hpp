namespace Qt::Gui {

class $QOpenGLWidget : public QOpenGLWidget {
  private: OpenGLWidget *widget;
  public: $QOpenGLWidget(OpenGLWidget *widget) {
    this->widget = widget;
  }
  public: void initializeGL() {widget->InitializeGL();}
  public: void paintGL() {widget->PaintGL();}
  public: void resizeGL(int x, int y) {widget->ResizeGL(x, y);}
};

}
