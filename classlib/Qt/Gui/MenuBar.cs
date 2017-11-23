using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "public: QMenuBar *$q;"
    )]
    public class MenuBar : Widget {
        public MenuBar() {
            CPP.Add("$q = new QMenuBar();");
            CPP.Add("Widget::$base($q);");
        }
        public Menu AddMenu(Menu menu) {
            CPP.Add("$q->addMenu(menu->$q);");
            return menu;
        }
        public MenuItem AddMenuItem(MenuItem item) {
            CPP.Add("$q->addAction(item->$q);");
            return item;
        }
        public MenuItem AddSeparator() {
            CPP.Add("QAction *$a = $q->addSeparator();");
            return (MenuItem)CPP.ReturnObject("std::make_shared<MenuItem>($a)");
        }
    }
}
