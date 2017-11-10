using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QApplication")]
    [CPPClass(
        "public: Application() : QApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv) {};\r\n"
    )]
    /** Application is for GUI apps that use Widgets*/
    public class Application {
        public static void Exit(int returnCode = 0) {
            CPP.Add("exit(returnCode);\r\n");
        }
        public void Exec() {
            CPP.Add("exec();");
        }
    }
}
