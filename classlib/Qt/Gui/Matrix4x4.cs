using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QMatrix4x4")]
    public class Matrix4x4 {
        public void Perspective(float verticalAngle, float aspectRatio, float nearPlane, float farPlane) {
            CPP.Add("perspective(verticalAngle, aspectRatio, nearPlane, farPlane);");
        }
        public void Translate(float x, float y, float z) {
            CPP.Add("translate(x, y, z);");
        }
        public void Rotate(float a, float x, float y, float z) {
            CPP.Add("rotate(a, x, y, z);");
        }
    }
}
