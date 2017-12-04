﻿using Qt.Core;
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
}
