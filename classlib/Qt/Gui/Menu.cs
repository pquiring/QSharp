using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QMenu *$q;"
    )]
    public class Menu : Widget {
        public Menu(String title) {
            CPP.Add("$q = new QMenu(title->qstring());");
            CPP.Add("Widget::$base($q);");
        }
        public MenuItem AddAction(String text) {
            CPP.Add("QAction *$a = $q->addAction(text->qstring());");
            return (MenuItem)CPP.ReturnObject("std::make_shared<MenuItem>($a)");
        }
        public MenuItem AddAction(Image icon, String text) {
            CPP.Add("QAction *$a = $q->addAction(icon->$icon(), text->qstring());");
            return (MenuItem)CPP.ReturnObject("std::make_shared<MenuItem>($a)");
        }
        public MenuItem AddSeparator() {
            CPP.Add("QAction *$a = $q->addSeparator();");
            return (MenuItem)CPP.ReturnObject("std::make_shared<MenuItem>($a)");
        }
    }
}