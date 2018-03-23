using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass("std::unique_ptr<QOpenGLShaderProgram> $q;")]
    public class OpenGLShaderProgram {
        public OpenGLShaderProgram() {
            CPP.Add("$q = std::make_unique<QOpenGLShaderProgram>();");
        }

        public bool AddShaderFromSourceCode(int type, string src) {
            CPP.Add("bool ret = $q->addShaderFromSourceCode((QOpenGLShader::ShaderType)type, $check(src)->qstring());");
            return CPP.ReturnBool("ret");
        }

        public int GetAttributeLocation(string name) {
            return CPP.ReturnInt("$q->attributeLocation($check(name)->cstring().constData())");
        }
        public int GetUniformLocation(string name) {
            return CPP.ReturnInt("$q->uniformLocation($check(name)->cstring().constData())");
        }
        public void SetUniformValue(int location, int value) {
            CPP.Add("$q->setUniformValue(location, value);");
        }
        public void SetUniformValue(int location, int[] value) {
            CPP.Add("$q->setUniformValueArray(location, (const GLint*)value->data(), value->Size());");
        }
        public void SetUniformValue(int location, float value) {
            CPP.Add("$q->setUniformValue(location, value);");
        }
        public void SetUniformValue(int location, float[] value) {
            CPP.Add("$q->setUniformValueArray(location, (const GLfloat*)value->data(), value->Size(), 1);");
        }
        public void SetUniformValue(int location, Matrix4x4 value) {
            CPP.Add("$q->setUniformValue(location, (QMatrix4x4)*($check(value)->$value()));");
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
