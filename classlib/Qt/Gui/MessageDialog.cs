using Qt.Core;
using Qt.QSharp;

namespace Qt.Gui {
    public enum KeyType {
        None = 0,
        Enter = 0x100,
        Escape = 0x200
    }
    [CPPEnum("QMessageBox::StandardButton")]
    public enum ButtonType {
        Ok = 0x400,
        Save = 0x800,
        SaveAll = 0x1000,
        Open = 0x2000,
        Yes = 0x4000,
        YesToAll = 0x8000,
        No = 0x10000,
        NoToAll = 0x20000,
        Abort = 0x40000,
        Retry = 0x80000,
        Ignore = 0x100000,
        Close = 0x200000,
        Cancel = 0x400000,
        Discard = 0x800000,
        Help = 0x1000000,
        Apply = 0x2000000,
        Reset = 0x4000000,
        RestoreDefaults = 0x8000000
    }
    [CPPClass(
        "public: std::shared_ptr<QMessageBox> $q;"
    )]
    public class MessageDialog : Dialog {
        public MessageDialog(IconType icontype, String title, String msg) : base(QSharpDerived.derived) {
            CPP.Add("$q = std::make_shared<QMessageBox>(icontype, $check(title)->qstring(), $check(msg)->qstring());");
            CPP.Add("Dialog::$base($q);");
        }
        public void AddButton(ButtonType buttonType, KeyType keyType = KeyType.None) {
            CPP.Add("QPushButton *button = $q->addButton(buttonType);");
            if (keyType == KeyType.Enter) {
                CPP.Add("$q->setDefaultButton(button);");
            }
            if (keyType == KeyType.Escape) {
                CPP.Add("$q->setEscapeButton(button);");
            }
        }
    }
}
