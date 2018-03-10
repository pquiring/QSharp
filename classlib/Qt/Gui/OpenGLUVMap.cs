using Qt.Core;

namespace Qt.Gui {

public class OpenGLUVMap : OpenGLFunctions {
    private Array<float> uvl;    //texture coords list (UV)
    private String name;
    private int textureIndex = -1;
    private bool loaded = false;
    private int uvb = -1;    //GL buffer
    private int mapIndex;

    public OpenGLUVMap(int idx) {
        InitializeOpenGLFunctions();
        uvl = new Array<float>();
        this.mapIndex = idx;
        if (idx > 1) {
            Console.WriteLine("UVMap:Warning:More than 2 UVMaps not supported");
        }
    }

    public int GetUVCount() {
        return uvl.Size() / 2;
    }

    public void SetName(String name) {
        this.name = name;
    }

    public String GetName() {
        return name;
    }

    public int GetTextureIndex() {
        return textureIndex;
    }
    public void SetTextureIndex(int index) {
        this.textureIndex = index;
    }

    public int GetMapIndex() {
        return mapIndex;
    }
    public void SetMapIndex(int index) {
        this.mapIndex = index;
    }

    public bool IsLoaded() {
        return loaded;
    }
    public void SetLoaded(bool loaded) {
        this.loaded = loaded;
    }

    public void Add(float uv) {
        uvl.Add(uv);
    }

    public void Add(float[] uv) {
        uvl.Add(uv);
    }

    public float[] GetBuffer() {
        return uvl.ToArray();
    }

    public void CopyBuffers() {
        int[] ids = new int[1];
        if (uvb == -1) {
            glGenBuffers(1, ids);
            uvb = ids[0];
        }
        glBindBuffer(GL_ARRAY_BUFFER, uvb);
        glBufferData(GL_ARRAY_BUFFER, uvl.Size() * 4, uvl.ToArray(), GL_STATIC_DRAW);
    }

    public void BindBuffers(OpenGLScene scene) {
        glBindBuffer(GL_ARRAY_BUFFER, uvb);
        glVertexAttribPointer(scene.tca[mapIndex], 2, GL_FLOAT, GL_FALSE, 0);
    }
}

}
