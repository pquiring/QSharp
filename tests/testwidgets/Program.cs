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
            Window window = new Window();
            window.SetSize(640, 480);
            window.SetCentralWidget(widget);
            window.Show();
            NativeWindow nw = window.GetNativeWindow();
            nw.OnInputEvents(new Events());
            app.Exec();
        }
        public static GroupBox group() {
            GroupBox group = new GroupBox("Group Box");
            VBoxLayout layout = new VBoxLayout();
            Button pb = new Button();
            pb.SetText("Test #1");
            pb.OnClicked(() => {Console.WriteLine("Clicked Button!");});
            layout.AddWidget(pb);
            ToggleButton tb = new ToggleButton("Test #2");
            layout.AddWidget(tb);
            group.SetLayout(layout);
            return group;
        }
    }
    public class Events : InputEvents {
        public override bool KeyPressed(KeyCode key) {
            Console.WriteLine("KeyPressed:" + (int)key);
            return false;
        }

        public override bool KeyReleased(KeyCode key) {
            Console.WriteLine("KeyReleased:" + (int)key);
            return false;
        }

        public override bool KeyTyped(char key) {
            Console.WriteLine("KeyTyped:" + key);
            return false;
        }

        public override bool MousePressed(int x, int y, int button) {
            Console.WriteLine("MousePressed:" + x  + "," + y + ":" + button);
            return false;
        }

        public override bool MouseReleased(int x, int y, int button) {
            Console.WriteLine("MouseReleased:" + x  + "," + y + ":" + button);
            return false;
        }

        public override bool MouseMoved(int x, int y, int button) {
            Console.WriteLine("MouseMoved:" + x  + "," + y + ":" + button);
            return false;
        }
    }
}
