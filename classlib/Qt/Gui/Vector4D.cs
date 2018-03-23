using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "std::shared_ptr<QVector4D> $q;" +
        "QVector4D* $value() {return $q.get();}"
    )]
    public class Vector4D {
        public Vector4D() {
            CPP.Add("$q = std::make_shared<QVector4D>();");
        }
        public Vector4D(float x, float y, float z, float w) {
            CPP.Add("$q = std::make_shared<QVector4D>(x,y,z,w);");
        }
        [CPPReplaceArgs("QVector4D arg1")]
        private Vector4D(NativeArg1 arg1) {
            CPP.Add("$q = std::make_shared<QVector4D>(arg1);");
        }
        public float GetX() {return CPP.ReturnFloat("$q->x()");}
        public float GetY() {return CPP.ReturnFloat("$q->y()");}
        public float GetZ() {return CPP.ReturnFloat("$q->z()");}
        public float GetW() {return CPP.ReturnFloat("$q->w()");}
        public void Set(float x, float y, float z, float w) {
            CPP.Add("$q->setX(x);");
            CPP.Add("$q->setY(y);");
            CPP.Add("$q->setZ(z);");
            CPP.Add("$q->setW(w);");
        }
        public float Length() {
            return CPP.ReturnFloat("$q->length()");
        }
        public float LengthSquared() {
            return CPP.ReturnFloat("$q->lengthSquared()");
        }
        public void Normalize() {
            CPP.Add("$q->normalize();");
        }
        public void Add(Vector4D other) {
            CPP.Add("(*$q.get())+=(*(other->$value()));");
        }
        public void Sub(Vector4D other) {
            CPP.Add("(*$q.get())-=(*(other->$value()));");
        }
        public void Multiply(float factor) {
            CPP.Add("(*$q.get())*=factor;");
        }
        public void Divide(float divsor) {
            CPP.Add("(*$q.get())/=divsor;");
        }
        public static float DotProduct(Vector4D a, Vector4D b) {
            return CPP.ReturnFloat("QVector4D::dotProduct(*($check(a)->$q), *($check(b)->$q))");
        }
    }
}
