using Qt.QSharp;

namespace Qt.Gui {
    [CPPClass(
        "QMenuBar *$q;"
    )]
    public class MenuBar : Widget {
        public MenuBar() {
            CPP.Add("$q = new QMenuBar();");
            CPP.Add("Widget::$base($q);");
        }
        public Menu AddMenu(Menu menu) {
            CPP.Add("$q->addMenu($check(menu)->$q);");
            return menu;
        }
        public MenuItem AddMenuItem(MenuItem item) {
            CPP.Add("$q->addAction($check(item)->$q);");
            return item;
        }
        public MenuItem AddSeparator() {
            return (MenuItem)CPP.ReturnObject("new MenuItem($q->addSeparator())");
        }
    }
}
