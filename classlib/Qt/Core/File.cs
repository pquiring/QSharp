using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QFile *$q;\r\n"
    )]
    public class File : IODevice {
        public File(String filename) {
            CPP.Add("$q = new QFile(filename->qstring());\r\n");
            CPP.Add("$base($q);\r\n");
        }
        public bool Exists() {return CPP.ReturnBool("exists()");}
        public bool Remove() {return CPP.ReturnBool("remove()");}
        public bool Rename(String newName) {return CPP.ReturnBool("rename(newName->qstring())");}
        public bool Resize(long newSize) {return CPP.ReturnBool("resize(newSize)");}
        public long Size() {return CPP.ReturnLong("size()");}
        ~File() {
            CPP.Add("delete $q;\r\n");
        }
    }
}
