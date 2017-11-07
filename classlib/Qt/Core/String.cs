using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: QString *$q = nullptr;\r\n" +
        "public: String() { $q = new QString(); }\r\n" +
        "public: String(const char* cstring) { $q = new QString(cstring); }\r\n" +
        "public: String(std::string ss) { $q = new QString(ss.c_str()); }\r\n" +
        "public: const char* cstring() {return $q->toUtf8().constData();}\r\n" +
        "public: QString qstring() {return *$q;}\r\n"
    )]
    public class String : Object {
        public String() {}
        public String(char[] str, int idx, int length) {
            CPP.Add("$q->append((const QChar*)$deref(str)->data(),length);");
        }
        public String(byte[] utf8, int idx, int length) {
            CPP.Add("$q->append(QByteArray((const char*)$deref(utf8)->data()+idx,length));");
        }
        public String(string s) {
            Append(s);
        }
        public static implicit operator String(string s) {
            //convert builtin string to Qt.Core.String
            return new String(s);
        }
        public static implicit operator string(String s) {
            //convert Qt.Core.String to builtin string
            return (string)s;
        }

        public int Length {get{return CPP.ReturnInt("$q->length()");}}
        public String Append(String s) {
            CPP.Add("$q->append(*$deref(s)->$q);");
            return this;
        }
        public String Append(char ch) {
            CPP.Add("$q->append((QChar)ch);");
            return this;
        }
        public String Append(int v) {
            String s = Int32.ToString(v);
            Append(s);
            return this;
        }
        public String Append(uint v) {
            String s = UInt32.ToString(v);
            Append(s);
            return this;
        }
        public String Append(long v) {
            String s = Int64.ToString(v);
            Append(s);
            return this;
        }
        public String Append(ulong v) {
            String s = UInt64.ToString(v);
            Append(s);
            return this;
        }
        public String Append(float v) {
            String s = Float.ToString(v);
            Append(s);
            return this;
        }
        public String Append(double v) {
            String s = Double.ToString(v);
            Append(s);
            return this;
        }

        public bool Equals(String s) {
            return CPP.ReturnBool("$q->compare(*$deref(s)->$q) == 0");
        }

        public bool EqualsIgnoreCase(String s) {
            return CPP.ReturnBool("$q->compare(*$deref(s)->$q, Qt::CaseInsensitive) == 0");
        }

        public int IndexOf(char ch) {
            return CPP.ReturnInt("$q->indexOf((QChar)ch)");
        }
        public int LastIndexOf(char ch) {
            return CPP.ReturnInt("$q->lastIndexOf((QChar)ch)");
        }

        public char[] ToCharArray() {
            CPP.Add("//TODO\r\n");
            return CPP.ReturnCharArray("std::shared_ptr<QVector<char16>>()");
        }
        public byte[] ToByteArray() {
            CPP.Add("//TODO\r\n");
            return CPP.ReturnByteArray("std::shared_ptr<QVector<uint8>>()");
        }

        ~String() {
            CPP.Add("delete $q;");
        }
    }
}
