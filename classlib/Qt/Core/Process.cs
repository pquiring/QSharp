using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "private: std::shared_ptr<QProcess> $q;"
    )]
    public class Process : IOStream {
        public Process() {
            CPP.Add("$q = std::make_shared<QProcess>();");
            CPP.Add("IOStream::$base((std::shared_ptr<QIODevice>)$q);");
        }
        public String GetWorkingDirectory() {
            return CPP.ReturnString("Qt::Core::String::$new($q->workingDirectory())");
        }
        public void SetWorkingDirectory(String directory) {
            CPP.Add("$q->setWorkingDirectory($check(directory)->qstring());");
        }
        public String GetProgram() {
            return CPP.ReturnString("Qt::Core::String::$new($q->program())");
        }
        public void SetProgram(String directory) {
            CPP.Add("$q->setProgram($check(directory)->qstring());");
        }
        public String[] GetArguments() {
            return (String[])CPP.ReturnObject("$QStringListToStringArray($q->arguments())");
        }
        public void Start() {
            CPP.Add("$q->start();");
        }
        public bool WaitForStarted(int timeoutms = 30000) {
            return CPP.ReturnBool("$q->waitForStarted(timeoutms)");
        }
        public bool WaitForFinished(int timeoutms = 30000) {
            return CPP.ReturnBool("$q->waitForFinished(timeoutms)");
        }
    }
}
