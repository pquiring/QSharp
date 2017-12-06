using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "public: std::shared_ptr<QByteArray> $q;" +
        "public: void $base(std::shared_ptr<QByteArray> ba) {$q = ba;}" +
        "public: void $base(QByteArray ba) {*$q = ba;}" +
        "public: ByteArray() {$q = std::make_shared<QByteArray>();}" +
        "public: ByteArray(QByteArray ba) {$q = std::make_shared<QByteArray>(); $base(ba);}"
    )]
    public class ByteArray {
        public ByteArray() {}
        public ByteArray(byte[] array) {
            CPP.Add("$q->append((const char*)array->data(), array->$get_Length());");
        }
        public ByteArray(String str) {
            CPP.Add("$q->append(str->qstring());");
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
            return (ByteArray)CPP.ReturnObject("std::make_shared<ByteArray>($q->toBase64())");
        }
        public static ByteArray FromBase64(ByteArray base64) {
            return (ByteArray)CPP.ReturnObject("std::make_shared<ByteArray>(QByteArray::fromBase64(*$deref(base64)->$q))");
        }
        public new String ToString() {
            return new String(this);
        }
    }
}
