using Qt.Core;

namespace Qt.Gui {
    public class GetPasswordDialog : Dialog {
        private Label msgLabel;
        private TextField textField;
        private Button ok;
        private Button cancel;
        public GetPasswordDialog(String title, String msg, String text) {
            SetWindowTitle(title);
            VBoxLayout layout = new VBoxLayout();
            HBoxLayout line = new HBoxLayout();
            layout.AddLayout(line);
            msgLabel = new Label(msg);
            line.AddWidget(msgLabel);
            textField = new TextField(text);
            textField.SetPasswordMode(true);
            line.AddWidget(textField);
            HBoxLayout buttons = new HBoxLayout();
            layout.AddLayout(buttons);
            ok = new Button("Okay");
            ok.OnClicked(() => {Accept();});
            buttons.AddWidget(ok);
            cancel = new Button("Cancel");
            cancel.OnClicked(() => {Reject();});
            buttons.AddWidget(cancel);
            buttons.SetAlignment(Alignment.AlignRight);
            SetLayout(layout);
        }
        public String GetText() {
            return textField.GetText();
        }
    }
}
