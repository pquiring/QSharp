using Qt.Core;

namespace Qt.Gui {

/** OpenGLScene is a primative OpenGL scene framework (mostly to test functionality). */

public class OpenGLScene : OpenGLFunctions {
    private const bool DEBUG = false;

    private bool needinittex = true;

    private ArrayList<OpenGLModel> ml;
    private Map<String, OpenGLTexture> tl; //texture list
    private Map<String, OpenGLModel> mtl; //model templates list

    private OpenGLShaderProgram program;

    private ArrayList<int> freeglidlist;

    private OpenGLTexture blankTexture;

    public OpenGLScene() {
        InitializeOpenGLFunctions();
        freeglidlist = new ArrayList<int>();
        reset();
        texturePath = "";
        blankTexture = new OpenGLTexture(0);
        blankTexture.Set(new uint[] {0xffffffff},1,1);    //white pixel
    }

    public bool inited = false;

    public String texturePath;

    public int fragShader, vertexShader;
    public int vpa;    //attribs
    public int[] tca = new int[2];
    public int uUVMaps;
    public int mpu, mmu, mvu;    //uniform matrix'es (perspective, model, view)

//code
    public void init(String vertex, String fragment) {    //must give size of render window
        glFrontFace(GL_CCW);    //3DS uses GL_CCW
        glEnable(GL_CULL_FACE);    //don't draw back sides
        glEnable(GL_DEPTH_TEST);
        glDepthFunc(GL_LEQUAL);
        glEnable(GL_BLEND);
        glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
        glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
        glEnable(GL_TEXTURE_2D);
        glActiveTexture(GL_TEXTURE0);

        program = new OpenGLShaderProgram();
        program.AddShaderFromSourceCode(OpenGLShader.Vertex, vertex);
        program.AddShaderFromSourceCode(OpenGLShader.Fragment, fragment);
        program.Link();
        program.Bind();

        vpa = program.GetAttributeLocation("aVertexPosition");
        glEnableVertexAttribArray(vpa);

        tca[0] = program.GetAttributeLocation("aTextureCoord1");
        glEnableVertexAttribArray(tca[0]);
        int uSampler1 = program.GetUniformLocation("uSampler1");
        glUniform1i(uSampler1, 0);

        tca[1] = program.GetAttributeLocation("aTextureCoord2");
        glEnableVertexAttribArray(tca[1]);
        int uSampler2 = program.GetUniformLocation("uSampler2");
        glUniform1i(uSampler2, 1);

        mpu = program.GetUniformLocation("uPMatrix");
        mmu = program.GetUniformLocation("uMMatrix");
        mvu = program.GetUniformLocation("uVMatrix");
        uUVMaps = program.GetUniformLocation("uUVMaps");

//        Console.WriteLine("attribs=" + vpa + "," + tca[0] + "," + tca[1]);
//        Console.WriteLine("uniforms=" + mpu + "," + mmu + "," + mvu + "," + uUVMaps + "," + uSampler1 + "," /*+ uSampler2*/);

        InitTextures();

        inited = true;
    }
    public void reset() {
        if (tl != null) ReleaseTextures();
        ml = new ArrayList<OpenGLModel>();
        tl = new Map<String, OpenGLTexture>();
        mtl = new Map<String, OpenGLModel>();
    }
    private void ReleaseTextures() {
        OpenGLTexture[] ts = tl.Values();
        for(int i=0;i<ts.Length;i++) {
            if (ts[i].glid != -1) {
                ReleaseTexture(ts[i].glid);
                ts[i].glid = -1;
            }
        }
    }
    private void ReleaseTexture(int glid) {
        freeglidlist.Add(glid);
    }
//load textures from disk to general-purpose memory
    public bool LoadTextures() {
        //scan thru object list and load them all
        bool ret = true;
        OpenGLObject obj;
        OpenGLModel mod;
        int modCnt = ml.Size();
        for(int a=0;a<modCnt;a++) {
            mod = ml.Get(a);
            int objCnt = mod.ol.Size();
            for(int b=0;b<objCnt;b++) {
                obj = mod.ol.Get(b);
                int mapCnt = obj.GetUVMapCount();
                for(int m=0;m<mapCnt;m++) {
                    OpenGLUVMap map = obj.GetUVMap(m);
                    if (map.IsLoaded()) continue;
                    if (loadTexture(mod.GetTexture(map.GetTextureIndex()), map.GetMapIndex())) {
                        map.SetLoaded(true);
                    } else {
                        ret = false;
                    }
                }
            }
        }
        return ret;
    }
    private bool loadTexture(String fn, int idx) {
        if (fn == null) return false;
        OpenGLTexture tex;

        tex = tl.Get(fn);
        if (tex != null) {
            tex.refcnt++;
            return true;
        }
        needinittex = true;
        tex = new OpenGLTexture(idx);
        tex.name = fn;
        if (!tex.Load(fn)) {
            Console.WriteLine("Error:Failed to load texture:" + fn);
            return false;
        }
        tex.refcnt = 1;
        tl.Set(fn, tex);
        return true;
    }
    //directly load a texture
    public bool SetTexture(String fn, uint[] px, int w, int h, int idx) {
        OpenGLTexture tex = tl.Get(fn);
        if (tex == null) {
            tex = new OpenGLTexture(idx);
            tl.Set(fn, tex);
        } else {
            tex.loaded = false;
        }
        tex.Set(px, w, h);
        needinittex = true;
        return false;
    }
//load textures into video memory (texture objects)
    bool InitTextures() {
        if (!needinittex) {
            return true;
        }
        //setup blankTexture
        if (blankTexture.glid == -1) InitTexture(blankTexture);
        //first uninit any that have been deleted
        if (freeglidlist.Size() > 0) UninitTextures();
        //scan thru object list and load them all
        OpenGLTexture[] ts = tl.Values();
        for(int i=0;i<ts.Length;i++) {
            if (!InitTexture(ts[i])) return false;
        }
        needinittex = false;
        return true;
    }
    private bool InitTexture(OpenGLTexture tex) {
        return tex.Load();
    }
    private bool UninitTextures() {
        while (freeglidlist.Size() > 0) {
            uninitTexture(freeglidlist.Get(0));
            freeglidlist.Remove(0);
        }
        return true;
    }
    private bool uninitTexture(int glid) {
        int[] id = new int[1];
        id[0] = glid;
        glDeleteTextures(1, id);
        return true;
    }
    public void releaseUnusedTextures() {
        OpenGLTexture[] ts = tl.Values();
        for(int i=0;i<ts.Length;i++) {
            if (ts[i].refcnt == 0) ReleaseTexture(ts[i].glid);
        }
    }
    /** Release a cloned model @ index.
     */
    public void ReleaseModel(int idx) {
        OpenGLModel mod;
        OpenGLObject obj;
        mod = ml.Get(idx);
        int size = mod.ol.Size();
        for(int a=0;a<size;a++) {
            obj = mod.ol.Get(a);
            for(int m=0;m<obj.GetUVMapCount();m++) {
                OpenGLUVMap map = obj.GetUVMap(m);
                tl.Get(mod.GetTexture(map.GetTextureIndex())).refcnt--;
            }
        }
        ml.RemoveAt(idx);
    }
    public int modelCount() { return ml.Size(); }
    public bool addModel(OpenGLModel mod) { return addModel(mod, 0); }    //places Model at start of list
    public bool addModel(OpenGLModel mod, int idx) { if (mod == null) return false; ml.Insert(idx, mod); return true;}    //place Models with transparent textures last
    public int indexOfModel(OpenGLModel mod) { return ml.IndexOf(mod); }
    public void removeModel(int idx) { ml.RemoveAt(idx); }
    public void removeModel(OpenGLModel mod) { ml.Remove(mod); }
    public void nextFrame(int objidx) { ml.Get(objidx).NextFrame(); }
    public void setFrame(int objidx, int frame) { ml.Get(objidx).SetFrame(frame); }
    public void modelTranslate(int idx, float x, float y, float z) { ml.Get(idx).Translate(x,y,z); }
    public void modelRotate(int idx, float angle, float x, float y, float z) { ml.Get(idx).Rotate(angle,x,y,z); }
    public void modelScale(int idx, float x, float y, float z) { ml.Get(idx).Scale(x,y,z); }
    /** Loads a .3DS file into the template array.
     * Use addModel() to add a clone into the render scene.
     */
    public OpenGLModel Load3DS(String fn) {
        OpenGLModel mod;

        mod = mtl.Get(fn);
        if (mod != null) {
            mod.refcnt++;
            return mod;
        }

        OpenGL_3DS loader = new OpenGL_3DS();
        mod = loader.Load(fn);
        if (mod == null) return null;
        mtl.Set(fn, mod);
        mod.refcnt = 1;
        mod = (OpenGLModel)mod.Clone();

        return mod;
    }
    /** Loads a .blend file into the template array.
     * Use addModel() to add a clone into the render scene.
     */
    public OpenGLModel LoadBlend(String fn) {
        OpenGLModel mod;

        mod = mtl.Get(fn);
        if (mod != null) {
            mod.refcnt++;
            return mod;
        }

        OpenGL_BLEND loader = new OpenGL_BLEND();
        mod = loader.Load(fn);
        if (mod == null) return null;
        mtl.Set(fn, mod);
        mod.refcnt = 1;
        mod = (OpenGLModel)mod.Clone();

        return mod;
    }
    /** Loads a .JF3D file into the template array.
     * Use addModel() to add a clone into the render scene.
     */
    public OpenGLModel loadJF3D(String fn) {
        OpenGLModel mod;

        mod = mtl.Get(fn);
        if (mod != null) {
            mod.refcnt++;
            return mod;
        }

        OpenGL_JF3D loader = new OpenGL_JF3D();
        mod = loader.Load(fn);
        if (mod == null) return null;
        mtl.Set(fn, mod);
        mod.refcnt = 1;
        mod = (OpenGLModel)mod.Clone();

        return mod;
    }
    /** Clones a pre-loaded model.
     * Use addModel() to add into the render scene.
     */
    public OpenGLModel CloneModel(String fn) {
        OpenGLModel mod = mtl.Get(fn);
        if (mod == null) return null;
        mod.refcnt++;
        return (OpenGLModel)mod.Clone();
    }
    public void UnloadModel(OpenGLModel mod) {
        mod.refcnt--;
    }
    //this will release all unused models
    public void ReleaseModel() {
        OpenGLModel[] ms = ml.ToArray();
        int idx = 0;
        for(int i=0;i<ms.Length;i++) {
            if (ms[i].refcnt == 0) ReleaseModel(idx); else idx++;
        }
    }
}

}
