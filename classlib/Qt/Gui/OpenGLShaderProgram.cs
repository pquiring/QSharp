using Qt.Core;
using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass("private: QOpenGLShaderProgram *$q = nullptr;\r\n")]
    public class OpenGLShaderProgram {
        public OpenGLShaderProgram(OpenGLWindow parent) {
            CPP.Add("$q = new QOpenGLShaderProgram();");
        }

        public bool AddShaderFromSourceCode(int type, string src) {
            CPP.Add("bool ret = $q->addShaderFromSourceCode((QOpenGLShader::ShaderType)type, *src.get()->$q);\r\n");
            return CPP.ReturnBool("ret");
        }
        public int AttributeLocation(string name) {
            return CPP.ReturnInt("$q->attributeLocation(name->cstring())");
        }
        public int UniformLocation(string name) {
            return CPP.ReturnInt("$q->uniformLocation(name->cstring())");
        }
        public void SetUniformValue(int location, int value) {
            CPP.Add("$q->setUniformValue(location, value);");
        }
        public void SetUniformValue(int location, float value) {
            CPP.Add("$q->setUniformValue(location, value);");
        }
        public void SetUniformValue(int location, Matrix4x4 value) {
            CPP.Add("$q->setUniformValue(location, (QMatrix4x4)*value.get());");
        }
        public void Link() {
            CPP.Add("$q->link();");
        }
        public void Bind() {
            CPP.Add("$q->bind();");
        }
        public void Release() {
            CPP.Add("$q->release();");
        }
    }
}
