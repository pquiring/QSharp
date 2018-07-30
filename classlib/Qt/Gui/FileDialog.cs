using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPEnum("QFileDialog::FileMode")]
    public enum FileMode {AnyFile, ExistingFile, Directory, ExistingFiles}
    [CPPClass(
        "QFileDialog *$q;"
    )]
    public class FileDialog : Dialog {
        public FileDialog() : base(QSharpDerived.derived) {
            CPP.Add("$q = new QFileDialog();");
            CPP.Add("Dialog::$base($q);");
        }
        public String[] GetSelectedFiles() {
            CPP.Add("QStringList list = $q->selectedFiles();");
            return (String[])CPP.ReturnObject("$QStringListToStringArray(list)");
        }
        public String GetDirectory() {
            return CPP.ReturnString("Qt::Core::String::$new($q->directory().absolutePath())");
        }
        public void SetDirectory(String dir) {
            CPP.Add("$q->setDirectory($check(dir)->qstring());");
        }
        public FileMode GetFileMode() {
            return (FileMode)CPP.ReturnInt("$q->fileMode()");
        }
        public void SetFileMode(FileMode mode) {
            CPP.Add("$q->setFileMode((QFileDialog::FileMode)mode);");
        }
        public String[] GetFilters() {
            CPP.Add("QStringList list = $q->nameFilters();");
            return (String[])CPP.ReturnObject("$QStringListToStringArray(list)");
        }
        public void SetFilters(String[] filters) {
            CPP.Add("QStringList list = $StringArrayToQStringList($check(filters));");
            CPP.Add("$q->setNameFilters(list);");
        }
    }
}
