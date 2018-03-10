using Qt.Core;

namespace Qt.Gui {

public class OpenGLUVMap : OpenGLFunctions {
    public Array<float> uvl;    //texture coords list (UV)
    public String name;
    public int textureIndex = -1;
    public bool texloaded = false;
    public int uvb = -1;    //GL buffer
    public int idx;    //map index

    public OpenGLUVMap(int idx) {
        InitializeOpenGLFunctions();
        uvl = new Array<float>();
        this.idx = idx;
        if (idx > 1) {
            Console.WriteLine("UVMap:Warning:More than 2 UVMaps not supported");
        }
    }

    public void Add(float[] uv) {
        uvl.Add(uv);
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
        glVertexAttribPointer(scene.tca[idx], 2, GL_FLOAT, GL_FALSE, 0);
    }
}

}
