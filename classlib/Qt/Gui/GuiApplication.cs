using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "private: std::shared_ptr<QGuiApplication> $q;\r\n"
    )]
    public class GuiApplication {
        public GuiApplication() {
            CPP.Add("$q = std::make_shared<QGuiApplication>(Qt::Core::g_argc, (char**)Qt::Core::g_argv);");
        }
        public void Exec() {
            CPP.Add("$q->exec();");
        }
    }
}
