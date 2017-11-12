using Qt.Core;
using Qt.Gui;

namespace testwidgets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Application app = new Application();
            VBoxLayout layout = new VBoxLayout();
            PushButton pb = new PushButton();
            pb.SetText("Test");
            pb.OnClicked((bool selected) => {Console.WriteLine("Clicked Button!");});
            layout.AddWidget(pb);
            Widget window = new Widget();
            window.SetSize(640, 480);
            window.SetLayout(layout);
            window.Show();
            app.Exec();
        }
    }
}
