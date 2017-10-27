using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: QGuiApplication *q = nullptr;\r\n" +
        "public: GuiApplication() {q = new QGuiApplication(Qt::Core::g_argc, (char**)Qt::Core::g_argv);}\r\n"
    )]
    public class GuiApplication {
        public void Exec() {
            CPP.Add("q->exec();");
        }
        ~GuiApplication() {
            CPP.Add("delete q;");
        }
    }
}
