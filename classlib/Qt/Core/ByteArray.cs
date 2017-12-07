using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: std::shared_ptr<QByteArray> $q;" +
        "public: void $base(std::shared_ptr<QByteArray> ba) {$q = ba;}" +
        "public: void $base(QByteArray ba) {*$q = ba;}"
    )]
    public class ByteArray {
        public ByteArray() {
            CPP.Add("$q = std::make_shared<QByteArray>();");
        }
        public ByteArray(byte[] array) {
            CPP.Add("$q = std::make_shared<QByteArray>((const char*)array->data(), array->$get_Length());");
        }
        public ByteArray(String str) {
            CPP.Add("$q = std::make_shared<QByteArray>(str->cstring(), str->$get_Length());");
        }
        [CPPReplaceArgs("QByteArray array")]
        private ByteArray(NativeArg1 arg) {
            CPP.Add("$q = std::make_shared<QByteArray>(array);");
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
            CPP.Add("int length = $q->size();");
            CPP.Add("std::shared_ptr<QSharpArray<uint8>> array = std::make_shared<QSharpArray<uint8>>(length);");
            CPP.Add("std::memcpy(array->data(), $q->data(), length);");
            return (byte[])CPP.ReturnObject("array");
        }
        public void Clear() {
            CPP.Add("$q->clear();");
        }
        public void SetSize(int size) {
            CPP.Add("$q->resize(size);");
        }
        public ByteArray ToBase64() {
            CPP.Add("std::shared_ptr<ByteArray> array = ByteArray::$new();");
            CPP.Add("array->$base($q->toBase64());");
            return (ByteArray)CPP.ReturnObject("array");
        }
        public static ByteArray FromBase64(ByteArray base64) {
            CPP.Add("std::shared_ptr<ByteArray> array = ByteArray::$new();");
            CPP.Add("array->$base(QByteArray::fromBase64(*$deref(base64)->$q));");
            return (ByteArray)CPP.ReturnObject("array");
        }
        public new String ToString() {
            return new String(this);
        }
    }
}
