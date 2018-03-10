using Qt.Core;

namespace Qt.Gui {

public class OpenGLModel : Object {
    public ArrayList<OpenGLObject> ol;    //obj list
    public ArrayList<String> textures;
    public Matrix4x4 m;    //translation, rotation, scale matrix for all sub-objects
    public bool visible = true;
    public int refcnt;

    public OpenGLModel() {
        m = new Matrix4x4();
        m.SetIdentity();
        ol = new ArrayList<OpenGLObject>();
        textures = new ArrayList<String>();
    }
    private OpenGLModel(Matrix4x4 m) {    //for clone()
        this.m = m;
        ol = new ArrayList<OpenGLObject>();
    }
    /**
    * Clones deep enough so that the cloned object will include seperate GLObjects, but share vertex, vertex point,
    * and animation data (except for the frame position).
    */
    public Object Clone() {
        OpenGLModel c = new OpenGLModel((Matrix4x4)m.Clone());
        int objs = ol.Size();
        for(int a=0;a<objs;a++) c.ol.Add((OpenGLObject)ol.Get(a).Clone());
        c.textures = textures;
        return (Object)c;
    }
    public void SetVisible(bool state) {visible = state;}
    public void AddObject(OpenGLObject obj) {
        ol.Add(obj);
    }
    public OpenGLObject GetObject(String name) {
        for(int a=0;a<ol.Size();a++) {
            OpenGLObject o = ol.Get(a);
            if (o.name.Equals(name)) {
                return o;
            }
        }
        Console.WriteLine("OpenGLModel:Could not find object:" + name);
        return null;
    }
    public void SetIdentity() {
        m.SetIdentity();
    }
    //these are additive
    public void Rotate(float angle, float x, float y, float z) {
        m.Rotate(angle, x, y, z);
    }
    public void Translate(float x, float y, float z) {
        m.Translate(x, y, z);
    }
    public void Scale(float x, float y, float z) {
        m.Scale(x, y, z);
    }
    public void NextFrame() {
        OpenGLObject obj;
        int size = ol.Size();
        for(int i=0;i<size;i++) {
            obj = ol.Get(i);
            obj.NextFrame();
        }
    }
    public void SetFrame(int idx) {
        OpenGLObject obj;
        int size = ol.Size();
        for(int i=0;i<size;i++) {
            obj = ol.Get(i);
            obj.SetFrame(idx);
        }
    }
    /** Adds a texture filename and returns index. */
    public int AddTexture(String fn) {
        for(int a=0;a<textures.Size();a++) {
            if (textures.Get(a).Equals(fn)) return a;
        }
        textures.Add(fn);
        return textures.Size() - 1;
    }
    public String GetTexture(int idx) {
        return textures.Get(idx);
    }
/*
    public void Print() {
        Console.WriteLine("Model data");
        for(int a=0;a<ol.Size();a++) {
            ol.Get(a).Print(this);
        }
    }
*/
}

}
