using Qt.Core;
using Qt.Gui;

namespace testwidgets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Application app = new Application();
            GridLayout layout = new GridLayout();
            layout.AddWidget(group(), 0, 0);
            layout.AddWidget(group(), 0, 1);
            layout.AddWidget(group(), 1, 0);
            layout.AddWidget(group(), 1, 1);
            Widget widget = new Widget();
            widget.SetLayout(layout);
            MainWindow window = new MainWindow();
            window.SetSize(640, 480);
            window.SetCentralWidget(widget);
            window.Show();
            app.Exec();
        }
        public static GroupBox group() {
            GroupBox group = new GroupBox("Group Box");
            VBoxLayout layout = new VBoxLayout();
            PushButton pb = new PushButton();
            pb.SetText("Test #1");
            pb.OnClicked((bool selected) => {Console.WriteLine("Clicked Button!");});
            layout.AddWidget(pb);
            PushButton pb2 = new PushButton("Test #2");
            layout.AddWidget(pb2);
            group.SetLayout(layout);
            return group;
        }
    }
}
