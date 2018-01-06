using Qt.Core;
using Qt.QSharp;

namespace Qt.Gui {
    public class KeyType {
        public const int None = 0;
        public const int Enter = 0x100;
        public const int Escape = 0x200;
    }
    public class ButtonType {
        public const int Ok = 0x400;
        public const int Save = 0x800;
        public const int SaveAll = 0x1000;
        public const int Open = 0x2000;
        public const int Yes = 0x4000;
        public const int YesToAll = 0x8000;
        public const int No = 0x10000;
        public const int NoToAll = 0x20000;
        public const int Abort = 0x40000;
        public const int Retry = 0x80000;
        public const int Ignore = 0x100000;
        public const int Close = 0x200000;
        public const int Cancel = 0x400000;
        public const int Discard = 0x800000;
        public const int Help = 0x1000000;
        public const int Apply = 0x2000000;
        public const int Reset = 0x4000000;
        public const int RestoreDefaults = 0x8000000;
    }
    [CPPClass(
        "public: std::shared_ptr<QMessageBox> $q;"
    )]
    public class MessageDialog : Dialog {
        public MessageDialog(IconType icontype, String title, String msg) : base(QSharpDerived.derived) {
            CPP.Add("$q = std::make_shared<QMessageBox>((QMessageBox::Icon)icontype, $check(title)->qstring(), $check(msg)->qstring());");
            CPP.Add("Dialog::$base($q);");
        }
        public void AddButton(int buttonType, int keyType = KeyType.None) {
            CPP.Add("QPushButton *button = $q->addButton((QMessageBox::StandardButton)buttonType);");
            if (keyType == KeyType.Enter) {
                CPP.Add("$q->setDefaultButton(button);");
            }
            if (keyType == KeyType.Escape) {
                CPP.Add("$q->setEscapeButton(button);");
            }
        }
    }
}
