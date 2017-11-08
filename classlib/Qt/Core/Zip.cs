using Qt.QSharp;

namespace Qt.Core {
    public enum ZipMode { mdNotOpen, mdUnzip, mdCreate, mdAppend, mdAdd }
    [CPPClass(
        "private: std::shared_ptr<QuaZipFile> $q;\r\n" +
        "public: void $base(std::shared_ptr<QuaZipFile> zf) {$q = zf; IODevice::$base((std::shared_ptr<QIODevice>)zf);}"
    )]
    public class ZipEntry : IODevice {
        private String _filename;
        public String filename {get {return _filename;}}
        public ZipEntry(String filename) {
            _filename = filename;
        }
    }
    [CPPClass(
        "private: std::shared_ptr<QuaZip> $q;\r\n"
    )]
    public class ZipFile {
        protected String filename;
        public ZipFile(String filename, OpenMode mode) {
            CPP.Add("$q = std::make_shared<QuaZip>(filename->qstring());");
        }
        public bool Open(ZipMode mode) {
            return CPP.ReturnBool("$q->open((QuaZip::Mode)mode)");
        }
        public bool NextEntry() {
            bool next = CPP.ReturnBool("$q->goToNextFile()");
            if (next) {
                CPP.Add("filename = std::make_shared<String>($q->getCurrentFileName());\r\n");
            } else {
                filename = null;
            }
            return next;
        }
        public ZipEntry GetEntry() {
            CPP.Add("std::shared_ptr<QuaZipFile> zf = std::make_shared<QuaZipFile>($q.get());\r\n");
            CPP.Add("std::shared_ptr<ZipEntry> ze = std::make_shared<ZipEntry>(); ze->$ctor(filename);\r\n");
            CPP.Add("ze->$base(zf);\r\n");
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
