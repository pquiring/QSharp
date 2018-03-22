using Qt.QSharp;

namespace Qt.Core {
    [CPPNonClassHPP(
        "Qt::QSharp::FixedArray1D<std::shared_ptr<Qt::Core::String>> $QStringListToStringArray(QStringList list);" +
        "QStringList $StringArrayToQStringList(Qt::QSharp::FixedArray1D<std::shared_ptr<Qt::Core::String>> array);"
    )]
    [CPPClass(
        "private: std::unique_ptr<QString> $q;" +
        "public: QString* $value() {return $q.get();}" +
        "public: QByteArray cstring() {return $q->toUtf8();}" +
        "public: QString qstring() {return *$q;}" +
        "public: const char16* ustring() {return (const char16*)$q->utf16();}"
    )]
    public class String : Object {
        public String() {
            CPP.Add("$q = std::make_unique<QString>();");
        }
        [CPPReplaceArgs("const char *cs")]
        private String(NativeArg1 arg) {
            CPP.Add("$q = std::make_unique<QString>(cs);");
        }
        [CPPReplaceArgs("std::string ss")]
        private String(NativeArg2 arg) {
            CPP.Add("$q = std::make_unique<QString>(ss.c_str());");
        }
        [CPPReplaceArgs("const QString qs")]
        private String(NativeArg3 arg) {
            CPP.Add("$q = std::make_unique<QString>(qs);");
        }
        [CPPReplaceArgs("const QByteArray array")]
        private String(NativeArg4 arg) {
            CPP.Add("$q = std::make_unique<QString>(array);");
        }
        public String(char[] str, int offset, int length) {
            CPP.Add("$check(str, offset, length);");
            CPP.Add("$q = std::make_unique<QString>();");
            CPP.Add("$q->append((const QChar*)str->data()+offset,length);");
        }
        public String(char[] str) {
            CPP.Add("$check(str);");
            CPP.Add("int length = str->Length;");
            CPP.Add("$q = std::make_unique<QString>();");
            CPP.Add("$q->append((const QChar*)str->data(),length);");
        }
        public String(byte[] utf8, int offset, int length) {
            CPP.Add("$check(utf8, offset, length);");
            CPP.Add("$q = std::make_unique<QString>();");
            CPP.Add("$q->append(QByteArray((const char*)utf8->data()+offset,length));");
        }
        public String(byte[] utf8) {
            CPP.Add("$check(utf8);");
            CPP.Add("int length = utf8->Length;");
            CPP.Add("$q = std::make_unique<QString>();");
            CPP.Add("$q->append(QByteArray((const char*)utf8->data(),length));");
        }
        public String(string s) {
            CPP.Add("$q = std::make_unique<QString>(*$check(s)->$value());");
        }
        public String(ByteArray array) {
            CPP.Add("$q = std::make_unique<QString>(*$check(array)->$value());");
        }
        public static implicit operator String(string s) {
            //convert builtin string to Qt.Core.String
            return new String(s);
        }
        public static implicit operator string(String s) {
            //convert Qt.Core.String to builtin string
            return (string)s;
        }

        public int Length {get{return CPP.ReturnInt("$q->length()");} set{CPP.Add("$q->resize(value);");}}
        public String Append(String s) {
            CPP.Add("$q->append(*$check(s)->$q);");
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

        public String Insert(int pos, char ch) {
            CPP.Add("$q->insert(pos, (QChar)ch);");
            return this;
        }

        public String Remove(int pos, int count = 1) {
            CPP.Add("$q->remove(pos, count);");
            return this;
        }

        public bool Equals(String s) {
            return CPP.ReturnBool("$q->compare(*$check(s)->$q) == 0");
        }

        public bool EqualsIgnoreCase(String s) {
            return CPP.ReturnBool("$q->compare(*$check(s)->$q, Qt::CaseInsensitive) == 0");
        }

        public bool Contains(String s) {
            return CPP.ReturnBool("$q->contains(*$check(s)->$q)");
        }

        public bool ContainsIgnoreCase(String s) {
            return CPP.ReturnBool("$q->contains(*$check(s)->$q, Qt::CaseInsensitive)");
        }

        public bool ContainsEx(String s) {
            return CPP.ReturnBool("$q->contains(QRegExp(*$check(s)->$q))");
        }

        public void Replace(char before, char after) {
            CPP.Add("$q->replace((QChar)before, (QChar)after);");
        }

        public void Replace(String before, String after) {
            CPP.Add("$q->replace($check(before)->qstring(), $check(after)->qstring());");
        }

        public void ReplaceIgnoreCase(char before, char after) {
            CPP.Add("$q->replace((QChar)before, (QChar)after, Qt::CaseInsensitive);");
        }

        public void ReplaceIgnoreCase(String before, String after) {
            CPP.Add("$q->replace($check(before)->qstring(), $check(after)->qstring(), Qt::CaseInsensitive);");
        }

        public void ReplaceEx(String before, String after) {
            CPP.Add("$q->replace(QRegExp($check(before)->qstring()), $check(after)->qstring());");
        }

        public int IndexOf(char ch) {
            return CPP.ReturnInt("$q->indexOf((QChar)ch)");
        }
        public int LastIndexOf(char ch) {
            return CPP.ReturnInt("$q->lastIndexOf((QChar)ch)");
        }

        public int IndexOf(String str) {
            return CPP.ReturnInt("$q->indexOf($check(str)->qstring())");
        }
        public int LastIndexOf(String str) {
            return CPP.ReturnInt("$q->lastIndexOf($check(str)->qstring())");
        }

        public bool StartsWith(String s) {
            return CPP.ReturnBool("$q->startsWith($check(s)->qstring())");
        }
        public bool EndsWith(String s) {
            return CPP.ReturnBool("$q->endsWith($check(s)->qstring())");
        }

        public String[] Split(char ch) {
            CPP.Add("QStringList list = $q->split(ch);");
            return (String[])CPP.ReturnObject("$QStringListToStringArray(list)");
        }

        public String[] Split(String str) {
            CPP.Add("QStringList list = $q->split($check(str)->qstring());");
            return (String[])CPP.ReturnObject("$QStringListToStringArray(list)");
        }

        public String[] SplitEx(String str) {
            CPP.Add("QStringList list = $q->split(QRegExp($check(str)->qstring()));");
            return (String[])CPP.ReturnObject("$QStringListToStringArray(list)");
        }
        /**
            Substring()
                int start
                int length
         */
        public String Substring(int start, int length = -1) {
            return CPP.ReturnString("Qt::Core::String::$new($q->mid(start, length))");
        }
        /**
            Substring()
                int start
                int end
            Return : String from start to end-1
            Compatible with Java String.substring()
        */
        public String SubstringIdx(int start, int end) {
            if (start > end) return null;
            int length = end - start;
            return CPP.ReturnString("Qt::Core::String::$new($q->mid(start, length))");
        }
        public char CharAt(int idx) {
            return CPP.ReturnChar("$q->at(idx).unicode()");
        }

        public String ToUpperCase() {
            return CPP.ReturnString("Qt::Core::String::$new($q->toUpper())");
        }
        public String ToLowerCase() {
            return CPP.ReturnString("Qt::Core::String::$new($q->toLower())");
        }

        public char[] ToCharArray() {
            CPP.Add("Qt::QSharp::FixedArray1D<char16> array;");
            CPP.Add("int length = Length;");
            CPP.Add("array = Qt::QSharp::FixedArray1D<char16>::$new(length);");
            CPP.Add("char16 *dest = array->data();");
            CPP.Add("const char16 *src = ustring();");
            CPP.Add("std::memcpy(dest, src, length * 2);");
            return CPP.ReturnCharArray("array");
        }
        public byte[] ToByteArray() {
            CPP.Add("Qt::QSharp::FixedArray1D<uint8> array;");
            CPP.Add("int length = Length;");
            CPP.Add("array = Qt::QSharp::FixedArray1D<uint8>::$new(length);");
            CPP.Add("uint8 *dest = array->data();");
            CPP.Add("QByteArray cstr = cstring();");
            CPP.Add("const uint8 *src = (const uint8*)cstr.constData();");
            CPP.Add("std::memcpy(dest, src, length);");
            return CPP.ReturnByteArray("array");
        }
    }
}
