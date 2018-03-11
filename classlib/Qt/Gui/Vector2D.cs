using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: std::shared_ptr<QVector2D> $q;" +
        "public: QVector2D* $value() {return $q.get();}"
    )]
    public class Vector2D {
        public Vector2D() {
            CPP.Add("$q = std::make_shared<QVector2D>();");
        }
        public Vector2D(float x, float y) {
            CPP.Add("$q = std::make_shared<QVector2D>(x,y);");
        }
        public float GetX() {return CPP.ReturnFloat("$q->x()");}
        public float GetY() {return CPP.ReturnFloat("$q->y()");}
        public void Set(float x, float y) {
            CPP.Add("$q->setX(x);");
            CPP.Add("$q->setY(y);");
        }
        public float Length() {
            return CPP.ReturnFloat("$q->length()");
        }
        public void Normalize() {
            CPP.Add("$q->normalize();");
        }
        public void Add(Vector2D other) {
            CPP.Add("(*$q.get())+=(*($check(other)->$value()));");
        }
        public void Sub(Vector2D other) {
            CPP.Add("(*$q.get())-=(*($check(other)->$value()));");
        }
        public void Multiply(float factor) {
            CPP.Add("(*$q.get())*=factor;");
        }
        public void Divide(float divsor) {
            CPP.Add("(*$q.get())/=divsor;");
        }
        public static float DotProduct(Vector2D a, Vector2D b) {
            return CPP.ReturnFloat("QVector2D::dotProduct(*($check(a)->$q), *($check(b)->$q))");
        }
    }
}
