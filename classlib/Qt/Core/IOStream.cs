using Qt.QSharp;

namespace Qt.Core {
    [CPPEnum("QIODevice::OpenMode")]
    public enum OpenMode {NotOpen, ReadOnly, WriteOnly, ReadWrite, Append, Truncate, Text, Unbuffered}
    [CPPClass(
        "std::qt_ptr<QIODevice> $q;" +
        "void $base(QIODevice *io) {$q = io;}" +
        "QIODevice* $value() {return $q.get();} "
    )]
    public class IOStream {
        public bool Open(OpenMode mode) {return CPP.ReturnBool("$q->open((QIODevice::OpenMode)mode)");}
        public void Close() {CPP.Add("$q->close();");}
        public int Read(byte[] data, int offset = 0, int length = -1) {
            if (length == -1) length = data.Length;
            CPP.Add("$check(data, offset, length);");
            return CPP.ReturnInt("$q->read((char*)data->data() + offset, length)");
        }
        public int Write(byte[] data, int offset = 0, int length = -1) {
            if (length == -1) length = data.Length;
            CPP.Add("$check(data, offset, length);");
            return CPP.ReturnInt("$q->write((char*)data->data() + offset, length)");
        }
        public bool SetPosition(long pos) {
            return CPP.ReturnBool("$q->seek(pos)");
        }
        public long GetPosition() {
            return CPP.ReturnLong("$q->pos()");
        }
        public ByteArray ReadAll() {
            return (ByteArray)CPP.ReturnObject("ByteArray::$new($q->readAll())");
        }
        public bool ReadAll(byte[] buf, int pos = 0, int length = -1) {
            if (length == -1) length = buf.Length - pos;
            while (length > 0) {
                int read = Read(buf, pos, length);
                if (read < 0) return false;
                pos += read;
                length -= read;
            }
            return true;
        }
        public int GetAvailable() {
            return CPP.ReturnInt("$q->bytesAvailable()");
        }
    }
}
