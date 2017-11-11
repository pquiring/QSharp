using Qt.Core;
using Qt.Gui;

namespace testwidgets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Application app = new Application();
            Widget window = new Widget();
            window.SetSize(640, 480);
            window.Show();
            app.Exec();
        }
    }
}
