using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "std::qt_ptr<QOpenGLShader> $q;"
    )]
    public class OpenGLShader {
        public OpenGLShader(int type) {
            CPP.Add("$q = new QOpenGLShader((QOpenGLShader::ShaderTypeBit)type);");
        }
        public static int Vertex = 1;
        public static int Fragment = 2;
    }
}
