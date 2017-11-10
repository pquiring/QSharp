using Qt.QSharp;

namespace Qt.Gui {
    [CPPExtends("QGuiApplication")]
    [CPPClass(
        "public: GuiApplication() : QGuiApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv) {};\r\n"
    )]
    /** GuiApplication is for GUI apps that do NOT use Widgets (like OpenGL apps) */
    public class GuiApplication {
        public static void Exit(int returnCode = 0) {
            CPP.Add("exit(returnCode);\r\n");
        }
        public void Exec() {
            CPP.Add("exec();");
        }
    }
}
