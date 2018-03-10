namespace Qt.Gui {

/** Stores scale. */

public class OpenGLScale {
    public float x, y, z;
    public OpenGLScale() {}
    public OpenGLScale(float x, float y, float z) {
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
        this.x *= x;
        this.y *= y;
        this.z *= z;
    }
};

}
