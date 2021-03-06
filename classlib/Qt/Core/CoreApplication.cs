using Qt.QSharp;

namespace Qt.Core {
    [CPPClass(
        "std::qt_ptr<QCoreApplication> $q;"
    )]
    /** CoreApplication is for non-GUI (console) apps */
    public class CoreApplication {
        public CoreApplication() {
            CPP.Add("$q = new QCoreApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv);");
        }
        public static void Exit(int returnCode = 0) {
            CPP.Add("QCoreApplication::exit(returnCode);");
        }
        public void Exec() {
            CPP.Add("$q->exec();");
        }
    }
}
