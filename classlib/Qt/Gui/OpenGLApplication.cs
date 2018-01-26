using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: std::shared_ptr<QGuiApplication> $q;"
    )]
    /** OpenGLApplication is for apps that do NOT use Widgets. */
    public class OpenGLApplication {
        public OpenGLApplication() {
            CPP.Add("$q = std::make_shared<QGuiApplication>(Qt::Core::g_argc, (char**)Qt::Core::g_argv);");
        }
        public static void Exit(int returnCode = 0) {
            CPP.Add("QGuiApplication::exit(returnCode);");
        }
        public void Exec() {
            CPP.Add("$q->exec();");
        }
    }
}
