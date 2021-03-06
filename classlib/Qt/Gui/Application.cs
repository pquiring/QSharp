using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "std::qt_ptr<QApplication> $q;"
    )]
    /** Application is for GUI apps that use Widgets*/
    public class Application {
        public Application() {
            CPP.Add("$q = new QApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv);");
        }
        public static void Exit(int returnCode = 0) {
            CPP.Add("QApplication::exit(returnCode);");
        }
        public void Exec() {
            CPP.Add("$q->exec();");
        }
    }
}
