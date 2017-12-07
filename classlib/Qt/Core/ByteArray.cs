using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::unique_ptr<QByteArray> $q;" +
        "public: QByteArray* $value() {return $q.get();}"
    )]
    public class ByteArray {
        public ByteArray() {
            CPP.Add("$q = std::make_unique<QByteArray>();");
        }
        public ByteArray(byte[] array) {
            CPP.Add("$q = std::make_unique<QByteArray>((const char*)array->data(), array->$get_Length());");
        }
        public ByteArray(String str) {
            CPP.Add("$q = std::make_unique<QByteArray>(str->cstring(), str->$get_Length());");
        }
        [CPPReplaceArgs("QByteArray array")]
        private ByteArray(NativeArg1 arg) {
            CPP.Add("$q = std::make_unique<QByteArray>(array);");
        }
        public void Append(byte[] buf, int offset, int length) {
            CPP.Add("$checkArray(buf, offset, length);");
            CPP.Add("$q->append((const char*)(buf->data() + offset), length);");
        }
        public void Insert(int index, byte[] buf, int offset, int length) {
            CPP.Add("$checkArray(buf, offset, length);");
            CPP.Add("$q->insert(index, (const char*)(buf->data() + offset), length);");
        }
        public void Remove(int pos, int length) {
            CPP.Add("$q->remove(pos, length);");
        }
        public byte[] ToArray() {
            return (byte[])CPP.ReturnObject("std::make_shared<QSharpArray<uint8>>($q->data(), $q->size(), true)");
        }
        public void Clear() {
            CPP.Add("$q->clear();");
        }
        public void SetSize(int size) {
            CPP.Add("$q->resize(size);");
        }
        public ByteArray ToBase64() {
            return (ByteArray)CPP.ReturnObject("ByteArray::$new($q->toBase64())");
        }
        public static ByteArray FromBase64(ByteArray base64) {
            return (ByteArray)CPP.ReturnObject("ByteArray::$new(QByteArray::fromBase64(*$deref(base64)->$q))");
        }
        public new String ToString() {
            return new String(this);
        }
    }
}
