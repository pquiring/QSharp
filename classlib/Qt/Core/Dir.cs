using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QDir> $q;"
    )]
    public class Dir {
        public Dir(String folder) {
            CPP.Add("$q = new QDir(folder->qstring());");
        }
        public String AbsolutePath() {return CPP.ReturnString("new Qt::Core::String($q->absolutePath())");}
        public bool Exists() {return CPP.ReturnBool("$q->exists()");}
        public bool Remove() {return CPP.ReturnBool("$q->rmdir(AbsolutePath()->qstring())");}
        public bool Rename(String newName) {return CPP.ReturnBool("$q->rename(AbsolutePath()->qstring(), $check(newName)->qstring())");}
        public bool Create() {return CPP.ReturnBool("$q->mkdir(AbsolutePath()->qstring())");}
        public bool CreateAll() {return CPP.ReturnBool("$q->mkpath(AbsolutePath()->qstring())");}
        public String[] List() {return (String[])CPP.ReturnObject("$QStringListToStringArray($q->entryList())");}
    }
}
