namespace Qt::Gui {
  bool NativeWindow::eventFilter(QObject* obj, QEvent* event)
  {
    Q_UNUSED(obj);
    if (events == nullptr) return false;
    if (event->type() == QEvent::KeyPress) {
      QKeyEvent* keyEvent = (QKeyEvent*)(event);
      events->KeyPressed((KeyCode)keyEvent->key());
      QString txt = keyEvent->text();
      if (txt.length() > 0) {
        events->KeyTyped(txt.at(0).toLatin1());
      }
      return true;
    } else if (event->type() == QEvent::KeyRelease) {
      QKeyEvent* keyEvent = (QKeyEvent*)(event);
      events->KeyReleased((KeyCode)keyEvent->key());
      return true;
    } else if (event->type() == QEvent::MouseButtonPress) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      events->MousePressed(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
      return true;
    } else if (event->type() == QEvent::MouseButtonRelease) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      events->MouseReleased(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
      return true;
    } else if (event->type() == QEvent::MouseMove) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      events->MouseMoved(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
      return true;
    } else if (event->type() == QEvent::Wheel) {
      QWheelEvent* mouseEvent = (QWheelEvent*)(event);
      events->MouseWheel(mouseEvent->x(),mouseEvent->y());
      return true;
    } else {
      return false;
    }
  }
}
