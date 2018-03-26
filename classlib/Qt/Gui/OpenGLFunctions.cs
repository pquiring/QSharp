using Qt.Core;
using Qt.QSharp;

//TODO : $check() all arrays

namespace Qt.Gui {
    [CPPClass("static std::unique_ptr<QOpenGLFunctions> $q;")]
    public class OpenGLFunctions : OpenGLConstants {
        /** Initialize the GL functions. Only needs to be called once from main thread after a context is created. See OpenGLWindow.InitializeGL() */
        public static void InitializeOpenGLFunctions() {
            CPP.Add("$q = std::make_unique<QOpenGLFunctions>();");
            CPP.Add("if (QOpenGLContext::currentContext() == nullptr) {");
            CPP.Add("Qt::Core::Console::WriteLine(Qt::Core::String::$new(\"OpenGLFunctions:Error:No Context Created!\"));");
            CPP.Add("return;");
            CPP.Add("}");
            CPP.Add("$q->initializeOpenGLFunctions();");
        }

        public static void glActiveTexture(int texture) {CPP.Add("$q->glActiveTexture(texture);");}
        public static void glAttachShader(int program, int shader) {CPP.Add("$q->glAttachShader(program, shader);");}
        public static void glBindAttribLocation(int program, int index, string name) {CPP.Add("$q->glBindAttribLocation(program,index,$check(name)->cstring().constData());");}
        public static void glBindBuffer(int target, int buffer) {CPP.Add("$q->glBindBuffer(target, buffer);");}
        public static void glBindFramebuffer(int target, int framebuffer) {CPP.Add("$q->glBindFramebuffer(target, framebuffer);");}
        public static void glBindRenderbuffer(int target, int renderbuffer) {CPP.Add("$q->glBindRenderbuffer(target, renderbuffer);");}
        public static void glBindTexture(int target, int texture) {CPP.Add("$q->glBindTexture(target, texture);");}
        public static void glBlendColor(float red, float green, float blue, float alpha) {CPP.Add("$q->glBlendColor(red, green, blue, alpha);");}
        public static void glBlendEquation(int mode) {CPP.Add("$q->glBlendEquation(mode);");}
        public static void glBlendEquationSeparate(int modeRGB, int modeAlpha) {CPP.Add("$q->glBlendEquationSeparate(modeRGB, modeAlpha);");}
        public static void glBlendFunc(int sfactor, int dfactor) {CPP.Add("$q->glBlendFunc(sfactor, dfactor);");}
        public static void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha) {CPP.Add("$q->glBlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);");}
        public static void glBufferData(int target, int size, byte[] data, int usage) {CPP.Add("$q->glBufferData(target, size, $check(data, 0, size)->data(), usage);");}
        public static void glBufferData(int target, int size, int[] data, int usage) {CPP.Add("$q->glBufferData(target, size*4, $check(data, 0, size/4)->data(), usage);");}
        public static void glBufferData(int target, int size, float[] data, int usage) {CPP.Add("$q->glBufferData(target, size*4, $check(data, 0, size/4)->data(), usage);");}
        public static void glBufferSubData(int target, int offset, int size, byte[] data) {CPP.Add("$q->glBufferSubData(target, offset, size, $check(data, 0, size/4)->data());");}
        public static int glCheckFramebufferStatus(int target) {return CPP.ReturnInt("$q->glCheckFramebufferStatus(target)");}
        public static void glClear(int mask) {CPP.Add("$q->glClear(mask);");}
        public static void glClearColor(float red, float green, float blue, float alpha) {CPP.Add("$q->glClearColor(red, green, blue, alpha);");}
        public static void glClearDepthf(float depth) {CPP.Add("$q->glClearDepthf(depth);");}
        public static void glClearStencil(int s) {CPP.Add("$q->glClearStencil(s);");}
        public static void glColorMask(int red, int green, int blue, int alpha) {CPP.Add("$q->glColorMask(red, green, blue, alpha);");}
        public static void glCompileShader(int shader) {CPP.Add("$q->glCompileShader(shader);");}
        public static void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border, int imageSize, byte[] data) {CPP.Add("$q->glCompressedTexImage2D(target, level, internalformat, width, height, border, imageSize, data->data());");}
        public static void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int imageSize, byte[] data) {CPP.Add("$q->glCompressedTexSubImage2D(target, level, xoffset, yoffset, width, height, format, imageSize, data->data());");}
        public static void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border) {CPP.Add("$q->glCopyTexImage2D(target, level, internalformat, x, y, width, height, border);");}
        public static void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height) {CPP.Add("$q->glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);");}
        public static int glCreateProgram() {return CPP.ReturnInt("$q->glCreateProgram();");}
        public static int glCreateShader(int type) {return CPP.ReturnInt("$q->glCreateShader(type)");}
        public static void glCullFace(int mode) {CPP.Add("$q->glCullFace(mode);");}
        public static void glDeleteBuffers(int n, int[] buffers) {CPP.Add("$q->glDeleteBuffers(n, (const GLuint*)buffers->data());");}
        public static void glDeleteFramebuffers(int n, int[] framebuffers) {CPP.Add("$q->glDeleteFramebuffers(n, (const GLuint*)framebuffers->data());");}
        public static void glDeleteProgram(int program) {CPP.Add("$q->glDeleteProgram(program);");}
        public static void glDeleteRenderbuffers(int n, int[] renderbuffers) {CPP.Add("$q->glDeleteRenderbuffers(n, (const GLuint*)renderbuffers->data());");}
        public static void glDeleteShader(int shader) {CPP.Add("$q->glDeleteShader(shader);");}
        public static void glDeleteTextures(int n, int[] textures) {CPP.Add("$q->glDeleteTextures(n, (const GLuint*)textures->data());");}
        public static void glDepthFunc(int func) {CPP.Add("$q->glDepthFunc(func);");}
        public static void glDepthMask(int flag) {CPP.Add("$q->glDepthMask(flag);");}
        public static void glDepthRangef(float zNear, float zFar) {CPP.Add("$q->glDepthRangef(zNear, zFar);");}
        public static void glDetachShader(int program, int shader) {CPP.Add("$q->glDetachShader(program, shader);");}
        public static void glDisable(int cap) {CPP.Add("$q->glDisable(cap);");}
        public static void glDisableVertexAttribArray(int index) {CPP.Add("$q->glDisableVertexAttribArray(index);");}
        public static void glDrawArrays(int mode, int first, int count) {CPP.Add("$q->glDrawArrays(mode, first, count);");}
        public static void glDrawElements(int mode, int count, int type) {CPP.Add("$q->glDrawElements(mode, count, type, (const GLvoid*)nullptr);");}
        public static void glEnable(int cap) {CPP.Add("$q->glEnable(cap);");}
        public static void glEnableVertexAttribArray(int index) {CPP.Add("$q->glEnableVertexAttribArray(index);");}
        public static void glFinish() {CPP.Add("$q->glFinish();");}
        public static void glFlush() {CPP.Add("$q->glFlush();");}
        public static void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer) {CPP.Add("$q->glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);");}
        public static void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level) {CPP.Add("$q->glFramebufferTexture2D(target, attachment, textarget, texture, level);");}
        public static void glFrontFace(int mode) {CPP.Add("$q->glFrontFace(mode);");}
        public static void glGenBuffers(int n, int[] buffers) {CPP.Add("$q->glGenBuffers(n, (GLuint*)buffers->data());");}
        public static void glGenFramebuffers(int n, int[] framebuffers) {CPP.Add("$q->glGenFramebuffers(n, (GLuint*)framebuffers->data());");}
        public static void glGenRenderbuffers(int n, int[] renderbuffers) {CPP.Add("$q->glGenRenderbuffers(n, (GLuint*)renderbuffers->data());");}
        public static void glGenTextures(int n, int[] textures) {CPP.Add("$q->glGenTextures(n, (GLuint*)textures->data());");}
        public static void glGenerateMipmap(int target) {CPP.Add("$q->glGenerateMipmap(target);");}
        public static void glGetActiveAttrib(int program, int index, int bufsize, int[] length, int[] size, int[] type, byte[] name) {CPP.Add("$q->glGetActiveAttrib(program, index, bufsize, length->data(), size->data(), (GLenum*)type->data(), (char*)name->data());");}
        public static void glGetActiveUniform(int program, int index, int bufsize, int[] length, int[] size, int[] type, byte[] name) {CPP.Add("$q->glGetActiveUniform(program, index, bufsize, length->data(), size->data(), (GLenum*)type->data(), (char*)name->data());");}
        public static void glGetAttachedShaders(int program, int maxcount, int[] count, int[] shaders) {CPP.Add("$q->glGetAttachedShaders(program, maxcount, count->data(), (GLuint*)shaders->data());");}
        public static int glGetAttribLocation(int program, string name) {return CPP.ReturnInt("$q->glGetAttribLocation(program, $check(name)->cstring().constData())");}
        public static void glGetBooleanv(int pname, int[] args) {CPP.Add("$q->glGetBooleanv(pname, (GLboolean*)args->data());");}
        public static void glGetBufferParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glGetBufferParameteriv(target, pname, args->data());");}
        public static int glGetError() {return CPP.ReturnInt("$q->glGetError()");}
        public static void glGetFloatv(int pname, float[] args) {CPP.Add("$q->glGetFloatv(pname, args->data());");}
        public static void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, int[] args) {CPP.Add("$q->glGetFramebufferAttachmentParameteriv(target, attachment, pname, args->data());");}
        public static void glGetIntegerv(int pname, int[] args) {CPP.Add("$q->glGetIntegerv(pname, args->data());");}
        public static void glGetProgramInfoLog(int program, int bufsize, int[] length, byte[] infolog) {CPP.Add("$q->glGetProgramInfoLog(program, bufsize, length->data(), (char*)infolog->data());");}
        public static void glGetProgramiv(int program, int pname, int[] args) {CPP.Add("$q->glGetProgramiv(program, pname, args->data());");}
        public static void glGetRenderbufferParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glGetRenderbufferParameteriv(target, pname, args->data());");}
        public static void glGetShaderInfoLog(int shader, int bufsize, int[] length, byte[] infolog) {CPP.Add("$q->glGetShaderInfoLog(shader, bufsize, length->data(), (char*)infolog->data());");}
        public static void glGetShaderPrecisionFormat(int shadertype, int precisiontype, int[] range, int[] precision) {CPP.Add("$q->glGetShaderPrecisionFormat(shadertype, precisiontype, range->data(), precision->data());");}
        public static void glGetShaderSource(int shader, int bufsize, int[] length, byte []source) {CPP.Add("$q->glGetShaderSource(shader, bufsize, length->data(), (char*)source->data());");}
        public static void glGetShaderiv(int shader, int pname, int[] args) {CPP.Add("$q->glGetShaderiv(shader, pname, args->data());");}
        public static string glGetString(int name) {return CPP.ReturnString("Qt::Core::String::$new((const char*)$q->glGetString((GLenum)name))");}
        public static void glGetTexParameterfv(int target, int pname, float[] args) {CPP.Add("$q->glGetTexParameterfv(target, pname, args->data());");}
        public static void glGetTexParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glGetTexParameteriv(target, pname, args->data());");}
        public static int glGetUniformLocation(int program, string name) {return CPP.ReturnInt("$q->glGetUniformLocation(program, $check(name)->cstring().constData())");}
        public static void glGetUniformfv(int program, int location, float[] args) {CPP.Add("$q->glGetUniformfv(program, location, args->data());");}
        public static void glGetUniformiv(int program, int location, int[] args) {CPP.Add("$q->glGetUniformiv(program, location, args->data());");}
        public static void glGetVertexAttribPointerv(int index, int pname, long[] pointer) {CPP.Add("$q->glGetVertexAttribPointerv(index, pname, (GLvoid **)pointer->data());");}
        public static void glGetVertexAttribfv(int index, int pname, float[] args) {CPP.Add("$q->glGetVertexAttribfv(index, pname, args->data());");}
        public static void glGetVertexAttribiv(int index, int pname, int[] args) {CPP.Add("$q->glGetVertexAttribiv(index, pname, args->data());");}
        public static void glHint(int target, int mode) {CPP.Add("$q->glHint(target, mode);");}
        public static int glIsBuffer(int buffer) {return CPP.ReturnInt("$q->glIsBuffer(buffer)");}
        public static int glIsEnabled(int cap) {return CPP.ReturnInt("$q->glIsEnabled(cap)");}
        public static int glIsFramebuffer(int framebuffer) {return CPP.ReturnInt("$q->glIsFramebuffer(framebuffer)");}
        public static int glIsProgram(int program) {return CPP.ReturnInt("$q->glIsProgram(program)");}
        public static int glIsRenderbuffer(int renderbuffer) {return CPP.ReturnInt("$q->glIsRenderbuffer(renderbuffer)");}
        public static int glIsShader(int shader) {return CPP.ReturnInt("$q->glIsShader(shader)");}
        public static int glIsTexture(int texture) {return CPP.ReturnInt("$q->glIsTexture(texture)");}
        public static void glLineWidth(float width) {CPP.Add("$q->glLineWidth(width);");}
        public static void glLinkProgram(int program) {CPP.Add("$q->glLinkProgram(program);");}
        public static void glPixelStorei(int pname, int param) {CPP.Add("$q->glPixelStorei(pname, param);");}
        public static void glPolygonOffset(float factor, float units) {CPP.Add("$q->glPolygonOffset(factor, units);");}
        public static void glReadPixels(int x, int y, int width, int height, int format, int type, uint[] pixels) {CPP.Add("$q->glReadPixels(x, y, width, height, format, type, pixels->data());");}
        public static void glReleaseShaderCompiler() {CPP.Add("$q->glReleaseShaderCompiler();");}
        public static void glRenderbufferStorage(int target, int internalformat, int width, int height) {CPP.Add("$q->glRenderbufferStorage(target, internalformat, width, height);");}
        public static void glSampleCoverage(float value, int invert) {CPP.Add("$q->glSampleCoverage(value, invert);");}
        public static void glScissor(int x, int y, int width, int height) {CPP.Add("$q->glScissor(x, y, width, height);");}
        public static void glShaderBinary(int n, int[] shaders, int binaryformat, byte[] binary, int length) {CPP.Add("$q->glShaderBinary(n, (const GLuint*)shaders->data(), binaryformat, binary->data(), length);");}
        public static void glShaderSource(int shader, int count, string[] str, int[] length) {
            CPP.Add("QVector<const char*> src;");
            CPP.Add("for(int a=0;a<count;a++) {src.append($check($check(str)->at(a))->cstring().constData());}");
            CPP.Add("$q->glShaderSource(shader, count, (const char**)src.data(), length->data());");
        }
        public static void glStencilFunc(int func, int refc, int mask) {CPP.Add("$q->glStencilFunc(func, refc, mask);");}
        public static void glStencilFuncSeparate(int face, int func, int refc, int mask) {CPP.Add("$q->glStencilFuncSeparate(face, func, refc, mask);");}
        public static void glStencilMask(int mask) {CPP.Add("$q->glStencilMask(mask);");}
        public static void glStencilMaskSeparate(int face, int mask) {CPP.Add("$q->glStencilMaskSeparate(face, mask);");}
        public static void glStencilOp(int fail, int zfail, int zpass) {CPP.Add("$q->glStencilOp(fail, zfail, zpass);");}
        public static void glStencilOpSeparate(int face, int fail, int zfail, int zpass) {CPP.Add("$q->glStencilOpSeparate(face, fail, zfail, zpass);");}
        public static void glTexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, uint[] pixels) {CPP.Add("$q->glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels->data());");}
        public static void glTexParameterf(int target, int pname, float param) {CPP.Add("$q->glTexParameterf(target, pname, param);");}
        public static void glTexParameterfv(int target, int pname, float[] args) {CPP.Add("$q->glTexParameterfv(target, pname, args->data());");}
        public static void glTexParameteri(int target, int pname, int param) {CPP.Add("$q->glTexParameteri(target, pname, param);");}
        public static void glTexParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glTexParameteriv(target, pname, args->data());");}
        public static void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type, uint[] pixels) {CPP.Add("$q->glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels->data());");}

        public static void glUniform1f(int location, float x) {CPP.Add("$q->glUniform1f(location, x);");}
        public static void glUniform1fv(int location, int count, float[] v) {CPP.Add("$q->glUniform1fv(location, count, v->data());");}
        public static void glUniform1i(int location, int x) {CPP.Add("$q->glUniform1i(location, x);");}
        public static void glUniform1iv(int location, int count, int[] v) {CPP.Add("$q->glUniform1iv(location, count, v->data());");}
        public static void glUniform2f(int location, float x, float y) {CPP.Add("$q->glUniform2f(location, x, y);");}
        public static void glUniform2fv(int location, int count, float[] v) {CPP.Add("$q->glUniform2fv(location, count, v->data());");}
        public static void glUniform2i(int location, int x, int y) {CPP.Add("$q->glUniform2i(location, x, y);");}
        public static void glUniform2iv(int location, int count, int[] v) {CPP.Add("$q->glUniform2iv(location, count, v->data());");}
        public static void glUniform3f(int location, float x, float y, float z) {CPP.Add("$q->glUniform3f(location, x, y, z);");}
        public static void glUniform3fv(int location, int count, float[] v) {CPP.Add("$q->glUniform3fv(location, count, v->data());");}
        public static void glUniform3i(int location, int x, int y, int z) {CPP.Add("$q->glUniform3i(location, x, y, z);");}
        public static void glUniform3iv(int location, int count, int[] v) {CPP.Add("$q->glUniform3iv(location, count, v->data());");}
        public static void glUniform4f(int location, float x, float y, float z, float w) {CPP.Add("$q->glUniform4f(location, x, y, z, w);");}
        public static void glUniform4fv(int location, int count, float[] v) {CPP.Add("$q->glUniform4fv(location, count, v->data());");}
        public static void glUniform4i(int location, int x, int y, int z, int w) {CPP.Add("$q->glUniform4i(location, x, y, z, w);");}
        public static void glUniform4iv(int location, int count, int[] v) {CPP.Add("$q->glUniform4iv(location, count, v->data());");}

        public static void glUniformMatrix2fv(int location, int count, int transpose, float[] value) {CPP.Add("$q->glUniformMatrix2fv(location, count, transpose, value->data());");}
        public static void glUniformMatrix3fv(int location, int count, int transpose, float[] value) {CPP.Add("$q->glUniformMatrix3fv(location, count, transpose, value->data());");}
        public static void glUniformMatrix4fv(int location, int count, int transpose, float[] value) {CPP.Add("$q->glUniformMatrix4fv(location, count, transpose, value->data());");}
        public static void glUseProgram(int program) {CPP.Add("$q->glUseProgram(program);");}
        public static void glValidateProgram(int program) {CPP.Add("$q->glValidateProgram(program);");}
        public static void glVertexAttrib1f(int index, float x) {CPP.Add("$q->glVertexAttrib1f(index, x);");}
        public static void glVertexAttrib1fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib1fv(index, values->data());");}
        public static void glVertexAttrib2f(int index, float x, float y) {CPP.Add("$q->glVertexAttrib2f(index, x, y);");}
        public static void glVertexAttrib2fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib2fv(index, values->data());");}
        public static void glVertexAttrib3f(int index, float x, float y, float z) {CPP.Add("$q->glVertexAttrib3f(index, x, y, z);");}
        public static void glVertexAttrib3fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib3fv(index, values->data());");}
        public static void glVertexAttrib4f(int index, float x, float y, float z, float w) {CPP.Add("$q->glVertexAttrib4f(index, x, y, z, w);");}
        public static void glVertexAttrib4fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib4fv(index, values->data());");}
        public static void glVertexAttribPointer(int index, int size, int type, int normalized, int stride, byte[] ptr) {CPP.Add("$q->glVertexAttribPointer(index, size, type, normalized, stride, ptr->data());");}
        public static void glVertexAttribPointer(int index, int size, int type, int normalized, int stride, float[] ptr) {CPP.Add("$q->glVertexAttribPointer(index, size, type, normalized, stride, ptr->data());");}
        public static void glVertexAttribPointer(int index, int size, int type, int normalized, int stride) {CPP.Add("$q->glVertexAttribPointer(index, size, type, normalized, stride, nullptr);");}
        public static void glViewport(int x, int y, int width, int height) {CPP.Add("$q->glViewport(x, y, width, height);");}
//        public static bool hasOpenGLFeature($q->OpenGLFeature feature) { }  //TODO
    }
}
