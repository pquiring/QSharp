using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: std::shared_ptr<QMatrix4x4> $q;" +
        "public: QMatrix4x4* $value() {return $q.get();}"
    )]
    public class Matrix4x4 {
        public Matrix4x4() {
            CPP.Add("$q = std::make_shared<QMatrix4x4>();");
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
    }
}
