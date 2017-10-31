namespace Qt::Gui {
  bool Window::eventFilter(QObject* obj, QEvent* event)
  {
    Q_UNUSED(obj);
    if (event->type() == QEvent::KeyPress) {
      QKeyEvent* keyEvent = (QKeyEvent*)(event);
      KeyPressed((Key)keyEvent->key());
      QString txt = keyEvent->text();
      if (txt.length() > 0) {
        KeyTyped(txt.at(0).toLatin1());
      }
      return true;
    } else if (event->type() == QEvent::KeyRelease) {
      QKeyEvent* keyEvent = (QKeyEvent*)(event);
      KeyReleased((Key)keyEvent->key());
      return true;
    } else if (event->type() == QEvent::MouseButtonPress) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      MousePressed(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
      return true;
    } else if (event->type() == QEvent::MouseButtonRelease) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      MouseReleased(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
      return true;
    } else if (event->type() == QEvent::MouseMove) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      MouseMoved(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
      return true;
    } else if (event->type() == QEvent::Wheel) {
      QWheelEvent* mouseEvent = (QWheelEvent*)(event);
      MouseWheel(mouseEvent->x(),mouseEvent->y());
      return true;
    } else {
      return false;
    }
  }
}
