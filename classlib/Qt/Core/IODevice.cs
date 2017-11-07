using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QIODevice *$q;\r\n" +
        "public $base(QIODevice *io) {$q = io;}\r\n"
    )]
    public class IODevice {
        public IODevice() {
            CPP.Add("$q = nullptr;");
        }
        public enum Mode {NotOpen, ReadOnly, WriteOnly, ReadWrite, Append, Truncate, Text, Unbuffered}
        public bool Open(Mode mode) {return CPP.ReturnBool("$q->open(mode)");}
        public void Close() {CPP.Add("$q->close();\r\n");}
        public int Read(byte[] data, int offset, int length) {return CPP.ReturnInt("$checkArray(data, offset, length); $q->read(data->data() + offset, length");}
        public int Write(byte[] data, int offset, int length) {return CPP.ReturnInt("$checkArray(data, offset, length); $q->write(data->data() + offset, length)");}
    }
}
