using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: $field *$refType;"
    )]
    public class Field {
        [CPPReplaceArgs("$field *$ref")]
        private Field(NativeArg1 arg) {
            CPP.Add("$refType = $ref;");
        }
        public String GetName() {
            return CPP.ReturnString("Qt::Core::String::$new($refType->name)");
        }
    }
    [CPPClass(
        "public: $method *$refType;"
    )]
    public class Method {
        [CPPReplaceArgs("$method *$ref")]
        private Method(NativeArg1 arg) {
            CPP.Add("$refType = $ref;");
        }
        public String GetName() {
            return CPP.ReturnString("Qt::Core::String::$new($refType->name)");
        }
    }
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
            CPP.Add("$refType = $check(type)->$refType;");
        }
        public Type(Type type) {
            CPP.Add("$refType = $check(type)->$refType;");
        }
        public static implicit operator Type(System.Type type) {
            //convert System.Type to Qt.Core.Type
            return new Type(type);
        }
        public String GetName() {
            return CPP.ReturnString("Qt::Core::String::$new($refType->name)");
        }
        public Type GetBaseType() {
            CPP.Add("if ($refType->base == nullptr) return std::shared_ptr<Qt::Core::Type>();");
            return (Type)CPP.ReturnObject("Qt::Core::Type::$new($refType->base)");
        }
        public Field[] GetFields() {
            CPP.Add("int cnt = $refType->fields->size();");
            CPP.Add("Qt::QSharp::FixedArray1D<std::shared_ptr<Field>> fields = Qt::QSharp::FixedArray1D<std::shared_ptr<Field>>::$new(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {fields->at(idx) = Qt::Core::Field::$new($refType->fields->at(idx));}");
            return (Field[])CPP.ReturnObject("fields");
        }
        public Method[] GetMethods() {
            CPP.Add("int cnt = $refType->methods->size();");
            CPP.Add("Qt::QSharp::FixedArray1D<std::shared_ptr<Method>> methods = Qt::QSharp::FixedArray1D<std::shared_ptr<Method>>::$new(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {methods->at(idx) = Qt::Core::Method::$new($refType->methods->at(idx));}");
            return (Method[])CPP.ReturnObject("methods");
        }
        /** Matches exactly. */
        public bool Equals(Type type) {
            return CPP.ReturnBool("($refType == $check(type)->$refType)");
        }
        /** Matches if same or derived from type. */
        public bool IsDerivedFrom(Type type) {
            return CPP.ReturnBool("$refType->isDerivedFrom($check(type)->$refType)");
        }
        /** Returns if type represents an interface */
        public bool IsInterface() {
            return CPP.ReturnBool("$refType->iface");
        }
        /** Use to convert System.Type to Qt.Core.Type for now.
        * ie: Type type = Type.Convert(typeof(MyClass));
        * This is a temporary work around.
        * or: Type type = new Type(typeof(MyClass));
        */
        public static Type Convert(System.Object sysType) {
            return (Type)CPP.ReturnObject("sysType");
        }
        /** Returns new instance of Type if class accepts zero arguments (arguments not supported yet) */
        public Object NewInstance() {
            return (Object)CPP.ReturnObject("$refType->newInstance()");
        }
    }
    [CPPClass(
        "public: std::weak_ptr<Qt::Core::Object> $weak_this;"
    )]
    public class Object {
        public override string ToString() {return CPP.ReturnString("Qt::Core::String::$new($getType()->name)");}
        public override bool Equals(object obj) {return this == obj;}
        public bool ReferenceEquals(object obj) {return this == obj;}
        public override int GetHashCode() {return CPP.ReturnInt("$hash(this)");}
        public new Type GetType() {return (Type)CPP.ReturnObject("Qt::Core::Type::$new($getType())");}
        ~Object() {}
    }
}
