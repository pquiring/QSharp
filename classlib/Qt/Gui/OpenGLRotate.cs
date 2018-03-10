namespace Qt.Gui {

/** Stores angle-axis rotation. */

public class OpenGLRotate {
    public float angle, x, y, z;
    public OpenGLRotate() {}
    public OpenGLRotate(float angle, float x, float y, float z) {
        this.angle = angle;
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public void Set(float angle, float x, float y, float z) {
        this.angle = angle;
        this.x = x;
        this.y = y;
        this.z = z;
    }
/*
    public void Add(float angle, float x, float y, float z) {
        //TODO
    }
*/
}

}
