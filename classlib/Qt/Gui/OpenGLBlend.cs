using Qt.Core;

namespace Qt.Gui {

/**
 * Blender .blend reader
 *
 * NOTE:
 *     Supports Blender v2.63+
 *     Supports objects with multiple UVMaps
 *     Rotation/Scale on objects are ignored, please rotate/scale in edit mode (the vertex data)
 *     BHead chunks can have duplicate old pointer addresses in which case they must be used in order.
 *         See : https://developer.blender.org/T45471
 * TODO:
 *     Animation data
 *
 * Blender Source : https://git.blender.org/gitweb/gitweb.cgi/blender.git/tree/HEAD:/source/blender
 *     - look in blenloader and makesdna folders
 *     - most important to understand DNA : makesdna/intern/dna_genfile.c:init_structDNA()
 *     - also see doc/blender_file_format/mystery_of_the_blend.html
 *
 * @author pquiring
 */

public class OpenGL_BLEND : OpenGLConstants {
    private byte[] data;
    private int datapos;

    private bool x64;    //64bit file format (else 32bit)
    private bool le;    //little endian file format (else big endian)

    private OpenGLModel model;
    private OpenGLObject obj;

    private float[] org = new float[3];
    private bool haveDups;

    private Map<long, BlendChunk> chunks = new Map<long, BlendChunk>();

    private const int ID_ME = 0x454d;    //ME (mesh)
    private const int ID_OB = 0x424f;    //OB (bobject)
    private const int ID_SC = 0x4353;    //SCE (OpenGLScene)
    private const int ID_DNA1 = 0x31414e44;    //DNA1

// typedef enum CustomDataType {...}
    private const int CD_MVERT                        = 0;
    private const int CD_MSTICKY                    = 1;    /* DEPRECATED */
    private const int CD_MDEFORMVERT            = 2;
    private const int CD_MEDGE                        = 3;
    private const int CD_MFACE                        = 4;
    private const int CD_MTFACE                     = 5;
    private const int CD_MCOL                         = 6;
    private const int CD_ORIGINDEX                = 7;
    private const int CD_NORMAL                     = 8;
/*    private const int CD_POLYINDEX                = 9; */
    private const int CD_PROP_FLT                 = 10;
    private const int CD_PROP_INT                 = 11;
    private const int CD_PROP_STR                 = 12;
    private const int CD_ORIGSPACE                = 13;    /* for modifier stack face location mapping */
    private const int CD_ORCO                         = 14;
    private const int CD_MTEXPOLY                 = 15;
    private const int CD_MLOOPUV                    = 16;
    private const int CD_MLOOPCOL                 = 17;
    private const int CD_TANGENT                    = 18;
    private const int CD_MDISPS                     = 19;
    private const int CD_PREVIEW_MCOL         = 20;    /* for displaying weightpaint colors */
    private const int CD_ID_MCOL                    = 21;
    private const int CD_TEXTURE_MCOL         = 22;
    private const int CD_CLOTH_ORCO             = 23;
    private const int CD_RECAST                     = 24;

/* BMESH ONLY START */
    private const int CD_MPOLY                        = 25;
    private const int CD_MLOOP                        = 26;
    private const int CD_SHAPE_KEYINDEX     = 27;
    private const int CD_SHAPEKEY                 = 28;
    private const int CD_BWEIGHT                    = 29;
    private const int CD_CREASE                     = 30;
    private const int CD_ORIGSPACE_MLOOP    = 31;
    private const int CD_PREVIEW_MLOOPCOL = 32;
    private const int CD_BM_ELEM_PYPTR        = 33;
/* BMESH ONLY END */

    private const int CD_PAINT_MASK             = 34;
    private const int CD_GRID_PAINT_MASK    = 35;
    private const int CD_MVERT_SKIN             = 36;
    private const int CD_FREESTYLE_EDGE     = 37;
    private const int CD_FREESTYLE_FACE     = 38;
    private const int CD_MLOOPTANGENT         = 39;
    private const int CD_TESSLOOPNORMAL     = 40;
    private const int CD_CUSTOMLOOPNORMAL = 41;

    private const int CD_NUMTYPES                 = 42;

    //DNA stuff
    private ArrayList<String> names = new ArrayList<String>();    //Member names
    private ArrayList<String> types = new ArrayList<String>();    //Struct names
    private ArrayList<short> typelen = new ArrayList<short>();
    private ArrayList<BlendStruct> structs = new ArrayList<BlendStruct>();

    private class BlendMember {
        public short typelenidx;    //index into typelen
        public short nameidx;    //index into names
        public String name;
        public int typelen;
        public int size;    //total size of Member
    }
    private class BlendStruct {
        public short typeidx;    //index into types
        public short nr;    //# of members
        public String name;
        public ArrayList<BlendMember> members = new ArrayList<BlendMember>();
    }
    private BlendStruct getStruct(String name) {
        for(int a=0;a<structs.Size();a++) {
            BlendStruct s = structs.Get(a);
            if (s.name.Equals(name)) return s;
        }
        throw new Exception("Struct not found:" + name);
    }
    private int calcMemberSize(BlendMember m) {
        if (m.name.StartsWith("*")) {
            if (x64) return 8;
            return 4;
        }
        if (m.name.IndexOf('[') != -1) {
            //array type
            String rep = new String(m.name);
            rep.Replace("]", "");
            String[] f = rep.Split("[");
            if (f.Length == 2) {
                //single array
                return m.typelen * Int32.ValueOf(f[1]);
            } else {
                //double array
                return m.typelen * Int32.ValueOf(f[1]) * Int32.ValueOf(f[2]);
            }
        }
        return m.typelen;
    }

    public OpenGLModel Load(String filename) {
        try {
            return loadBlend(new File(filename));
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return null;
        }
    }
    public OpenGLModel Load(IOStream fis) {
        try {
            return loadBlend(fis);
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return null;
        }
    }
    private bool eof() {
        return datapos >= data.Length;
    }
    private byte readuint8() {
        byte _uint8 = data[datapos++];
        return _uint8;
    }
    private short readuint16() {
        int _uint16;
        if (le) _uint16 = LE.getuint16(data, datapos);
        else _uint16 = BE.getuint16(data, datapos);
        datapos += 2;
        return (short)_uint16;
    }
    private int readuint32() {
        int _uint32;
        if (le) _uint32 = LE.getuint32(data, datapos);
        else _uint32 = BE.getuint32(data, datapos);
        datapos += 4;
        return _uint32;
    }
    private long readuint64() {
        long _uint64;
        if (le) _uint64 = LE.getuint64(data, datapos);
        else _uint64 = BE.getuint64(data, datapos);
        datapos += 8;
        return _uint64;
    }
    private long readptr() {
        if (x64) return readuint64();
        return readuint32();
    }
    private void readByteArray(byte[] _in) {
        Arrays.Copy(data, datapos, _in, 0, _in.Length);
        datapos+=_in.Length;
    }
    private void readPtrArray(long[] _in) {
        for(int a=0;a<_in.Length;a++) {
            _in[a] = readptr();
        }
    }
    private void readFloatArray(float[] _in) {
        for(int a=0;a<_in.Length;a++) {
            _in[a] = readfloat();
        }
    }
    private float readfloat() {
        return Float.IntBitsToFloat(readuint32());
    }
    /* Read fixed size char[] string */
    private String readString(int len) {
        int sl = strlen(data, datapos, len);
        String str = new String(data, datapos, sl);
        datapos+=len;
        return str;
    }
    /* Read C style string (NULL terminated) */
    private String readString() {
        int sl = strlen(data, datapos, data.Length - datapos);
        String str = new String(data, datapos, sl);
        datapos += sl+1;
        return str;
    }
    private void setData(byte[] _in) {
        data = _in;
        datapos = 0;
    }
    private class BlendContext {
        public byte[] data;
        public int datapos;
    }
    private BlendContext pushData() {
        BlendContext ctx = new BlendContext();
        ctx.data = data;
        ctx.datapos = datapos;
        return ctx;
    }
    private void popData(BlendContext ctx) {
        data = ctx.data;
        datapos = ctx.datapos;
    }
    private int strlen(byte[] str, int offset, int max) {
        for(int a=0;a<max;a++) {
            if (str[a+offset] == 0) return a;
        }
        return max;
    }
    private BlendChunk findChunkByPtr(long ptr) {
        if (ptr == 0) return null;
        BlendChunk chunk = chunks.Get(ptr);
        if (chunk == null) return null;
        if (chunk.dup) {
//            Console.WriteLine("Duplicate:" + Long.toString(ptr, 16) + ",idx=" + chunk.dupidx);
            int cnt = chunk.dupidx;
            chunk.dupidx++;
            for(int a=0;a<cnt;a++) {
                chunk = chunk.nextdup;
            }
        }
        return chunk;
    }
    private class BlendVertex {
        public float[] xyz;
    }
    private OpenGLModel loadBlend(IOStream fis) {
        setData(fis.ReadAll().ToArray());

        if (data.Length < 12) {
            throw new Exception("GL_BLEND:File too small");
        }

        model = new OpenGLModel();

        //load signature (12 bytes) "BLENDER_V100"
        if (!new String(data, 0, 7).Equals("BLENDER")) {
            throw new Exception("Not a blender file");
        }
        switch ((char)data[7]) {
            case '-': x64 = true; break;
            case '_': x64 = false; break;
            default:
                throw new Exception("GL_BLEND:Unknown bit size");
        }

        switch ((char)data[8]) {
            case 'v': le = true; break;
            case 'V': le = false; break;
            default:
                throw new Exception("GL_BLEND:Unknown Endianness");
        }

        String version = new String(data, 9, 3);
//        Console.WriteLine("Blender file version:" + version);
        int ver = Int32.ValueOf(version);
        if (ver < 263) {
            throw new Exception("Error:Blender file too old, can not read.");
        }

        datapos = 12;    //skip main header

        //first phase - read raw chunks
        while (!eof()) {
            BlendChunk chunk = new BlendChunk(this);
            chunk.filepos = datapos;
            chunk.read();
            BlendChunk ochunk = chunks.Get(chunk.ptr);
            if (ochunk != null) {
                if (!haveDups) {
                    Console.WriteLine("Warning:This file contains duplicate BHeads.");
                    haveDups = true;
                }
                ochunk.dup = true;
                while (ochunk.nextdup != null) {
                    ochunk = ochunk.nextdup;
                }
                ochunk.nextdup = chunk;
            } else {
                chunks.Set(chunk.ptr, chunk);
            }
        }

        int chunkCnt = chunks.Size();
        BlendChunk[] chunkArray = chunks.Values();
        BlendChunk raw;

        //2nd phase - parse DNA chunk
        for(int i=0;i<chunkCnt;i++) {
            if (chunkArray[i].id == ID_DNA1) {
                raw = chunkArray[i];
                setData(raw.raw);
                //SDNA
                String SDNA = readString(4);
                if (!SDNA.Equals("SDNA")) throw new Exception("Bad DNA Struct:SDNA");
                //NAME
                String NAME = readString(4);
                if (!NAME.Equals("NAME")) throw new Exception("Bad DNA Struct:NAME");
                int nr_names = readuint32();
                for(int a=0;a<nr_names;a++) {
                    String str = readString();
//                    Console.WriteLine("name=" + str);
                    names.Add(str);
                }
                //align pointer
                datapos += 3;
                datapos &= 0x7ffffffc;
                //TYPE
                String TYPE = readString(4);
                if (!TYPE.Equals("TYPE")) throw new Exception("Bad DNA Struct:TYPE");
                int nr_types = readuint32();
                for(int a=0;a<nr_types;a++) {
                    String str = readString();
//                    Console.WriteLine("type=" + str);
                    types.Add(str);
                }
                //align pointer
                datapos += 3;
                datapos &= 0x7ffffffc;
                //TLEN
                String TLEN = readString(4);
                if (!TLEN.Equals("TLEN")) throw new Exception("Bad DNA Struct:TLEN");
                for(int a=0;a<nr_types;a++) {
                    typelen.Add(readuint16());
                }
                //align pointer
                datapos += 3;
                datapos &= 0x7ffffffc;
                //STRC
                String STRC = readString(4);
                if (!STRC.Equals("STRC")) throw new Exception("Bad DNA Struct:STRC");
                int nr_structs = readuint32();
                for(int a=0;a<nr_structs;a++) {
                    BlendStruct s = new BlendStruct();
                    s.typeidx = readuint16();
                    s.nr = readuint16();
                    s.name = types.Get(s.typeidx);
//                    Console.WriteLine("Struct:" + s.name + "==" + a);
                    for(int b=0;b<s.nr;b++) {
                        BlendMember m = new BlendMember();
                        m.typelenidx = readuint16();
                        m.nameidx = readuint16();
                        m.name = names.Get(m.nameidx);
                        m.typelen = typelen.Get(m.typelenidx);
                        m.size = calcMemberSize(m);
//                        Console.WriteLine("    Member:" + m.name + "=" + m.Length);
                        s.members.Add(m);
                    }
                    structs.Add(s);
                }
                break;
            }
        }

        //3nd phase - now look for objects and piece together chunks
        for(int i=0;i<chunkCnt;i++) {
            if (chunkArray[i].id == ID_SC) {
                setData(chunkArray[i].raw);
                BlendScene OpenGLScene = new BlendScene(this);
                OpenGLScene.read();
                long ptr = OpenGLScene.last;
                while (ptr != 0) {
                    BlendChunk chunk = findChunkByPtr(ptr);
                    if (chunk == null) break;
                    setData(chunk.raw);
                    BlendBase _base = new BlendBase(this);
                    _base.read();
                    chunk = findChunkByPtr(_base.bobject);
                    if (chunk.id == ID_OB) {
                        readObject(chunk);
                    }
                    ptr = _base.prev;
                }
            }
        }

        return model;
    }

    private void readObject(BlendChunk chunk) {
        ArrayList<BlendVertex> vertexList = new ArrayList<BlendVertex>();
        ArrayList<int> loopList = new ArrayList<int>();
        setData(chunk.raw);
        BlendObject bObj = new BlendObject(this);
        bObj.read();
//                Console.WriteLine("bobject.type=" + bObj.type);
        if (bObj.type != 1) return;    //not a mesh bobject (could be camera, light, etc.)
        obj = new OpenGLObject();
        model.AddObject(obj);
        obj.SetName(bObj.id.name.Substring(2));
//        Console.WriteLine("bobject=" + obj.name);
        chunk = findChunkByPtr(bObj.data);
        if (chunk == null) {
            throw new Exception("GL_BLEND:Unable to find Mesh for Object");
        }
        BlendMesh mesh = new BlendMesh(this);
        setData(chunk.raw);
//                Console.WriteLine("Mesh@" + Int32.toString(raw.fileOffset, 16));
        mesh.read();
        obj.GetOrigin().x = bObj.loc[0];
        org[0] = bObj.loc[0];
        obj.GetOrigin().y = bObj.loc[1];
        org[1] = bObj.loc[1];
        obj.GetOrigin().z = bObj.loc[2];
        org[2] = bObj.loc[2];
        //find mvert
        chunk = findChunkByPtr(mesh.mvert);
        if (chunk == null) {
            throw new Exception("GL_BLEND:Unable to find MVert for Mesh");
        }
        setData(chunk.raw);
        for(int a=0;a<chunk.nr;a++) {
            BlendMVert mvert = new BlendMVert(this);
            mvert.read();
//                    obj.AddVertex(mvert.co);
            BlendVertex v = new BlendVertex();
            v.xyz = mvert.v;
            vertexList.Add(v);
        }
        //find mloop
        chunk = findChunkByPtr(mesh.mloop);
        if (chunk == null) {
            throw new Exception("GL_BLEND:Unable to find MLoop for Mesh");
        }
        setData(chunk.raw);
        for(int a=0;a<chunk.nr;a++) {
            BlendMLoop mloop = new BlendMLoop(this);
            mloop.read();
            loopList.Add(mloop.v);
        }
        //find mloopuv
/*    //use the UVMaps _in the CustomData instead - this fis only the active one
        raw = findChunkByPtr(mesh.mloopuv);
        if (raw == null) {
            throw new Exception("GL_BLEND:Unable to find MLoopUV for Mesh");
        }
        setData(raw.raw);
        Console.WriteLine("MLoopUV:nr=" + raw.nr);
        for(int a=0;a<raw.nr;a++) {
            MLoopUV mloopuv = new MLoopUV();
            mloopuv.read();
        }
*/
        //find mpoly
        chunk = findChunkByPtr(mesh.mpoly);
        if (chunk == null) {
            throw new Exception("GL_BLEND:Unable to find MPoly for Mesh");
        }
        setData(chunk.raw);
//TODO : calc which vertex needed to be dup'ed for each unique uv value (Blender does this _in their 3ds export script)
        int type = -1;
        int pcnt = -1;
        int vidx = 0;
        //MPoly = faces
        for(int a=0;a<chunk.nr;a++) {
            BlendMPoly mpoly = new BlendMPoly(this);
            mpoly.read();
            switch (mpoly.totloop) {
                case 3:
                    if (type == GL_QUADS) {
                        throw new Exception("GL_BLEND:Mixed QUADS/TRIANGLES not supported");
                    }
                    type = GL_TRIANGLES;
                    pcnt = 3;
                    break;
                case 4:
                    if (type == GL_TRIANGLES) {
                        throw new Exception("GL_BLEND:Mixed QUADS/TRIANGLES not supported");
                    }
                    type = GL_QUADS;
                    pcnt = 4;
                    break;
                default:
                    throw new Exception("GL_BLEND:Polygon not supported:nr=" + mpoly.totloop);
            }
            int loopidx = mpoly.loopstart;
            for(int p=0;p<pcnt;p++) {
                int idx = loopList.Get(loopidx++);
                obj.AddVertex(vertexList.Get(idx).xyz);
                obj.AddPoly(new int[] {vidx++});
            }
        }
        obj.SetType(type);
        //find customdata types
        readLayer(mesh.vdata.layers, "vdata");
        readLayer(mesh.edata.layers, "edata");
        readLayer(mesh.fdata.layers, "fdata");
        readLayer(mesh.pdata.layers, "pdata");
        readLayer(mesh.ldata.layers, "ldata");
    }
    private void readLayer(long layers, String name) {
        if (layers == 0) return;
//        Console.WriteLine(name + ".layers=" + Long.toString(layers, 16));
        BlendChunk raw = findChunkByPtr(layers);
        if (raw == null) {
            throw new Exception("GL_BLEND:Unable to find " + name + ".layers for Mesh");
        }
        setData(raw.raw);
//        Console.WriteLine("#layers=" + raw.nr);
        for(int a=0;a<raw.nr;a++) {
            BlendCustomDataLayer layer = new BlendCustomDataLayer(this);
            layer.read();
            String layer_name = layer.name;
            if (layer.data == 0) {
//                Console.WriteLine("layer.data == null");
                continue;
            }
            BlendChunk layer_data = findChunkByPtr(layer.data);
            if (layer_data == null) {
                throw new Exception("GL_BLEND:Unable to find " + name + ".layers.data for Mesh");
            }
            BlendContext ctx = pushData();
            setData(layer_data.raw);
//            Console.WriteLine("layer.data=" + Long.toString(layer.data, 16) + ",type==" + layer.type + ",a=" + a);
            switch (layer.type) {
                case CD_MTEXPOLY: {    //15
                    //NOTE:There fis a MTexPoly per face, I only read the first
                    BlendMTexPoly tex = new BlendMTexPoly(this);
                    tex.read();
                    BlendChunk imageChunk = findChunkByPtr(tex.tpage);
                    if (imageChunk == null) {
                        throw new Exception("GL_BLEND:No texture found for UVMap:" + a);
                    }
                    setData(imageChunk.raw);
                    BlendImage image = new BlendImage(this);
                    image.read();
                    OpenGLUVMap map;
                    if (a < obj.GetUVMapCount())
                        map = obj.GetUVMap(a);
                    else
                        map = obj.CreateUVMap();
                    String tn = image.name;
                    //string texture path for now
                    int tnidx = tn.LastIndexOf('/');
                    if (tnidx != -1) {
                        tn = tn.Substring(tnidx+1);
                    }
                    tnidx = tn.LastIndexOf('\\');
                    if (tnidx != -1) {
                        tn = tn.Substring(tnidx+1);
                    }
                    int tidx = model.AddTexture(tn);
                    map.SetTextureIndex(tidx);
                    map.SetName(layer_name);
//                    Console.WriteLine("texpoly=" + map.name);
                    break;
                }
                case CD_MLOOPUV: { //16
                    //There fis a UV per face per vertex
                    if (a >= obj.GetUVMapCount()) {
                        obj.CreateUVMap();
                    }
//                    Console.WriteLine("loopuv.nr=" + layer_data.nr);
                    for(int b=0;b<layer_data.nr;b++) {
                        BlendMLoopUV uv = new BlendMLoopUV(this);
                        uv.read();
                        uv.uv[1] = 1.0f - uv.uv[1];    //invert V(y)
                        obj.AddUV(uv.uv, a);
                    }
                    break;
                }
            }
            popData(ctx);
        }
    }
    private class BlendChunk {
        //BHead
        public int id;
        public int len;
        public long ptr;    //the actual memory address of this chunk when it was saved to disk !!!
        public int SDNAnr;
        public int nr;    //array count of Struct

        public byte[] raw;

        public int filepos;    //for debugging

        public bool dup;
        public int dupidx;
        public BlendChunk nextdup;

        public int fileOffset;

        public OpenGL_BLEND blend;
        public BlendChunk(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            id = blend.readuint32();
            len = blend.readuint32();
            ptr = blend.readptr();
            SDNAnr = blend.readuint32();
            nr = blend.readuint32();
            fileOffset = blend.datapos;
            if (len == 0) return;
            raw = new byte[len];
            blend.readByteArray(raw);
        }
    }
    private class BlendID {
        public String name;
        public OpenGL_BLEND blend;
        public BlendID(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("ID");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("name[66]")) {
                    name = blend.readString(m.size);
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendScene {
        public long first;    //first Base
        public long last;
        public OpenGL_BLEND blend;
        public BlendScene(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("OpenGLScene");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("_base")) {
                    first = blend.readptr();
                    last = blend.readptr();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendBase {
        public long next;
        public long prev;
        public long bobject;
        public OpenGL_BLEND blend;
        public BlendBase(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("Base");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("*next")) {
                    next = blend.readptr();
                }
                else if (m.name.Equals("*prev")) {
                    prev = blend.readptr();
                }
                else if (m.name.Equals("*object")) {
                    bobject = blend.readptr();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendObject {
        public BlendID id;
        public int type;
        public long data;    // -> Mesh
        public float[] loc = new float[3];    //Location (aka Origin)
        public OpenGL_BLEND blend;
        public BlendObject(OpenGL_BLEND blend) {
            this.blend = blend;
            id = new BlendID(blend);
        }
        public void read() {
            BlendStruct s = blend.getStruct("Object");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("*data")) {
                    data = blend.readptr();
                }
                else if (m.name.Equals("loc[3]")) {
                    blend.readFloatArray(loc);
                }
                else if (m.name.Equals("type")) {
                    type = blend.readuint16();
                }
                else if (m.name.Equals("id")) {
                    id.read();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendCustomData {
        public long layers;    //->CustomDataLayer
        public OpenGL_BLEND blend;
        public BlendCustomData(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read(String name) {
            BlendStruct s = blend.getStruct("CustomData");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("*layers")) {
                    layers = blend.readptr();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendCustomDataLayer {
        public int type;                 /* type of data _in layer */
        public String name;            /* layer name, MAX_CUSTOMDATA_LAYER_NAME */
        public long data;                /* layer data */
        public OpenGL_BLEND blend;
        public BlendCustomDataLayer(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("CustomDataLayer");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("type")) {
                    type = blend.readuint32();
                }
                else if (m.name.Equals("name[64]")) {
                    name = blend.readString(m.size);
                }
                else if (m.name.Equals("*data")) {
                    data = blend.readptr();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendMesh {
        public BlendID id;
        public long mpoly, mloop, mloopuv, mvert;
        public BlendCustomData vdata;
        public BlendCustomData edata;
        public BlendCustomData fdata;
        public BlendCustomData pdata;
        public BlendCustomData ldata;
        public OpenGL_BLEND blend;
        public BlendMesh(OpenGL_BLEND blend) {
            this.blend = blend;
            id = new BlendID(blend);
            vdata = new BlendCustomData(blend);
            edata = new BlendCustomData(blend);
            fdata = new BlendCustomData(blend);
            pdata = new BlendCustomData(blend);
            ldata = new BlendCustomData(blend);
        }
        public void read() {
            BlendStruct s = blend.getStruct("Mesh");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("*mpoly")) {
                    mpoly = blend.readptr();
                }
                else if (m.name.Equals("*mloop")) {
                    mloop = blend.readptr();
                }
                else if (m.name.Equals("*mloopuv")) {
                    mloopuv = blend.readptr();
                }
                else if (m.name.Equals("*mvert")) {
                    mvert = blend.readptr();
                }
                else if (m.name.Equals("vdata")) {
                    vdata.read("vdata");
                }
                else if (m.name.Equals("edata")) {
                    edata.read("edata");
                }
                else if (m.name.Equals("fdata")) {
                    fdata.read("fdata");
                }
                else if (m.name.Equals("pdata")) {
                    pdata.read("pdata");
                }
                else if (m.name.Equals("ldata")) {
                    ldata.read("ldata");
                }
                else if (m.name.Equals("id")) {
                    id.read();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendMVert {
        public float[] v = new float[3];
        public OpenGL_BLEND blend;
        public BlendMVert(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("MVert");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("co[3]")) {
                    for(int b=0;b<3;b++) {
                        v[b] = blend.readfloat() + blend.org[b];    //xyz position
                    }
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendMPoly {
        /* offset into loop array and number of loops _in the face */
        public int loopstart;
        public int totloop;
        public OpenGL_BLEND blend;
        public BlendMPoly(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("MPoly");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("loopstart")) {
                    loopstart = blend.readuint32();
                }
                else if (m.name.Equals("totloop")) {
                    totloop = blend.readuint32();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendMLoop {
        public int v;    /* vertex index */
        public OpenGL_BLEND blend;
        public BlendMLoop(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("MLoop");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("v")) {
                    v = blend.readuint32();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendMTexPoly {
        public long tpage;    //Image
        public OpenGL_BLEND blend;
        public BlendMTexPoly(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("MTexPoly");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("*tpage")) {
                    tpage = blend.readptr();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendMLoopUV {
        public float[] uv = new float[2];
        public OpenGL_BLEND blend;
        public BlendMLoopUV(OpenGL_BLEND blend) {
            this.blend = blend;
        }
        public void read() {
            BlendStruct s = blend.getStruct("MLoopUV");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("uv[2]")) {
                    uv[0] = blend.readfloat();
                    uv[1] = blend.readfloat();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
    private class BlendImage {
        public BlendID id;
        public String name;
        public OpenGL_BLEND blend;
        public BlendImage(OpenGL_BLEND blend) {
            this.blend = blend;
            id = new BlendID(blend);
        }
        public void read() {
            BlendStruct s = blend.getStruct("Image");
            for(int a=0;a<s.nr;a++) {
                BlendMember m = s.members.Get(a);
                if (m.name.Equals("name[1024]")) {
                    name = blend.readString(m.size);
                }
                else if (m.name.Equals("id")) {
                    id.read();
                }
                else {
                    blend.datapos += m.size;
                }
            }
        }
    }
}

}
