using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: inline int $mod(int x, int y) {return x % y;}\r\n" +
        "public: inline float $mod(float x, float y) {return ::fmod(x, y);}\r\n" +
        "public: inline double $mod(double x, double y) {return ::fmod(x, y);}\r\n"
    )]
    public class Object {
        public override string ToString() {return "Object";}
        public override bool Equals(object obj) {return this == obj;}
        public bool ReferenceEquals(object obj) {return this == obj;}
        public override int GetHashCode() {return CPP.ReturnInt("$hash(this)");}
        ~Object() {}
    }
}
