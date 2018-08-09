using Qt.QSharp;

namespace Qt.Core {
    [CPPEnum("QuaZip::Mode")]
    public enum ZipMode { NotOpen, Unzip, Create, Append, Add }
    [CPPClass("QuaZipFile* $d() {return dynamic_cast<QuaZipFile*>($q.get());}")]
    public class ZipEntry : IOStream {
        public String Filename {get;}
        public String GetFilename() {
            return Filename;
        }
        public ZipEntry(String filename) {
            this.Filename = filename;
        }
    }
    [CPPClass(
        "std::qt_ptr<QuaZip> $q;"
    )]
    public class ZipFile {
        protected String filename;
        public ZipFile(String filename) {
            CPP.Add("$q = new QuaZip($check(filename)->qstring());");
        }
        public bool Open(ZipMode mode) {
            return CPP.ReturnBool("$q->open((QuaZip::Mode)mode)");
        }
        public bool NextEntry() {
            bool next = CPP.ReturnBool("$q->goToNextFile()");
            if (next) {
                CPP.Add("filename = new Qt::Core::String($q->getCurrentFileName());");
            } else {
                filename = null;
            }
            return next;
        }
        public bool FirstEntry() {
            bool first = CPP.ReturnBool("$q->goToFirstFile()");
            if (first) {
                CPP.Add("filename = new Qt::Core::String($q->getCurrentFileName());");
            } else {
                filename = null;
            }
            return first;
        }
        public ZipEntry GetEntry() {
            CPP.Add("ZipEntry* ze = new ZipEntry(filename);");
            CPP.Add("ze->$base(new QuaZipFile($q.get()));");
            return (ZipEntry)CPP.ReturnObject("ze");
        }
        public ZipEntry GetEntry(String filename) {
            IEnumerator<ZipEntry> zipEnum = GetEnumerator();
            ZipEntry zipEntry;
            while(zipEnum.MoveNext()) {
                zipEntry = zipEnum.Current;
                if (zipEntry.Filename == filename) {
                    return zipEntry;
                }
            }
            return null;
        }
        //TODO : AddEntry()
        public IEnumerator<ZipEntry> GetEnumerator() {
            return new ZipEnumerator(this);
        }
    }
    public class ZipEnumerator : IEnumerator<ZipEntry> {
        private ZipFile zip;
        public ZipEnumerator(ZipFile zf) {
            zip = zf;
            zf.FirstEntry();
            zipEntry = zf.GetEntry();
        }
        private ZipEntry zipEntry;
        public ZipEntry /*IEnumerator<ZipEntry>.*/Current {get {return zipEntry;}}
        public bool MoveNext() {
            bool next = zip.NextEntry();
            if (next) {
                zipEntry = zip.GetEntry();
            } else {
                zipEntry = null;
            }
            return next;
        }
        public void Reset() {}
    }
    [CPPClass("QuaZIODevice* $d() {return dynamic_cast<QuaZIODevice*>($q.get());}")]
    public class Compress : IOStream {
        public Compress(ByteArrayStream input) {
            CPP.Add("$base(new QuaZIODevice($check(input)->$value()));");
        }
    }
    [CPPClass("QuaZIODevice* $d() {return dynamic_cast<QuaZIODevice*>($q.get());}")]
    public class Decompress : IOStream {
        public Decompress(ByteArrayStream input) {
            CPP.Add("$base(new QuaZIODevice($check(input)->$value()));");
        }
    }
}
