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
        public ByteArray(String str) {
            CPP.Add("$q->append(str->qstring());");
        }
        public void Append(byte[] buf, int offset, int length) {
            CPP.Add("$checkArray(buf, offset, length);");
            CPP.Add("$q->append((const char*)(buf->data() + offset), length);");
        }
        public byte[] ToArray() {
            CPP.Add("int length = $q->size();");
            CPP.Add("std::shared_ptr<QSharpArray<uint8>> array = std::make_shared<QSharpArray<uint8>>(length);");
            CPP.Add("std::memcpy(array->data(), $q->data(), length);");
            return (byte[])CPP.ReturnObject("array");
        }
    }
}
