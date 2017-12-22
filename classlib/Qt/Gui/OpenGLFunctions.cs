using Qt.Core;
using Qt.QSharp;

//TODO : $check() all arrays

namespace Qt.Gui {
    [CPPClass("private: std::unique_ptr<QOpenGLFunctions> $q;")]
    public class OpenGLFunctions : OpenGLConstants {
        /** Initialize the GL functions. */
        public void InitializeOpenGLFunctions() {
            CPP.Add("$q = std::make_unique<QOpenGLFunctions>();");
            CPP.Add("$q->initializeOpenGLFunctions();");
        }

        public void glActiveTexture(int texture) {CPP.Add("$q->glActiveTexture(texture);");}
        public void glAttachShader(int program, int shader) {CPP.Add("$q->glAttachShader(program, shader);");}
        public void glBindAttribLocation(int program, int index, string name) {CPP.Add("$q->glBindAttribLocation(program,index,$check(name)->cstring().constData());");}
        public void glBindBuffer(int target, int buffer) {CPP.Add("$q->glBindBuffer(target, buffer);");}
        public void glBindFramebuffer(int target, int framebuffer) {CPP.Add("$q->glBindFramebuffer(target, framebuffer);");}
        public void glBindRenderbuffer(int target, int renderbuffer) {CPP.Add("$q->glBindRenderbuffer(target, renderbuffer);");}
        public void glBindTexture(int target, int texture) {CPP.Add("$q->glBindTexture(target, texture);");}
        public void glBlendColor(float red, float green, float blue, float alpha) {CPP.Add("$q->glBlendColor(red, green, blue, alpha);");}
        public void glBlendEquation(int mode) {CPP.Add("$q->glBlendEquation(mode);");}
        public void glBlendEquationSeparate(int modeRGB, int modeAlpha) {CPP.Add("$q->glBlendEquationSeparate(modeRGB, modeAlpha);");}
        public void glBlendFunc(int sfactor, int dfactor) {CPP.Add("$q->glBlendFunc(sfactor, dfactor);");}
        public void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha) {CPP.Add("$q->glBlendFuncSeparate(srcRGB, dstRGB, srcAlpha, dstAlpha);");}
        public void glBufferData(int target, int size, byte[] data, int usage) {CPP.Add("$q->glBufferData(target, size, $check(data, 0, size)->data(), usage);");}
        public void glBufferSubData(int target, int offset, int size, byte[] data) {CPP.Add("$q->glBufferSubData(target, offset, size, $check(data, 0, size)->data());");}
        public int glCheckFramebufferStatus(int target) {return CPP.ReturnInt("$q->glCheckFramebufferStatus(target)");}
        public void glClear(int mask) {CPP.Add("$q->glClear(mask);");}
        public void glClearColor(float red, float green, float blue, float alpha) {CPP.Add("$q->glClearColor(red, green, blue, alpha);");}
        public void glClearDepthf(float depth) {CPP.Add("$q->glClearDepthf(depth);");}
        public void glClearStencil(int s) {CPP.Add("$q->glClearStencil(s);");}
        public void glColorMask(int red, int green, int blue, int alpha) {CPP.Add("$q->glColorMask(red, green, blue, alpha);");}
        public void glCompileShader(int shader) {CPP.Add("$q->glCompileShader(shader);");}
        public void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border, int imageSize, byte[] data) {CPP.Add("$q->glCompressedTexImage2D(target, level, internalformat, width, height, border, imageSize, data->data());");}
        public void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int imageSize, byte[] data) {CPP.Add("$q->glCompressedTexSubImage2D(target, level, xoffset, yoffset, width, height, format, imageSize, data->data());");}
        public void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border) {CPP.Add("$q->glCopyTexImage2D(target, level, internalformat, x, y, width, height, border);");}
        public void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height) {CPP.Add("$q->glCopyTexSubImage2D(target, level, xoffset, yoffset, x, y, width, height);");}
        public int glCreateProgram() {return CPP.ReturnInt("$q->glCreateProgram();");}
        public int glCreateShader(int type) {return CPP.ReturnInt("$q->glCreateShader(type)");}
        public void glCullFace(int mode) {CPP.Add("$q->glCullFace(mode);");}
        public void glDeleteBuffers(int n, int[] buffers) {CPP.Add("$q->glDeleteBuffers(n, (const GLuint*)buffers->data());");}
        public void glDeleteFramebuffers(int n, int[] framebuffers) {CPP.Add("$q->glDeleteFramebuffers(n, (const GLuint*)framebuffers->data());");}
        public void glDeleteProgram(int program) {CPP.Add("$q->glDeleteProgram(program);");}
        public void glDeleteRenderbuffers(int n, int[] renderbuffers) {CPP.Add("$q->glDeleteRenderbuffers(n, (const GLuint*)renderbuffers->data());");}
        public void glDeleteShader(int shader) {CPP.Add("$q->glDeleteShader(shader);");}
        public void glDeleteTextures(int n, int[] textures) {CPP.Add("$q->glDeleteTextures(n, (const GLuint*)textures->data());");}
        public void glDepthFunc(int func) {CPP.Add("$q->glDepthFunc(func);");}
        public void glDepthMask(int flag) {CPP.Add("$q->glDepthMask(flag);");}
        public void glDepthRangef(float zNear, float zFar) {CPP.Add("$q->glDepthRangef(zNear, zFar);");}
        public void glDetachShader(int program, int shader) {CPP.Add("$q->glDetachShader(program, shader);");}
        public void glDisable(int cap) {CPP.Add("$q->glDisable(cap);");}
        public void glDisableVertexAttribArray(int index) {CPP.Add("$q->glDisableVertexAttribArray(index);");}
        public void glDrawArrays(int mode, int first, int count) {CPP.Add("$q->glDrawArrays(mode, first, count);");}
        public void glDrawElements(int mode, int count, int type, int[] indices) {CPP.Add("$q->glDrawElements(mode, count, type, (const GLvoid*)indices->data());");}
        public void glEnable(int cap) {CPP.Add("$q->glEnable(cap);");}
        public void glEnableVertexAttribArray(int index) {CPP.Add("$q->glEnableVertexAttribArray(index);");}
        public void glFinish() {CPP.Add("$q->glFinish();");}
        public void glFlush() {CPP.Add("$q->glFlush();");}
        public void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer) {CPP.Add("$q->glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);");}
        public void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level) {CPP.Add("$q->glFramebufferTexture2D(target, attachment, textarget, texture, level);");}
        public void glFrontFace(int mode) {CPP.Add("$q->glFrontFace(mode);");}
        public void glGenBuffers(int n, int[] buffers) {CPP.Add("$q->glGenBuffers(n, (GLuint*)buffers->data());");}
        public void glGenFramebuffers(int n, int[] framebuffers) {CPP.Add("$q->glGenFramebuffers(n, (GLuint*)framebuffers->data());");}
        public void glGenRenderbuffers(int n, int[] renderbuffers) {CPP.Add("$q->glGenRenderbuffers(n, (GLuint*)renderbuffers->data());");}
        public void glGenTextures(int n, int[] textures) {CPP.Add("$q->glGenTextures(n, (GLuint*)textures->data());");}
        public void glGenerateMipmap(int target) {CPP.Add("$q->glGenerateMipmap(target);");}
        public void glGetActiveAttrib(int program, int index, int bufsize, int[] length, int[] size, int[] type, byte[] name) {CPP.Add("$q->glGetActiveAttrib(program, index, bufsize, length->data(), size->data(), (GLenum*)type->data(), (char*)name->data());");}
        public void glGetActiveUniform(int program, int index, int bufsize, int[] length, int[] size, int[] type, byte[] name) {CPP.Add("$q->glGetActiveUniform(program, index, bufsize, length->data(), size->data(), (GLenum*)type->data(), (char*)name->data());");}
        public void glGetAttachedShaders(int program, int maxcount, int[] count, int[] shaders) {CPP.Add("$q->glGetAttachedShaders(program, maxcount, count->data(), (GLuint*)shaders->data());");}
        public int glGetAttribLocation(int program, string name) {return CPP.ReturnInt("$q->glGetAttribLocation(program, $check(name)->cstring().constData())");}
        public void glGetBooleanv(int pname, int[] args) {CPP.Add("$q->glGetBooleanv(pname, (GLboolean*)args->data());");}
        public void glGetBufferParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glGetBufferParameteriv(target, pname, args->data());");}
        public int glGetError() {return CPP.ReturnInt("$q->glGetError()");}
        public void glGetFloatv(int pname, float[] args) {CPP.Add("$q->glGetFloatv(pname, args->data());");}
        public void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, int[] args) {CPP.Add("$q->glGetFramebufferAttachmentParameteriv(target, attachment, pname, args->data());");}
        public void glGetIntegerv(int pname, int[] args) {CPP.Add("$q->glGetIntegerv(pname, args->data());");}
        public void glGetProgramInfoLog(int program, int bufsize, int[] length, byte[] infolog) {CPP.Add("$q->glGetProgramInfoLog(program, bufsize, length->data(), (char*)infolog->data());");}
        public void glGetProgramiv(int program, int pname, int[] args) {CPP.Add("$q->glGetProgramiv(program, pname, args->data());");}
        public void glGetRenderbufferParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glGetRenderbufferParameteriv(target, pname, args->data());");}
        public void glGetShaderInfoLog(int shader, int bufsize, int[] length, byte[] infolog) {CPP.Add("$q->glGetShaderInfoLog(shader, bufsize, length->data(), (char*)infolog->data());");}
        public void glGetShaderPrecisionFormat(int shadertype, int precisiontype, int[] range, int[] precision) {CPP.Add("$q->glGetShaderPrecisionFormat(shadertype, precisiontype, range->data(), precision->data());");}
        public void glGetShaderSource(int shader, int bufsize, int[] length, byte []source) {CPP.Add("$q->glGetShaderSource(shader, bufsize, length->data(), (char*)source->data());");}
        public void glGetShaderiv(int shader, int pname, int[] args) {CPP.Add("$q->glGetShaderiv(shader, pname, args->data());");}
        public string glGetString(int name) {return CPP.ReturnString("String::$new((const char*)$q->glGetString((GLenum)name))");}
        public void glGetTexParameterfv(int target, int pname, float[] args) {CPP.Add("$q->glGetTexParameterfv(target, pname, args->data());");}
        public void glGetTexParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glGetTexParameteriv(target, pname, args->data());");}
        public int glGetUniformLocation(int program, string name) {return CPP.ReturnInt("$q->glGetUniformLocation(program, $check(name)->cstring().constData())");}
        public void glGetUniformfv(int program, int location, float[] args) {CPP.Add("$q->glGetUniformfv(program, location, args->data());");}
        public void glGetUniformiv(int program, int location, int[] args) {CPP.Add("$q->glGetUniformiv(program, location, args->data());");}
        public void glGetVertexAttribPointerv(int index, int pname, long[] pointer) {CPP.Add("$q->glGetVertexAttribPointerv(index, pname, (GLvoid **)pointer->data());");}
        public void glGetVertexAttribfv(int index, int pname, float[] args) {CPP.Add("$q->glGetVertexAttribfv(index, pname, args->data());");}
        public void glGetVertexAttribiv(int index, int pname, int[] args) {CPP.Add("$q->glGetVertexAttribiv(index, pname, args->data());");}
        public void glHint(int target, int mode) {CPP.Add("$q->glHint(target, mode);");}
        public int glIsBuffer(int buffer) {return CPP.ReturnInt("$q->glIsBuffer(buffer)");}
        public int glIsEnabled(int cap) {return CPP.ReturnInt("$q->glIsEnabled(cap)");}
        public int glIsFramebuffer(int framebuffer) {return CPP.ReturnInt("$q->glIsFramebuffer(framebuffer)");}
        public int glIsProgram(int program) {return CPP.ReturnInt("$q->glIsProgram(program)");}
        public int glIsRenderbuffer(int renderbuffer) {return CPP.ReturnInt("$q->glIsRenderbuffer(renderbuffer)");}
        public int glIsShader(int shader) {return CPP.ReturnInt("$q->glIsShader(shader)");}
        public int glIsTexture(int texture) {return CPP.ReturnInt("$q->glIsTexture(texture)");}
        public void glLineWidth(float width) {CPP.Add("$q->glLineWidth(width);");}
        public void glLinkProgram(int program) {CPP.Add("$q->glLinkProgram(program);");}
        public void glPixelStorei(int pname, int param) {CPP.Add("$q->glPixelStorei(pname, param);");}
        public void glPolygonOffset(float factor, float units) {CPP.Add("$q->glPolygonOffset(factor, units);");}
        public void glReadPixels(int x, int y, int width, int height, int format, int type, int[] pixels) {CPP.Add("$q->glReadPixels(x, y, width, height, format, type, pixels->data());");}
        public void glReleaseShaderCompiler() {CPP.Add("$q->glReleaseShaderCompiler();");}
        public void glRenderbufferStorage(int target, int internalformat, int width, int height) {CPP.Add("$q->glRenderbufferStorage(target, internalformat, width, height);");}
        public void glSampleCoverage(float value, int invert) {CPP.Add("$q->glSampleCoverage(value, invert);");}
        public void glScissor(int x, int y, int width, int height) {CPP.Add("$q->glScissor(x, y, width, height);");}
        public void glShaderBinary(int n, int[] shaders, int binaryformat, byte[] binary, int length) {CPP.Add("$q->glShaderBinary(n, (const GLuint*)shaders->data(), binaryformat, binary->data(), length);");}
        public void glShaderSource(int shader, int count, string[] str, int[] length) {
            CPP.Add("QVector<const char*> src;");
            CPP.Add("for(int a=0;a<count;a++) {src.append($check($check(str)->at(a))->cstring().constData());}");
            CPP.Add("$q->glShaderSource(shader, count, (const char**)src.data(), length->data());");
        }
        public void glStencilFunc(int func, int refc, int mask) {CPP.Add("$q->glStencilFunc(func, refc, mask);");}
        public void glStencilFuncSeparate(int face, int func, int refc, int mask) {CPP.Add("$q->glStencilFuncSeparate(face, func, refc, mask);");}
        public void glStencilMask(int mask) {CPP.Add("$q->glStencilMask(mask);");}
        public void glStencilMaskSeparate(int face, int mask) {CPP.Add("$q->glStencilMaskSeparate(face, mask);");}
        public void glStencilOp(int fail, int zfail, int zpass) {CPP.Add("$q->glStencilOp(fail, zfail, zpass);");}
        public void glStencilOpSeparate(int face, int fail, int zfail, int zpass) {CPP.Add("$q->glStencilOpSeparate(face, fail, zfail, zpass);");}
        public void glTexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, int[] pixels) {CPP.Add("$q->glTexImage2D(target, level, internalformat, width, height, border, format, type, pixels->data());");}
        public void glTexParameterf(int target, int pname, float param) {CPP.Add("$q->glTexParameterf(target, pname, param);");}
        public void glTexParameterfv(int target, int pname, float[] args) {CPP.Add("$q->glTexParameterfv(target, pname, args->data());");}
        public void glTexParameteri(int target, int pname, int param) {CPP.Add("$q->glTexParameteri(target, pname, param);");}
        public void glTexParameteriv(int target, int pname, int[] args) {CPP.Add("$q->glTexParameteriv(target, pname, args->data());");}
        public void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type, int[] pixels) {CPP.Add("$q->glTexSubImage2D(target, level, xoffset, yoffset, width, height, format, type, pixels->data());");}

        public void glUniform1f(int location, float x) {CPP.Add("$q->glUniform1f(location, x);");}
        public void glUniform1fv(int location, int count, float[] v) {CPP.Add("$q->glUniform1fv(location, count, v->data());");}
        public void glUniform1i(int location, int x) {CPP.Add("$q->glUniform1i(location, x);");}
        public void glUniform1iv(int location, int count, int[] v) {CPP.Add("$q->glUniform1iv(location, count, v->data());");}
        public void glUniform2f(int location, float x, float y) {CPP.Add("$q->glUniform2f(location, x, y);");}
        public void glUniform2fv(int location, int count, float[] v) {CPP.Add("$q->glUniform2fv(location, count, v->data());");}
        public void glUniform2i(int location, int x, int y) {CPP.Add("$q->glUniform2i(location, x, y);");}
        public void glUniform2iv(int location, int count, int[] v) {CPP.Add("$q->glUniform2iv(location, count, v->data());");}
        public void glUniform3f(int location, float x, float y, float z) {CPP.Add("$q->glUniform3f(location, x, y, z);");}
        public void glUniform3fv(int location, int count, float[] v) {CPP.Add("$q->glUniform3fv(location, count, v->data());");}
        public void glUniform3i(int location, int x, int y, int z) {CPP.Add("$q->glUniform3i(location, x, y, z);");}
        public void glUniform3iv(int location, int count, int[] v) {CPP.Add("$q->glUniform3iv(location, count, v->data());");}
        public void glUniform4f(int location, float x, float y, float z, float w) {CPP.Add("$q->glUniform4f(location, x, y, z, w);");}
        public void glUniform4fv(int location, int count, float[] v) {CPP.Add("$q->glUniform4fv(location, count, v->data());");}
        public void glUniform4i(int location, int x, int y, int z, int w) {CPP.Add("$q->glUniform4i(location, x, y, z, w);");}
        public void glUniform4iv(int location, int count, int[] v) {CPP.Add("$q->glUniform4iv(location, count, v->data());");}

        public void glUniformMatrix2fv(int location, int count, int transpose, float[] value) {CPP.Add("$q->glUniformMatrix2fv(location, count, transpose, value->data());");}
        public void glUniformMatrix3fv(int location, int count, int transpose, float[] value) {CPP.Add("$q->glUniformMatrix3fv(location, count, transpose, value->data());");}
        public void glUniformMatrix4fv(int location, int count, int transpose, float[] value) {CPP.Add("$q->glUniformMatrix4fv(location, count, transpose, value->data());");}
        public void glUseProgram(int program) {CPP.Add("$q->glUseProgram(program);");}
        public void glValidateProgram(int program) {CPP.Add("$q->glValidateProgram(program);");}
        public void glVertexAttrib1f(int index, float x) {CPP.Add("$q->glVertexAttrib1f(index, x);");}
        public void glVertexAttrib1fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib1fv(index, values->data());");}
        public void glVertexAttrib2f(int index, float x, float y) {CPP.Add("$q->glVertexAttrib2f(index, x, y);");}
        public void glVertexAttrib2fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib2fv(index, values->data());");}
        public void glVertexAttrib3f(int index, float x, float y, float z) {CPP.Add("$q->glVertexAttrib3f(index, x, y, z);");}
        public void glVertexAttrib3fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib3fv(index, values->data());");}
        public void glVertexAttrib4f(int index, float x, float y, float z, float w) {CPP.Add("$q->glVertexAttrib4f(index, x, y, z, w);");}
        public void glVertexAttrib4fv(int index, float[] values) {CPP.Add("$q->glVertexAttrib4fv(index, values->data());");}
        public void glVertexAttribPointer(int index, int size, int type, int normalized, int stride, byte[] ptr) {CPP.Add("$q->glVertexAttribPointer(index, size, type, normalized, stride, ptr->data());");}
        public void glVertexAttribPointer(int index, int size, int type, int normalized, int stride, float[] ptr) {CPP.Add("$q->glVertexAttribPointer(index, size, type, normalized, stride, ptr->data());");}
        public void glViewport(int x, int y, int width, int height) {CPP.Add("$q->glViewport(x, y, width, height);");}
//        public bool hasOpenGLFeature($q->OpenGLFeature feature) { }  //TODO
    }
}
