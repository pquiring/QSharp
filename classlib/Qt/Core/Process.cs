using Qt.QSharp;

namespace Qt.Core {
    [CPPClass("QProcess* $d() {return dynamic_cast<QProcess*>($q.get());}")]
    public class Process : IOStream {
        public Process() {
            CPP.Add("$base(new QProcess());");
        }
        public String GetWorkingDirectory() {
            return CPP.ReturnString("Qt::Core::String::$new($d()->workingDirectory())");
        }
        public void SetWorkingDirectory(String directory) {
            CPP.Add("$d()->setWorkingDirectory($check(directory)->qstring());");
        }
        public String GetProgram() {
            return CPP.ReturnString("Qt::Core::String::$new($d()->program())");
        }
        public void SetProgram(String directory) {
            CPP.Add("$d()->setProgram($check(directory)->qstring());");
        }
        public String[] GetArguments() {
            return (String[])CPP.ReturnObject("$QStringListToStringArray($d()->arguments())");
        }
        public void Start() {
            CPP.Add("$d()->start();");
        }
        public bool WaitForStarted(int timeoutms = 30000) {
            return CPP.ReturnBool("$d()->waitForStarted(timeoutms)");
        }
        public bool WaitForFinished(int timeoutms = 30000) {
            return CPP.ReturnBool("$d()->waitForFinished(timeoutms)");
        }
    }
}
