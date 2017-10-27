using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: QString *$q = nullptr;\r\n" +
        "public: String() { $q = new QString(); }\r\n" +
        "public: String(const char* cstring) { $q = new QString(cstring); }\r\n" +
        "public: const char* cstring() {return $q->toUtf8().constData();}\r\n"
    )]
    [CPPNonClassCPP(
        "std::shared_ptr<Qt::Core::String> operator+(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::String> s2) {\r\n" +
        "  std::shared_ptr<Qt::Core::String> s = std::make_shared<Qt::Core::String>();\r\n" +
        "  s->$q->append(*(s1->$q));\r\n" +
        "  s->$q->append(*(s2->$q));\r\n" +
        "  return s;\r\n" +
        "}\r\n"
    )]
    [CPPNonClassHPP(
        "std::shared_ptr<Qt::Core::String> operator+(std::shared_ptr<Qt::Core::String> s1, std::shared_ptr<Qt::Core::String> s2);\r\n"
    )]
    public class String : Object {
        public String() {}
        public String(char[] str, int idx, int length) {
            CPP.Add("$q->append((const QChar*)str->data(),length);");
        }
        public String(byte[] utf8, int idx, int length) {
            CPP.Add("$q->append(QByteArray((const char*)utf8->data()+idx,length));");
        }
        public String(string s) {}
        public static implicit operator String(string s) {
            return new String(s);
        }

        public int IndexOf(char ch) {
            return CPP.ReturnInt("$q->indexOf((QChar)ch)");
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
