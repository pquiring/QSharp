using Qt.QSharp;

namespace Qt.Core {
    [CPPClass("QBuffer* $d() {return dynamic_cast<QBuffer*>($q.get());}")]
    public class ByteArrayStream : IOStream {
        public ByteArrayStream() {
            CPP.Add("$base(new QBuffer());");
        }
        public ByteArrayStream(ByteArray data) {
            CPP.Add("$base(new QBuffer());");
            SetData(data);
        }
        public void SetData(ByteArray data) {
            CPP.Add("$d()->setBuffer($check(data)->$value());");
        }
        public ByteArray GetData() {
            return (ByteArray)CPP.ReturnObject("new ByteArray($d()->buffer())");
        }
    }
}
