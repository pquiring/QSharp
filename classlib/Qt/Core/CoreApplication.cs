using Qt.QSharp;

namespace Qt.Core {
    [CPPExtends("QCoreApplication")]
    [CPPClass(
        "public: CoreApplication() : QCoreApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv) {};\r\n"
    )]
    /** CoreApplication is for non-GUI (console) apps */
    public class CoreApplication {
        public static void Exit(int returnCode = 0) {
            CPP.Add("exit(returnCode);\r\n");
        }
        public void Exec() {
            CPP.Add("exec();\r\n");
        }
    }
}