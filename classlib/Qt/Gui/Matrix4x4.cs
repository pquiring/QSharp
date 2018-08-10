using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "std::qt_ptr<QMatrix4x4> $q;" +
        "std::qt_ptr<QMatrix4x4> $t;" +
        "QMatrix4x4* $value() {return $q.get();}"
    )]
    //NOTE : Data is stored internally as column-major format (OpenGL standard).
    public class Matrix4x4 {
        private float[] m = null;
        private Vector3D vector = new Vector3D();
        public Matrix4x4() {
            CPP.Add("$q = new QMatrix4x4();");
            CPP.Add("m = new Qt::QSharp::FixedArray<float>($q->data(), 16);");
        }
        public Matrix4x4(Matrix4x4 other) {
            CPP.Add("$q = new QMatrix4x4();");
            CPP.Add("m = new Qt::QSharp::FixedArray<float>($q->data(), 16);");
            for(int a=0;a<16;a++) {
                m[a] = other.m[a];
            }
        }
        public Matrix4x4 Clone() {
            return new Matrix4x4(this);
        }
        public float[] GetBuffer() {
            return m;
        }
        public void Perspective(float verticalAngle, float aspectRatio, float nearPlane, float farPlane) {
            CPP.Add("$q->perspective(verticalAngle, aspectRatio, nearPlane, farPlane);");
        }
        public void Ortho(float left, float right, float bottom, float top, float nearPlane, float farPlane) {
            CPP.Add("$q->ortho(left, right, bottom, top, nearPlane, farPlane);");
        }
        /** Adds translation to matrix ignoring current rotation/scale. */
        public void Translate(float x, float y, float z) {
            m[0+3*4] += x;
            m[1+3*4] += y;
            m[2+3*4] += z;
        }
        /** Adds translation to matrix with current rotation/scale. */
        public void Translate2(float x, float y, float z) {
            CPP.Add("$q->translate(x, y, z);");
        }
        /** Adds rotatation to matrix ignoring current translation. */
        public void Rotate(float a, float x, float y, float z) {
            float tx = m[12];
            float ty = m[13];
            float tz = m[14];
            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
            CPP.Add("$q->rotate(a, x, y, z);");
            m[12] = tx;
            m[13] = ty;
            m[14] = tz;
        }
        /** Adds rotation to matrix with current translation. */
        public void Rotate2(float a, float x, float y, float z) {
            CPP.Add("$q->rotate(a, x, y, z);");
        }
        /** Adds rotation adjusted to current rotation but ignoring translation. */
        public void Rotate3(float a, float x, float y, float z) {
            vector.Set(x,y,z);
            Multiply3x3(vector);
            x = vector.GetX();
            y = vector.GetX();
            z = vector.GetX();
            float tx = m[12];
            float ty = m[13];
            float tz = m[14];
            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
            CPP.Add("$q->rotate(a, x, y, z);");
            m[12] = tx;
            m[13] = ty;
            m[14] = tz;
        }
        /** Adds rotation adjusted to current rotation with current translation. */
        public void Rotate4(float a, float x, float y, float z) {
            vector.Set(x,y,z);
            Multiply3x3(vector);
            x = vector.GetX();
            y = vector.GetX();
            z = vector.GetX();
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
            SetIdentity();
            Rotate2(angle, x, y, z);
        }
        /** this = this * other */
        public void Multiply4x4(Matrix4x4 other) {
            CPP.Add("(*$q.get())*=(*($check(other)->$value()));");
        }
        /** vec = this * vec */
        public void Multiply4x4(Vector3D vec) {
            CPP.Add("QVector3D res = $q->map(*($check(vec)->$value()));");
            CPP.Add("vec->Set(res.x(), res.y(), res.z());");
        }
        /** vec = this * vec (3x3 matrix only - ignores translation/projection) */
        public void Multiply3x3(Vector3D vec) {
            CPP.Add("QVector3D res = $q->mapVector(*($check(vec)->$value()));");
            CPP.Add("vec->Set(res.x(), res.y(), res.z());");
        }
        public void ReverseTranslate() {
            m[0+3*4] *= -1.0f;
            m[1+3*4] *= -1.0f;
            m[2+3*4] *= -1.0f;
        }
        public void LookAt(Vector3D eye, Vector3D at, Vector3D up) {
            CPP.Add("$q->lookAt(*$check(eye)->$value(),*$check(at)->$value(),*$check(up)->$value());");
        }
    }
}
