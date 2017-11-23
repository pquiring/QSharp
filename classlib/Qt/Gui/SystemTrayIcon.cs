using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    public enum MessageIcon {NoIcon, Information, Warning, Critical};
    [CPPClass(
        "public: QSystemTrayIcon *$q;"
    )]
    public class SystemTrayIcon {
        public SystemTrayIcon(Image icon) {
            CPP.Add("$q = new QSystemTrayIcon(icon->$icon());");
        }
        public void SetContextMenu(Menu menu) {
            CPP.Add("$q->setContextMenu(menu->$q);");
        }
        public void SetIcon(Image icon) {
            CPP.Add("$q->setIcon(icon->$icon());");
        }
        public void ShowMessage(String title, String msg, MessageIcon MessageIcon = MessageIcon.Information, int msTimeout = 10000) {
            CPP.Add("$q->showMessage(title->qstring(), msg->qstring(), (QSystemTrayIcon::MessageIcon)MessageIcon, msTimeout);");
        }
/*  //available in Qt 5.9
        public void ShowMessage(String title, String msg, Image icon, int msTimeout = 10000) {
            CPP.Add("$q->showMessage(title->qstring(), msg->qstring(), icon->$icon(), msTimeout);");
        }
*/
    }
}