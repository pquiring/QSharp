using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: $class *$refType;"
    )]
    public class Type {
        [CPPReplaceArgs("$class *$ref")]
        private Type(NativeArg1 arg) {
            CPP.Add("$refType = $ref;");
        }
        public String GetName() {
            return CPP.ReturnString("String::$new($refType->name)");
        }
        public bool Equals(Type type) {
            return CPP.ReturnBool("$refType->is(type->$refType)");
        }
    }
    public class Object {
        public override string ToString() {return "Object";}
        public override bool Equals(object obj) {return this == obj;}
        public bool ReferenceEquals(object obj) {return this == obj;}
        public override int GetHashCode() {return CPP.ReturnInt("$hash(this)");}
        public new Type GetType() {return (Type)CPP.ReturnObject("Type::$new($type)");}
        ~Object() {}
    }
}
