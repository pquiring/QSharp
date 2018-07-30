using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QByteArray> $q;" +
        "QByteArray* $value() {return $q.get();}"
    )]
    public class ByteArray {
        public ByteArray() {
            CPP.Add("$q = new QByteArray();");
        }
        public ByteArray(byte[] array) {
            CPP.Add("$check(array);");
            CPP.Add("int length = array->Length;");
            CPP.Add("$q = new QByteArray((const char*)array->data(), length);");
        }
        public ByteArray(byte[] array, int offset, int length) {
            CPP.Add("$check(array, offset, length);");
            CPP.Add("$q = new QByteArray((const char*)array->data() + offset, length);");
        }
        public ByteArray(String str) {
            CPP.Add("$q = new QByteArray($check(str)->cstring());");
        }
        [CPPReplaceArgs("QByteArray array")]
        private ByteArray(NativeArg1 arg) {
            CPP.Add("$q = new QByteArray(array);");
        }
        public void Append(byte[] buf, int offset, int length) {
            CPP.Add("$check(buf, offset, length);");
            CPP.Add("$q->append((const char*)(buf->data() + offset), length);");
        }
        public void Insert(int index, byte[] buf, int offset, int length) {
            CPP.Add("$check(buf, offset, length);");
            CPP.Add("$q->insert(index, (const char*)(buf->data() + offset), length);");
        }
        public void Remove(int pos, int length) {
            CPP.Add("$q->remove(pos, length);");
        }
        public byte[] ToArray() {
            return (byte[])CPP.ReturnObject("Qt::QSharp::FixedArray1D<uint8>::$new((uint8*)$q->data(), $q->size(), true)");
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
            return (ByteArray)CPP.ReturnObject("ByteArray::$new(QByteArray::fromBase64(*$check(base64)->$q))");
        }
        public new String ToString() {
            return new String(this);
        }
    }
}
