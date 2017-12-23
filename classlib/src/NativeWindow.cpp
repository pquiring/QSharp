namespace Qt::Gui {
  bool NativeWindow::eventFilter(QObject* obj, QEvent* event)
  {
    Q_UNUSED(obj);
    bool discard = false;
    if (events == nullptr) return false;
    if (event->type() == QEvent::KeyPress) {
      QKeyEvent* keyEvent = (QKeyEvent*)(event);
      discard = events->KeyPressed((KeyCode)keyEvent->key());
      QString txt = keyEvent->text();
      if (txt.length() > 0) {
        discard = events->KeyTyped(txt.at(0).toLatin1());
      }
    } else if (event->type() == QEvent::KeyRelease) {
      QKeyEvent* keyEvent = (QKeyEvent*)(event);
      discard = events->KeyReleased((KeyCode)keyEvent->key());
    } else if (event->type() == QEvent::MouseButtonPress) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      discard = events->MousePressed(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
    } else if (event->type() == QEvent::MouseButtonRelease) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      discard = events->MouseReleased(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
    } else if (event->type() == QEvent::MouseMove) {
      QMouseEvent* mouseEvent = (QMouseEvent*)(event);
      discard = events->MouseMoved(mouseEvent->x(),mouseEvent->y(),mouseEvent->button());
    } else if (event->type() == QEvent::Wheel) {
      QWheelEvent* mouseEvent = (QWheelEvent*)(event);
      discard = events->MouseWheel(mouseEvent->x(),mouseEvent->y());
    }
    return discard;
  }
}
