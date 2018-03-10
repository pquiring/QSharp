using Qt.Core;

namespace Qt.Gui {

public class OpenGLTexture : OpenGLFunctions {
    public Image bitmap;
    public int refcnt;
    public int glid;
    public bool loaded;
    public int idx;    //GL texture unit
    public String name;

    private static bool mipmaps = false;

    public OpenGLTexture(int idx) {
        InitializeOpenGLFunctions();
        refcnt = 0;
        glid = -1;
        bitmap = new Image(0,0);
        loaded = false;
        this.idx = idx;
    }

    public void Set(int[] pixels, int x, int y) {
        bitmap = new Image(x,y);
        bitmap.SetPixels(pixels);
    }

    public bool Load(String filename) {
        Console.WriteLine("Loading Texture:" + filename);
        try {
            return Load(new File(filename));
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return false;
        }
    }

    public bool Load(IOStream fis) {
        if (!bitmap.Load(fis)) {
            return false;
        }
        return true;
    }

    public bool Load() {
        if (glid == -1) {
            int[] id = new int[1];
            id[0] = -1;
            glGenTextures(1, id);
            if (id[0] == -1) {
                Console.WriteLine("glGenTextures failed:Error=0x" + Int32.ToString(glGetError(), 16));
                return false;
            }
            glid = id[0];
        }
        if (loaded) {
            return true;
        }
        glActiveTexture(GL_TEXTURE0 + idx);
        glBindTexture(GL_TEXTURE_2D, glid);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
        if (mipmaps) {
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST_MIPMAP_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_NEAREST);
        } else {
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
        }
        glTexImage2D(GL_TEXTURE_2D, 0, 4, bitmap.GetWidth(), bitmap.GetHeight(), 0, GL_BGRA
            , GL_UNSIGNED_BYTE, bitmap.GetPixels());
        loaded = true;
        return true;
    }

    public void Bind() {
        glActiveTexture(GL_TEXTURE0 + idx);
        glBindTexture(GL_TEXTURE_2D, glid);
    }
}

}
