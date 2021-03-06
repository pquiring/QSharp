using Qt.Core;

namespace Qt.Gui {

public class OpenGLObject : OpenGLFunctions {
    //TODO : these public fields should be private
    private Array<float> vpl;    //vertex position list
    private Array<int> vil;    //vertex index list (poly)
    private int vpb = -1, vib = -1;    //GL Buffers

    private int type = GL_TRIANGLES;    //GL_TRIANGLES or GL_QUADS

    private List<OpenGLUVMap> maps = new List<OpenGLUVMap>();

    private bool visible = true;
//animation data
    private Map<int, OpenGLTranslate> tl;    //move list
    private Map<int, OpenGLRotate> rl;    //rotation list
    private Map<int, OpenGLScale> sl;    //scale list
    private int frameIndex;
    private Matrix4x4 m;    //current rotation, translation, scale
    private float[] color;    //RGBA (default = 1.0f,1.0f,1.0f,1.0f)
    private Vertex org;    //origin (default = 0.0f,0.0f,0.0f)
    private String name;
    private int parent;    //a 3ds file field (not used)
    private int maxframeCount;
    public OpenGLObject() {
        frameIndex = 0;
        vpl = new Array<float>();
        vil = new Array<int>();
        tl = new Map<int, OpenGLTranslate>((int k1, int k2) => {return 0;});
        rl = new Map<int, OpenGLRotate>((int k1, int k2) => {return 0;});
        sl = new Map<int, OpenGLScale>((int k1, int k2) => {return 0;});
        color = new float[4];
        for(int a=0;a<4;a++) color[a] = 1.0f;
        visible = true;
        org = new Vertex();
        parent = -1;
        maxframeCount = 0;
        m = new Matrix4x4();
    }
    public void SetName(String name) {
        this.name = name;
    }
    public String GetName() {
        return name;
    }
    /** Get Polygone type (default = GL_TRIANGLES) */
    public new int GetType() {
        return type;
    }
    /** Set Polygon type (default = GL_TRIANGLES) */
    public void SetType(int type) {
        this.type = type;
    }
    public List<OpenGLUVMap> GetUVMaps() {
        return maps;
    }
    public Map<int, OpenGLTranslate> GetTranslateMap() {
        return tl;
    }
    public Map<int, OpenGLRotate> GetRotateMap() {
        return rl;
    }
    public Map<int, OpenGLScale> GetScaleMap() {
        return sl;
    }
    public int GetParent() {
        return parent;
    }
    public void SetParent(int parent) {
        this.parent = parent;
    }
    public int GetMaxFrameCount() {
        return maxframeCount;
    }
    public void SetMaxFrameCount(int maxframeCount) {
        this.maxframeCount = maxframeCount;
    }
    public OpenGLObject Clone() {
        OpenGLObject cln = new OpenGLObject();
        cln.vpl = vpl;
        cln.vil = vil;
        cln.maps = maps;
        cln.visible = visible;
        cln.tl = tl;
        cln.rl = rl;
        cln.sl = sl;
        cln.frameIndex = frameIndex;
        cln.m = (Matrix4x4)m.Clone();
        cln.color = new float[4];
        for(int a=0;a<4;a++) cln.color[a] = color[a];
        cln.org = org;
        cln.parent = parent;
        cln.maxframeCount = maxframeCount;
        cln.type = type;
        return cln;
    }
    public Vertex GetOrigin() {
        return org;
    }
    public void SetVisible(bool state) {visible = state;}
    public void AddRotate(float angle, float x, float y, float z, Vertex org) {
        Matrix4x4 tmp = new Matrix4x4();
        //rotates relative to org
        tmp.SetAxisAngle(angle, x, y, z);    //set rotation
        tmp.Translate(org.x, org.y, org.z);    //set translation
        m.Multiply4x4(tmp);    //add it
        //now undo translation
        tmp.SetIdentity3x3();    //remove rotation
        tmp.ReverseTranslate();
        m.Multiply4x4(tmp);
    }
    public void AddTranslate(float x, float y, float z) {
        m.Translate(x,y,z);
    }
    public void AddScale(float x, float y, float z) {
        m.Scale(x,y,z);
    }
    public void SetFrame(int idx) {    //0=init state
        OpenGLRotate _r;
        OpenGLTranslate _t;
        OpenGLScale _s;
        frameIndex = idx;
        if (idx == 0) {
            m.SetIdentity();
            return;
        }
        _t = tl.Get(idx);
        if (_t != null) {
            AddTranslate((_t.x - org.x),(_t.y - org.y),(_t.z - org.z));
        }
        _r = rl.Get(idx);
        if (_r != null) {
            AddRotate(_r.angle,_r.x,_r.y,_r.z,org);
        }
        _s = sl.Get(idx);
        if (_s != null) {
            AddScale(_s.x, _s.y, _s.z);
        }
    }
    public void NextFrame() {
        SetFrame(frameIndex+1);
    }
    public int FrameCount() {
        return maxframeCount;
    }
    public void AddVertex(float[] xyz) {
        vpl.Add(xyz);
    }
    public void AddVertex(float[] xyz, float[] uv) {
        vpl.Add(xyz);
        maps.Get(0).Add(uv);
    }
    public void AddVertex(float[] xyz, float[] uv1, float[] uv2) {
        vpl.Add(xyz);
        maps.Get(0).Add(uv1);
        maps.Get(1).Add(uv2);
    }
    public void AddVertex(Vertex v) {
        vpl.Add(v.x);
        vpl.Add(v.y);
        vpl.Add(v.z);
        OpenGLUVMap map = maps.Get(0);
        map.Add(v.u);
        map.Add(v.v);
    }
    public int GetVertexCount() {
        return vpl.Size() / 3;
    }
    public float[] GetVertexBuffer() {
        return vpl.ToArray();
    }
    public void AddUV(float[] uv) {
        maps.Get(0).Add(uv);
    }
    public void AddUV(float[] uv, int map) {
        maps.Get(map).Add(uv);
    }
    public void AddPoly(int[] pts) {
        vil.Add(pts);
    }
    public int GetPolyCount() {
        return vil.Size();
    }
    public int[] GetPolyBuffer() {
        return vil.ToArray();
    }
    public void CopyBuffers() {
        int[] ids = new int[1];

        if (vpb == -1) {
            glGenBuffers(1, ids);
            vpb = ids[0];
        }
        glBindBuffer(GL_ARRAY_BUFFER, vpb);
        glBufferData(GL_ARRAY_BUFFER, vpl.Size() * 4, vpl.ToArray(), GL_STATIC_DRAW);

        for(int a=0;a<maps.Size();a++) {
            maps.Get(a).CopyBuffers();
        }

        if (vib == -1) {
            glGenBuffers(1, ids);
            vib = ids[0];
        }
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, vib);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, vil.Size() * 4, vil.ToArray(), GL_STREAM_DRAW);
    }
    public void BindBuffers(OpenGLScene scene) {
        glBindBuffer(GL_ARRAY_BUFFER, vpb);
        glVertexAttribPointer(scene.vpa, 3, GL_FLOAT, GL_FALSE, 0);

        for(int m=0;m<maps.Size();m++) {
            maps.Get(m).BindBuffers(scene);
        }

        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, vib);
    }
    public void Render(OpenGLScene scene) {
        if (vpl.Size() == 0 || vil.Size() == 0) return;    //crashes if empty ???
        int uvcnt = maps.Size();
        glUniform1i(scene.uUVMaps, uvcnt);
        glEnableVertexAttribArray(scene.tca[0]);
        if (uvcnt > 1) {
            glEnableVertexAttribArray(scene.tca[1]);
        } else {
            glDisableVertexAttribArray(scene.tca[1]);
        }
        glDrawElements(type, vil.Size(), GL_UNSIGNED_INT);
    }
    public OpenGLUVMap CreateUVMap() {
        OpenGLUVMap map = new OpenGLUVMap(maps.Size());
        maps.Add(map);
        return map;
    }
    public OpenGLUVMap GetUVMap(int idx) {
        return maps.Get(idx);
    }
    public OpenGLUVMap GetUVMap(String name) {
        for(int a=0;a<maps.Size();a++) {
            OpenGLUVMap map = maps.Get(a);
            if (map.GetName().Equals(name)) return map;
        }
        return null;
    }
    public int GetUVMapCount() {
        return maps.Size();
    }
/*
    public void print(GLModel model) {
        Console.WriteLine("Object:" + name);
        //print vertex data
        float vp[] = vpl.ToArray();
        for(int a=0;a<vp.length;) {
            Console.WriteLine(String.format("v[%d]=%6.3f,%6.3f,%6.3f", a/3, vp[a++], vp[a++], vp[a++]));
        }
        //print poly data
        int vi[] = vil.ToArray();
        for(int a=0;a<vi.length;) {
            switch (type) {
                case GL_TRIANGLES:
                    Console.WriteLine(String.format("i[%d]=%d,%d,%d", a/3, vi[a++], vi[a++], vi[a++]));
                    break;
                case GL_QUADS:
                    Console.WriteLine(String.format("i[%d]=%d,%d,%d,%d", a/4, vi[a++], vi[a++], vi[a++], vi[a++]));
                    break;
            }
        }
        //print uv maps
        for(int m=0;m<maps.Size();m++) {
            OpenGLUVMap map = maps.Get(m);
            Console.WriteLine("UVMap:" + map.name + ",texture=" + model.textures.Get(map.textureIndex));
            float[] uv = map.uvl.ToArray();
            for(int a=0;a<uv.length;) {
                Console.WriteLine(String.format("uv[%d]=%6.3f,%6.3f", a/2, uv[a++], uv[a++]));
            }
        }
    }
*/
}

}
