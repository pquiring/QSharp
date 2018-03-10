namespace Qt.Gui {

/** Stores a translation (movement). */

public class OpenGLTranslate {
    public float x, y, z;
    public OpenGLTranslate() {}
    public OpenGLTranslate(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public void Set(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public void Add(float x, float y, float z) {
        this.x += x;
        this.y += y;
        this.z += z;
    }
}

}
