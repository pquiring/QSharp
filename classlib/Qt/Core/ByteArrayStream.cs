using Qt.QSharp;

namespace Qt.Core {
    [CPPExtends("QBuffer")]
    public class ByteArrayStream : IOStream {
        public ByteArrayStream() {
            CPP.Add("$base((std::shared_ptr<QIODevice>)$this);");
        }
        public void SetData(ByteArray data) {
            CPP.Add("setBuffer(data->$q.get());");
        }
        public ByteArray GetData() {
            return (ByteArray)CPP.ReturnObject("std::make_shared<ByteArray>(buffer())");
        }
    }
}
