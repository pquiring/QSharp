using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "private: std::shared_ptr<QMatrix4x4> $q;" +
        "public: QMatrix4x4* $value() {return $q.get();}"
    )]
    //NOTE : Data is stored internally as column-major format (OpenGL standard).
    public class Matrix4x4 {
        private float[] m = null;
        public Matrix4x4() {
            CPP.Add("$q = std::make_shared<QMatrix4x4>();");
            CPP.Add("m = Qt::QSharp::FixedArray<float>::$new($q->data(), 16);");
        }
        public void Perspective(float verticalAngle, float aspectRatio, float nearPlane, float farPlane) {
            CPP.Add("$q->perspective(verticalAngle, aspectRatio, nearPlane, farPlane);");
        }
        public void Ortho(float left, float right, float bottom, float top, float nearPlane, float farPlane) {
            CPP.Add("$q->ortho(left, right, bottom, top, nearPlane, farPlane);");
        }
        public void Translate(float x, float y, float z) {
            CPP.Add("$q->translate(x, y, z);");
        }
        public void Rotate(float a, float x, float y, float z) {
            CPP.Add("$q->rotate(a, x, y, z);");
        }
        public void Scale(float x, float y, float z) {
            CPP.Add("$q->scale(x, y, z);");
        }
        public void SetIdentity() {
            CPP.Add("$q->setToIdentity();");
        }
        /** Sets identity except preserves translation (effectively removes scale/rotation) */
        public void SetIdentity3x3() {
            for(int a=0;a<11;a++) {if (a % 5 == 0) m[a] = 1.0f; else m[a] = 0.0f;}
        }
        /** Set matrix using axis-angle(vector) (see http://en.wikipedia.org/wiki/Axis_angle) */
        public void SetAxisAngle(float angle, float x, float y, float z) {
            float xx, yy, zz, xy, yz, zx, xs, ys, zs, one_c, s, c;

            s = (float)Math.Sin( angle * (float)Math.PI / 180.0f );
            c = (float)Math.Cos( angle * (float)Math.PI / 180.0f );

            SetIdentity();

            if (x == 0.0f) {
                if (y == 0.0f) {
                    if (z != 0.0f) {
                        /* rotate only around z-axis */
                        m[0+0*4] = c;
                        m[1+1*4] = c;
                        if (z < 0.0) {
                            m[0+1*4] = s;
                            m[1+0*4] = -s;
                        } else {
                            m[0+1*4] = -s;
                            m[1+0*4] = s;
                        }
                        return;
                    }
                } else if (z == 0.0f) {
                    /* rotate only around y-axis */
                    m[0+0*4] = c;
                    m[2+2*4] = c;
                    if (y < 0.0) {
                        m[0+2*4] = -s;
                        m[2+0*4] = s;
                    } else {
                        m[0+2*4] = s;
                        m[2+0*4] = -s;
                    }
                    return;
                }
            } else if (y == 0.0f) {
                if (z == 0.0f) {
                    /* rotate only around x-axis */
                    m[1+1*4] = c;
                    m[2+2*4] = c;
                    if (x < 0.0) {
                        m[1+2*4] = s;
                        m[2+1*4] = -s;
                    } else {
                        m[1+2*4] = -s;
                        m[2+1*4] = s;
                    }
                    return;
                }
            }

            float mag;
            //complex rotation
            mag = (float)Math.Sqrt(x * x + y * y + z * z);
            if (mag <= 1.0e-4f) return;        //rotation too small
            x /= mag;
            y /= mag;
            z /= mag;
            xx = x * x;
            yy = y * y;
            zz = z * z;
            xy = x * y;
            yz = y * z;
            zx = z * x;
            xs = x * s;
            ys = y * s;
            zs = z * s;
            one_c = 1.0f - c;
            m[0+0*4] = (one_c * xx) + c;
            m[0+1*4] = (one_c * xy) - zs;
            m[0+2*4] = (one_c * zx) + ys;
            m[1+0*4] = (one_c * xy) + zs;
            m[1+1*4] = (one_c * yy) + c;
            m[1+2*4] = (one_c * yz) - xs;
            m[2+0*4] = (one_c * zx) - ys;
            m[2+1*4] = (one_c * yz) + xs;
            m[2+2*4] = (one_c * zz) + c;
        }
        public void Multiply(Matrix4x4 other) {
            CPP.Add("(*$q.get())*=(*(other->$value()));");
        }
        public void ReverseTranslate() {
            m[0+3*4] *= -1.0f;
            m[1+3*4] *= -1.0f;
            m[2+3*4] *= -1.0f;
        }
    }
}
