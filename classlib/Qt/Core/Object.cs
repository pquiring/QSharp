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
        [CPPOmitConstructor]
        /** Converts System.Type to Qt.Core.Type */
        public Type(System.Type type) {
            CPP.Add("$refType = $deref(type)->$refType;");
        }
        public Type(Type type) {
            CPP.Add("$refType = $deref(type)->$refType;");
        }
        public String GetName() {
            return CPP.ReturnString("String::$new($refType->name)");
        }
        public Type GetBaseType() {
            CPP.Add("if ($refType->base == nullptr) return std::shared_ptr<Type>();");
            return (Type)CPP.ReturnObject("Type::$new($refType->base)");
        }
        /** Matches exactly. */
        public bool Equals(Type type) {
            return CPP.ReturnBool("($refType == $deref(type)->$refType)");
        }
        /** Matches if same or derived from type. */
        public bool IsDerivedFrom(Type type) {
            return CPP.ReturnBool("$refType->isDerivedFrom($deref(type)->$refType)");
        }
        /** Use to convert System.Type to Qt.Core.Type for now.
        * ie: Type type = Type.Convert(typeof(MyClass));
        * This is a temporary work around.
        * or: Type type = new Type(typeof(MyClass));
        */
        public static Type Convert(System.Object sysType) {
            return (Type)CPP.ReturnObject("sysType");
        }
    }
    public class Object {
        public override string ToString() {return "Object";}
        public override bool Equals(object obj) {return this == obj;}
        public bool ReferenceEquals(object obj) {return this == obj;}
        public override int GetHashCode() {return CPP.ReturnInt("$hash(this)");}
        public new Type GetType() {return (Type)CPP.ReturnObject("Type::$new($getType())");}
        ~Object() {}
    }
}
