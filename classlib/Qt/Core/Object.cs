using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "$field *$refType;"
    )]
    public class Field {
        [CPPReplaceArgs("$field *$ref")]
        private Field(NativeArg1 arg) {
            CPP.Add("$refType = $ref;");
        }
        public String GetName() {
            return CPP.ReturnString("new Qt::Core::String($refType->name)");
        }
    }
    [CPPClass(
        "$method *$refType;"
    )]
    public class Method {
        [CPPReplaceArgs("$method *$ref")]
        private Method(NativeArg1 arg) {
            CPP.Add("$refType = $ref;");
        }
        public String GetName() {
            return CPP.ReturnString("new Qt::Core::String($refType->name)");
        }
    }
    [CPPClass(
        "$class *$refType;"
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
            return CPP.ReturnString("new Qt::Core::String($refType->name)");
        }
        public Type GetBaseType() {
            CPP.Add("if ($refType->base == nullptr) return nullptr;");
            return (Type)CPP.ReturnObject("new Qt::Core::Type($refType->base)");
        }
        public Field[] GetFields() {
            CPP.Add("int cnt = $refType->fields->size();");
            CPP.Add("Qt::QSharp::FixedArray<Field*>* fields = new Qt::QSharp::FixedArray<Field*>(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {fields->at(idx) = new Qt::Core::Field($refType->fields->at(idx));}");
            return (Field[])CPP.ReturnObject("fields");
        }
        public Method[] GetMethods() {
            CPP.Add("int cnt = $refType->methods->size();");
            CPP.Add("Qt::QSharp::FixedArray<Method*>* methods = new Qt::QSharp::FixedArray<Method*>(cnt);");
            CPP.Add("for(int idx=0;idx<cnt;idx++) {methods->at(idx) = new Qt::Core::Method($refType->methods->at(idx));}");
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
//        public static Type Convert(System.Object sysType) {
//            return (Type)CPP.ReturnObject("sysType");
//        }
        /** Returns new instance of Type if class accepts zero arguments (arguments not supported yet) */
        public Object NewInstance() {
            return (Object)CPP.ReturnObject("$refType->newInstance()");
        }
    }
    [CPPClass(
        "#ifdef QSHARP_GC\r\n" +
        "void* operator new(size_t size) {return GC_MALLOC(size);}\r\n" +
        "void operator delete(void*ptr) {GC_FREE(ptr);}\r\n" +
        "#else\r\n" +
        "Object *prev = nullptr, *next = nullptr;\r\n" +
        "#endif"
    )]
    public class Object {
        public override string ToString() {return CPP.ReturnString("new Qt::Core::String($getType()->name)");}
        public override bool Equals(object obj) {return this == obj;}
        public bool ReferenceEquals(object obj) {return this == obj;}
        public override int GetHashCode() {return CPP.ReturnInt("$hash(this)");}
        public new Type GetType() {return (Type)CPP.ReturnObject("new Qt::Core::Type($getType())");}
        public static void DebugBreak() {
            CPP.Add("#ifdef _MSC_VER\r\n__debugbreak();\r\n#endif\r\n");
            CPP.Add("#ifdef __GNUC__\r\n__asm(\"int $3\");\r\n#endif\r\n");
        }
        public static void Delete(System.Object obj) {
            CPP.Add("delete obj;");
        }
        public void Detach() {
            CPP.Add("#ifndef QSHARP_GC\r\nstd::detach_object(this);\r\n#endif");
        }
        public Object() {
            CPP.Add("#ifndef QSHARP_GC\r\nstd::attach_object(this);\r\n#endif");
        }
        ~Object() {
            CPP.Add("#ifndef QSHARP_GC\r\nstd::detach_object(this);\r\n#endif");
        }
    }
    /* Creates a new auto release memory pool for method. See Object.Detach() to remove object from memory pool. */
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class AutoMemoryPool : System.Attribute {
        public AutoMemoryPool() {}
    }
}
