using Qt.QSharp;

namespace Qt.Core {
    public class Object {
        public override string ToString() {return "Object";}
        public override bool Equals(object obj) {return this == obj;}
        public bool ReferenceEquals(object obj) {return this == obj;}
        public override int GetHashCode() {return CPP.ReturnInt("$hash(this)");}
        ~Object() {}
    }
}
