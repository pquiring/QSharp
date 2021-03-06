/*
These attributes and classes inject C++ source into generated source code.
 */

using System;

namespace Qt.QSharp {

    /* Injects C++ code into the class definition. */
    [AttributeUsage(AttributeTargets.Class)]
    public class CPPClass : System.Attribute {
        public CPPClass(string src) {}
    }

    /* Allows C# enum to be converted to Qt C++ enum. */
    [AttributeUsage(AttributeTargets.Enum)]
    public class CPPEnum : System.Attribute {
        public CPPEnum(string src) {}
        public CPPEnum(string[] src) {}
    }

    /* Places #if QT_VERSION >= version directive around method. */
    [AttributeUsage(AttributeTargets.Method)]
    public class CPPVersion : System.Attribute {
        public CPPVersion(string version) {}
    }

    /* Adds base classes to a class (allows multiple inheritance) */
    [AttributeUsage(AttributeTargets.Class)]
    public class CPPExtends : System.Attribute {
        public CPPExtends(string s1) {}
        public CPPExtends(string[] ss) {}
    }

    /* Add forward declaration. */
    [AttributeUsage(AttributeTargets.Class)]
    public class CPPForward : System.Attribute {
        public CPPForward(string s1) {}
    }

    /* Passes values to real C++ constructor. */
    [AttributeUsage(AttributeTargets.Class)]
    public class CPPConstructorArgs : System.Attribute {
        public CPPConstructorArgs(string s1) {}
    }

    /* Adds source outside of the class. */
    [AttributeUsage(AttributeTargets.Class)]
    public class CPPNonClassCPP : System.Attribute {
        public CPPNonClassCPP(string s1) {}
    }

    /* Adds header outside of the class. */
    [AttributeUsage(AttributeTargets.Class)]
    public class CPPNonClassHPP : System.Attribute {
        public CPPNonClassHPP(string s1) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPOmitClass : System.Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPOmitExtends : System.Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPOmitFields : System.Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPOmitMethods : System.Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPOmitConstructors : System.Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPOmitBodies : System.Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPAddUsage : System.Attribute {
        public CPPAddUsage(string s1) {}
        public CPPAddUsage(string[] s1) {}
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CPPFirst : System.Attribute {}

    [AttributeUsage(AttributeTargets.Field)]
    public class CPPOmitField : System.Attribute {}

    /** Replaces args for methods (for native types).  Also changes method to public function. */
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public class CPPReplaceArgs : System.Attribute {
        public CPPReplaceArgs(string s1) {}
    }

    /* Passes values to real base constructor. */
    [AttributeUsage(AttributeTargets.Constructor)]
    public class CPPBaseConstructorArgs : System.Attribute {
        public CPPBaseConstructorArgs(string s1) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class CPPOmitMethod : System.Attribute {}

    [AttributeUsage(AttributeTargets.Method)]
    public class CPPOmitBody : System.Attribute {}

    [AttributeUsage(AttributeTargets.Constructor)]
    public class CPPOmitConstructor : System.Attribute {}

    public class CPP {
        //these special functions will inject code directly into source
        public static void Add(string s) {}
        public static bool ReturnBool(string s) {return false;}
        public static char ReturnChar(string s) {return ' ';}
        public static byte ReturnByte(string s) {return 0;}
        public static sbyte ReturnSByte(string s) {return 0;}
        public static short ReturnShort(string s) {return 0;}
        public static int ReturnInt(string s) {return 0;}
        public static uint ReturnUInt(string s) {return 0;}
        public static long ReturnLong(string s) {return 0;}
        public static ulong ReturnULong(string s) {return 0;}
        public static float ReturnFloat(string s) {return 0;}
        public static double ReturnDouble(string s) {return 0;}
        public static object ReturnObject(string s) {return null;}
        public static string ReturnString(string s) {return null;}
        public static byte[] ReturnByteArray(string s) {return null;}
        public static char[] ReturnCharArray(string s) {return null;}
    }
}
