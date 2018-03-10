using Qt.Core;

namespace Qt.Gui {

/**
 * JF3D - New format designed for JavaForce
 *     Chunk based format
 *
 * Supports:
 *     - OpenGLModel, OpenGLObject, OpenGLUVMap(s)
 * TODO:
 *     - animation data
 *
 * struct ChunkHeader {
 *     int id;
 *     int len;    //size of data excluding ChunkHeader
 * }
 *
 * All data stored in Little Endian format (Intel based)
 *
 * @author pquiring
 */

public class OpenGL_JF3D : OpenGLConstants {
    private byte[] data;
    private int datapos;
    private int skip;

    private const bool debug = false;

    private const int MAGIC = 0x4433464a;    //'JF3D'
    private const int VERSION = 0x100;

    private const int ID_MODEL = 0x010000;
    private const int ID_OBJECT = 0x020000;
    private const int ID_UVMAP = 0x030000;
// future reserved
//    private const int ID_CAMERA = 0x40000;
//    private const int ID_LIGHT = 0x50000;

    private OpenGLModel model;
    private OpenGLObject obj;

    public OpenGLModel Load(String filename) {
        try {
            return loadJF3D(new File(filename));
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return null;
        }
    }
    public OpenGLModel Load(IOStream fis) {
        try {
            return loadJF3D(fis);
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return null;
        }
    }
    private bool eof() {
        return datapos >= data.Length;
    }
    private int readuint16() {
        int _uint16 = LE.getuint16(data, datapos);
        datapos += 2;
        skip -= 2;
        return _uint16;
    }
    private int readuint32() {
        int _uint32 = LE.getuint32(data, datapos);
        datapos += 4;
        skip -= 4;
        return _uint32;
    }
    private float readfloat() {
        return Float.IntBitsToFloat(readuint32());
    }
    private String readString() {
        String ret = "";
        char ch;
        while (!eof()) {
            ch = (char)data[datapos++];
            skip--;
            if (ch == 0) break;
            ret += ch;
        }
        return ret;
    }
    private OpenGLModel loadJF3D(IOStream fis) {
        datapos = 0;
        fis.Open(OpenMode.ReadOnly);
        data = fis.ReadAll().ToArray();

        int magic = readuint32();
        if (magic != MAGIC) {
            throw new Exception("GL_JF3D:Not JF3D file");
        }
        int version = readuint32();
        if (version < VERSION) {
            throw new Exception("GL_JF3D:Bad version");
        }

        int fcnt, pcnt, vcnt, uvcnt;
        OpenGLUVMap map;

        while (!eof()) {
            int head_id = readuint32();
            int head_len = readuint32();
            skip = head_len;
            int head_ver = head_id & 0xffff;
            head_id &= 0x7fff0000;
            switch (head_id) {
                case ID_MODEL:
                    if (model != null) {
                        throw new Exception("GL_JF3D:Multiple Model chunks found");
                    }
                    model = new OpenGLModel();
                    fcnt = readuint32();
                    for(int a=0;a<fcnt;a++) {
                        String txt = readString();
                        model.textures.Add(txt);
//                        Console.WriteLine("Texture=" + txt);
                    }
                    if (head_ver > 0) {
                        //future reserved
                    }
                    break;
                case ID_OBJECT:
                    obj = new OpenGLObject();
                    model.AddObject(obj);
                    obj.SetName(readString());
                    obj.SetType(readuint32());
                    obj.GetOrigin().x = readfloat();
                    obj.GetOrigin().y = readfloat();
                    obj.GetOrigin().z = readfloat();
                    vcnt = readuint32();    //vertex count
                    for(int v=0;v<vcnt;v++) {
                        float fx = readfloat();
                        float fy = readfloat();
                        float fz = readfloat();
                        obj.AddVertex(new float[] {fx, fy, fz});
                    }
                    pcnt = readuint32();    //poly count
                    switch (obj.GetType()) {
                        case GL_TRIANGLES:
                            pcnt *= 3;
                            break;
                        case GL_QUADS:
                            pcnt *= 4;
                            break;
                        default:
                            Console.WriteLine("GL_JF3D:Error Unknown GL Type:" + obj.GetType());
                            return null;
                    }
                    for(int p=0;p<pcnt;p++) {
                        int pt = readuint32();
                        if (pt >= vcnt) {
                            Console.WriteLine("Error:Poly includes invalid vertex !!!");
                        }
                        obj.AddPoly(new int[] {pt});
                    }
                    break;
                case ID_UVMAP:
                    map = obj.CreateUVMap();
                    map.SetName(readString());
                    map.SetTextureIndex(readuint32());
                    uvcnt = readuint32();
                    if (uvcnt != obj.GetVertexCount()) {
                        Console.WriteLine("Warning:UVMAP size != vertex count");
                    }
                    for(int i=0;i<uvcnt;i++) {
                        float u = readfloat();
                        float v = readfloat();
                        map.Add(new float[] {u, v});
                    }
                    break;
                default:
                    break;
            }
            if (skip > 0) {
                datapos += skip;
            }
        }
        return model;
    }

    public bool Save(OpenGLModel model, String filename) {
        try {
            File fos = new File(filename);
            saveJF3D(model, fos);
            fos.Close();
            return true;
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return false;
        }
    }
    public bool Save(OpenGLModel model, IOStream os) {
        try {
            saveJF3D(model, os);
            return true;
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return false;
        }
    }

    private ByteArrayStream baos;
    private byte[] tmp;

    private void writeString(String str) {
        baos.Write(str.ToByteArray());
        baos.Write(new byte[] {0});  //NULL
    }

    private void writeuint16(int val) {
        LE.setuint16(tmp, 0, val);
        baos.Write(tmp, 0, 2);
    }

    private void writeuint32(int val) {
        LE.setuint32(tmp, 0, val);
        baos.Write(tmp, 0, 4);
    }

    private void writefloat(float f) {
        writeuint32(Float.FloatToIntBits(f));
    }

    private void saveJF3D(OpenGLModel model, IOStream os) {
        baos = new ByteArrayStream();
        tmp = new byte[8];
        int size;

        writeuint32(MAGIC);
        writeuint32(VERSION);
        writeuint32(ID_MODEL);
        size = 0;
        int tcnt = model.textures.Size();
        for(int a=0;a<tcnt;a++) {
            size += model.textures.Get(a).Length + 1;
        }
        writeuint32(size);
        writeuint32(tcnt);
        for(int a=0;a<tcnt;a++) {
            writeString(model.textures.Get(a));
        }
        for(int o=0;o<model.ol.Size();o++) {
            OpenGLObject obj = model.ol.Get(o);
            writeuint32(ID_OBJECT);
            int vcnt = obj.GetVertexCount() * 3;
            int pcnt = obj.GetPolyCount();
            size = obj.GetName().Length + 1 + 4 + (4*3) + (4 + (vcnt * 4)) + (4 + (pcnt * 4));
            writeuint32(size);
            writeString(obj.GetName());
            writeuint32(obj.GetType());
            writefloat(obj.GetOrigin().x);
            writefloat(obj.GetOrigin().y);
            writefloat(obj.GetOrigin().z);
            writeuint32(vcnt / 3);
            float[] xyz = obj.GetVertexBuffer();
            for(int a=0;a<vcnt;a++) {
                writefloat(xyz[a]);
            }
            switch (obj.GetType()) {
                case GL_TRIANGLES:
                    writeuint32(pcnt / 3);
                    break;
                case GL_QUADS:
                    writeuint32(pcnt / 4);
                    break;
            }
            int[] pts = obj.GetPolyBuffer();
            for(int a=0;a<pcnt;a++) {
                writeuint32(pts[a]);
            }
            int maps = obj.GetUVMaps().Size();
            if (maps == 0) {
                Console.WriteLine("GL_JF3D:Warning:No UVMaps found for object:" + obj.GetName());
            }
            for(int m=0;m<maps;m++) {
                OpenGLUVMap map = obj.GetUVMaps().Get(m);
                writeuint32(ID_UVMAP);
                int uvcnt = map.GetUVCount() * 2;
                size = map.GetName().Length + 1 + 4 + (4 + (uvcnt * 4));
                writeuint32(size);
                writeString(map.GetName());
                writeuint32(map.GetTextureIndex());
                writeuint32(uvcnt/2);
                float[] uv = map.GetBuffer();
                for(int a=0;a<uvcnt;a++) {
                    writefloat(uv[a]);
                }
            }
        }
        os.Write(baos.GetData().ToArray());
    }
}

}
