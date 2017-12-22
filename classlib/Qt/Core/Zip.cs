using Qt.QSharp;

namespace Qt.Core {
    public enum ZipMode { mdNotOpen, mdUnzip, mdCreate, mdAppend, mdAdd }
    [CPPClass(
        "private: std::shared_ptr<QuaZipFile> $q;" +
        "public: void $base(std::shared_ptr<QuaZipFile> zf) {$q = zf; IOStream::$base((std::shared_ptr<QIODevice>)zf);}"
    )]
    public class ZipEntry : IOStream {
        private String _filename;
        public String filename {get {return _filename;}}
        public ZipEntry(String filename) {
            _filename = filename;
        }
    }
    [CPPClass(
        "private: std::shared_ptr<QuaZip> $q;"
    )]
    public class ZipFile {
        protected String filename;
        public ZipFile(String filename, OpenMode mode) {
            CPP.Add("$q = std::make_shared<QuaZip>($check(filename)->qstring());");
        }
        public bool Open(ZipMode mode) {
            return CPP.ReturnBool("$q->open((QuaZip::Mode)mode)");
        }
        public bool NextEntry() {
            bool next = CPP.ReturnBool("$q->goToNextFile()");
            if (next) {
                CPP.Add("filename = String::$new($q->getCurrentFileName());");
            } else {
                filename = null;
            }
            return next;
        }
        public ZipEntry GetEntry() {
            CPP.Add("std::shared_ptr<QuaZipFile> zf = std::make_shared<QuaZipFile>($q.get());");
            CPP.Add("std::shared_ptr<ZipEntry> ze = ZipEntry::$new(filename);");
            CPP.Add("ze->$base(zf);");
            return (ZipEntry)CPP.ReturnObject("ze");
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
        }
        private ZipEntry zipEntry;
        public ZipEntry Current {get {return zipEntry;}}
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
}
