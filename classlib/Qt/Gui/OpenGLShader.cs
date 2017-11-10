using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QOpenGLShader")]
    [CPPClass("public: OpenGLShader(int type) : QOpenGLShader((QOpenGLShader::ShaderTypeBit)type) {}")]
    [CPPConstructorArgs("type")]
    public class OpenGLShader {
        public OpenGLShader(int type) {}
        public static int Vertex = 1;
        public static int Fragment = 2;
    }
}
