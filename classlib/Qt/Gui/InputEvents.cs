namespace Qt.Gui {
    /** Input Events.
    *   Use with NativeWindow.OnInputEvents()
    *   Return: discard event?
    */
    public class InputEvents {
        public virtual bool KeyPressed(KeyCode key) {return false;}
        public virtual bool KeyReleased(KeyCode key) {return false;}
        public virtual bool KeyTyped(char key) {return false;}
        public virtual bool MousePressed(int x, int y, int button) {return false;}
        public virtual bool MouseReleased(int x, int y, int button) {return false;}
        public virtual bool MouseMoved(int x, int y, int button) {return false;}
        public virtual bool MouseWheel(int x, int y) {return false;}
    }
}
