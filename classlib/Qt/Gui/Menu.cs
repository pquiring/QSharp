using Qt.QSharp;
using Qt.Core;

namespace Qt.Gui {
    [CPPClass(
        "QMenu *$q;"
    )]
    public class Menu : Widget {
        public Menu(String title) {
            CPP.Add("$q = new QMenu($check(title)->qstring());");
            CPP.Add("Widget::$base($q);");
        }
        public MenuItem AddAction(String text) {
            return (MenuItem)CPP.ReturnObject("MenuItem::$new($q->addAction($check(text)->qstring()))");
        }
        public MenuItem AddAction(Image icon, String text) {
            return (MenuItem)CPP.ReturnObject("MenuItem::$new($q->addAction($check(icon)->$icon(), $check(text)->qstring()))");
        }
        public MenuItem AddSeparator() {
            return (MenuItem)CPP.ReturnObject("MenuItem::$new($q->addSeparator())");
        }
    }
}