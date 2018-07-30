using Qt.QSharp;

namespace Qt.Core {
    [CPPClass("QFile* $d() {return dynamic_cast<QFile*>($q.get());}")]
    public class File : IOStream {
        public File(String filename) {
            CPP.Add("$base(new QFile($check(filename)->qstring()));");
        }
        public bool Exists() {return CPP.ReturnBool("$d()->exists()");}
        public bool Remove() {return CPP.ReturnBool("$d()->remove()");}
        public bool Rename(String newName) {return CPP.ReturnBool("$d()->rename($check(newName)->qstring())");}
        public bool Copy(String newName) {return CPP.ReturnBool("$d()->copy($check(newName)->qstring())");}
        public bool Resize(long newSize) {return CPP.ReturnBool("$d()->resize(newSize)");}
        public long Size() {return CPP.ReturnLong("$d()->size()");}
        public bool SetSize(long newSize) {return CPP.ReturnBool("$d()->resize(newSize)");}
        public DateTime GetLastModified() {
            return (DateTime)CPP.ReturnObject("Qt::Core::DateTime::$new(QFileInfo(*($d())).lastModified())");
        }
        public DateTime GetLastRead() {
            return (DateTime)CPP.ReturnObject("Qt::Core::DateTime::$new(QFileInfo(*($d())).lastRead())");
        }
        public String AbsolutePath() {
            return CPP.ReturnString("Qt::Core::String::$new(QFileInfo(*($d())).absoluteFilePath())");
        }
    }
}
