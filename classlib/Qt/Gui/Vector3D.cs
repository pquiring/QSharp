using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "std::shared_ptr<QVector3D> $q;" +
        "QVector3D* $value() {return $q.get();}"
    )]
    public class Vector3D {
        public Vector3D() {
            CPP.Add("$q = std::make_shared<QVector3D>();");
        }
        public Vector3D(float x, float y, float z) {
            CPP.Add("$q = std::make_shared<QVector3D>(x,y,z);");
        }
        [CPPReplaceArgs("QVector3D arg1")]
        private Vector3D(NativeArg1 arg1) {
            CPP.Add("$q = std::make_shared<QVector3D>(arg1);");
        }
        public float GetX() {return CPP.ReturnFloat("$q->x()");}
        public float GetY() {return CPP.ReturnFloat("$q->y()");}
        public float GetZ() {return CPP.ReturnFloat("$q->z()");}
        public void Set(Vector3D other) {
            Set(other.GetX(), other.GetY(), other.GetZ());
        }
        public void Set(float x, float y, float z) {
            CPP.Add("$q->setX(x);");
            CPP.Add("$q->setY(y);");
            CPP.Add("$q->setZ(z);");
        }
        public void SetX(float x) {CPP.Add("$q->setX(x);");}
        public void SetY(float y) {CPP.Add("$q->setY(y);");}
        public void SetZ(float z) {CPP.Add("$q->setZ(z);");}
        public float Length() {
            return CPP.ReturnFloat("$q->length()");
        }
        public float LengthSquared() {
            return CPP.ReturnFloat("$q->lengthSquared()");
        }
        public void Normalize() {
            CPP.Add("$q->normalize();");
        }
        public void Add(Vector3D other) {
            CPP.Add("(*$q.get())+=(*(other->$value()));");
        }
        public void Sub(Vector3D other) {
            CPP.Add("(*$q.get())-=(*(other->$value()));");
        }
        public void Multiply(float factor) {
            CPP.Add("(*$q.get())*=factor;");
        }
        public void Divide(float divsor) {
            CPP.Add("(*$q.get())/=divsor;");
        }
        public static float DotProduct(Vector3D a, Vector3D b) {
            return CPP.ReturnFloat("QVector3D::dotProduct(*($check(a)->$q), *($check(b)->$q))");
        }
        public static Vector3D CrossProduct(Vector3D a, Vector3D b) {
            return (Vector3D)CPP.ReturnObject("Vector3D::$new(QVector3D::crossProduct(*($check(a)->$q), *($check(b)->$q)))");
        }
    }
}
