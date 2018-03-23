using Qt.QSharp;

namespace Qt.Core {
    [CPPEnum("QuaZip::Mode")]
    public enum ZipMode { mdNotOpen, mdUnzip, mdCreate, mdAppend, mdAdd }
    [CPPClass(
        "std::shared_ptr<QuaZipFile> $q;" +
        "void $base(std::shared_ptr<QuaZipFile> zf) {$q = zf; IOStream::$base((std::shared_ptr<QIODevice>)zf);}"
    )]
    public class ZipEntry : IOStream {
        private String _filename;
        public String filename {get {return _filename;}}
        public ZipEntry(String filename) {
            _filename = filename;
        }
    }
    [CPPClass(
        "std::shared_ptr<QuaZip> $q;"
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
                CPP.Add("filename = Qt::Core::String::$new($q->getCurrentFileName());");
            } else {
                filename = null;
            }
            return next;
        }
        public bool FirstEntry() {
            bool first = CPP.ReturnBool("$q->goToFirstFile()");
            if (first) {
                CPP.Add("filename = Qt::Core::String::$new($q->getCurrentFileName());");
            } else {
                filename = null;
            }
            return first;
        }
        public ZipEntry GetEntry() {
            CPP.Add("std::shared_ptr<QuaZipFile> zf = std::make_shared<QuaZipFile>($q.get());");
            CPP.Add("std::shared_ptr<ZipEntry> ze = ZipEntry::$new(filename);");
            CPP.Add("ze->$base(zf);");
            return (ZipEntry)CPP.ReturnObject("ze");
        }
        public ZipEntry GetEntry(String filename) {
            IEnumerator<ZipEntry> zipEnum = GetEnumerator();
            ZipEntry zipEntry;
            while(zipEnum.MoveNext()) {
                zipEntry = zipEnum.Current;
                if (zipEntry.filename == filename) {
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
    [CPPClass(
        "std::shared_ptr<QuaZIODevice> $q;"
    )]
    public class Compress : IOStream {
        public Compress(ByteArrayStream input) {
            CPP.Add("$q = std::make_shared<QuaZIODevice>($check(input)->$value());");
            CPP.Add("$base($q);");
        }
    }
    [CPPClass(
        "std::shared_ptr<QuaZIODevice> $q;"
    )]
    public class Decompress : IOStream {
        public Decompress(ByteArrayStream input) {
            CPP.Add("$q = std::make_shared<QuaZIODevice>($check(input)->$value());");
            CPP.Add("$base($q);");
        }
    }
}
