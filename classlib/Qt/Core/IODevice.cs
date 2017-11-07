using Qt.QSharp;

namespace Qt.Core {
    public enum OpenMode {NotOpen, ReadOnly, WriteOnly, ReadWrite, Append, Truncate, Text, Unbuffered}
    [CPPClass(
        "private: QIODevice *$q;\r\n" +
        "public: void $base(QIODevice *io) {$q = io;}\r\n"
    )]
    public class IODevice {
        public IODevice() {
            CPP.Add("$q = nullptr;");
        }
        public bool Open(OpenMode mode) {return CPP.ReturnBool("$q->open((QIODevice::OpenMode)mode)");}
        public void Close() {CPP.Add("$q->close();\r\n");}
        public int Read(byte[] data, int offset, int length) {CPP.Add("$checkArray(data, offset, length);"); return CPP.ReturnInt("$q->read((char*)data->data() + offset, length)");}
        public int Write(byte[] data, int offset, int length) {CPP.Add("$checkArray(data, offset, length);"); return CPP.ReturnInt("$q->write((char*)data->data() + offset, length)");}
    }
}
