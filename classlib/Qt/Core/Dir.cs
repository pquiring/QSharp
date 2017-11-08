using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: QDir *$q;\r\n"
    )]
    public class Dir {
        public Dir(String folder) {
            CPP.Add("$q = new QDir(folder->qstring());\r\n");
        }
        public bool Exists() {return CPP.ReturnBool("$q->exists()");}
        public bool Exists(String name) {return CPP.ReturnBool("$q->exists(name->qstring())");}
        public bool Remove(String filename) {return CPP.ReturnBool("$q->remove(filename->qstring())");}
        public bool Rename(String oldName, String newName) {return CPP.ReturnBool("$q->rename(oldName->qstring(), newName->qstring())");}
        ~Dir() {
            CPP.Add("delete $q;\r\n");
        }
    }
}
