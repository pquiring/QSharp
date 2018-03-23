using Qt.QSharp;

namespace Qt.Core {
    [CPPClass("std::shared_ptr<QBuffer> $q;")]
    public class ByteArrayStream : IOStream {
        public ByteArrayStream() {
            CPP.Add("$q = std::make_shared<QBuffer>();");
            CPP.Add("$base((std::shared_ptr<QIODevice>)$q);");
        }
        public ByteArrayStream(ByteArray data) {
            CPP.Add("$q = std::make_shared<QBuffer>();");
            CPP.Add("$base((std::shared_ptr<QIODevice>)$q);");
            SetData(data);
        }
        public void SetData(ByteArray data) {
            CPP.Add("$q->setBuffer($check(data)->$value());");
        }
        public ByteArray GetData() {
            return (ByteArray)CPP.ReturnObject("ByteArray::$new($q->buffer())");
        }
    }
}
