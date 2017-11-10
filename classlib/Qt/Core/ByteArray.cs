using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QByteArray> $q;" +
        "public: void $base(std::shared_ptr<QByteArray> ba) {$q = ba;}" +
        "public: void $base(QByteArray ba) {*$q = ba;}" +
        "public: ByteArray() {$q = std::make_shared<QByteArray>();}"
    )]
    public class ByteArray {
        public void Append(byte[] buf, int offset, int length) {
            CPP.Add("$checkArray(buf, offset, length);");
            CPP.Add("$q->append((const char*)(buf->data() + offset), length);");
        }
        public byte[] ToArray() {
            CPP.Add("std::shared_ptr<QVector<uint8>> array = std::make_shared<QVector<uint8>>();");
            CPP.Add("int length = $q->size();");
            CPP.Add("array->resize(length);");
            CPP.Add("std::memcpy(array->data(), $q->data(), length);");
            return (byte[])CPP.ReturnObject("array");
        }
    }
}
