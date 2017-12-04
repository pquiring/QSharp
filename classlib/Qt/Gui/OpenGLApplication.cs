using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QGuiApplication")]
    [CPPClass(
        "public: OpenGLApplication() : QGuiApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv) {};"
    )]
    /** OpenGLApplication is for apps that do NOT use Widgets. */
    public class OpenGLApplication {
        public static void Exit(int returnCode = 0) {
            CPP.Add("exit(returnCode);");
        }
        public void Exec() {
            CPP.Add("exec();");
        }
    }
}
