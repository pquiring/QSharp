using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QByteArray> $q;\r\n" +
        "public: void $base(std::shared_ptr<QByteArray> ba) {$q = ba;}\r\n" +
        "public: void $base(QByteArray ba) {*$q = ba;}\r\n" +
        "public: ByteArray() {$q = std::make_shared<QByteArray>();}\r\n"
    )]
    public class ByteArray {
        public void Append(byte[] buf, int offset, int length) {
            CPP.Add("$checkArray(buf, offset, length);\r\n");
            CPP.Add("$q->append((const char*)(buf->data() + offset), length);\r\n");
        }
        public byte[] ToArray() {
            CPP.Add("std::shared_ptr<QVector<uint8>> array = std::make_shared<QVector<uint8>>();\r\n");
            CPP.Add("int length = $q->size();\r\n");
            CPP.Add("array->resize(length);\r\n");
            CPP.Add("std::memcpy(array->data(), $q->data(), length);\r\n");
            return (byte[])CPP.ReturnObject("array");
        }
    }
}
