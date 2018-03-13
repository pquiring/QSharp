using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QFile> $q;"
    )]
    public class File : IOStream {
        public File(String filename) {
            CPP.Add("$q = std::make_shared<QFile>($check(filename)->qstring());");
            CPP.Add("$base($q);");
        }
        public bool Exists() {return CPP.ReturnBool("$q->exists()");}
        public bool Remove() {return CPP.ReturnBool("$q->remove()");}
        public bool Rename(String newName) {return CPP.ReturnBool("$q->rename($check(newName)->qstring())");}
        public bool Resize(long newSize) {return CPP.ReturnBool("$q->resize(newSize)");}
        public long Size() {return CPP.ReturnLong("$q->size()");}
        public bool SetSize(long newSize) {return CPP.ReturnBool("$q->resize(newSize)");}
    }
}
