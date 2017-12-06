using Qt.QSharp;

namespace Qt.Core {
    public class Cipher {
        public static String AES = "AES";
        private String type;
        public Cipher(String type) {
            this.type = type;
        }
        public ByteArray Encode(ByteArray input, ByteArray key) {
            CPP.Add("std::shared_ptr<ByteArray> array = ByteArray::$new();");
            CPP.Add("array->$base(QAESEncryption::Crypt(QAESEncryption::AES::AES_128, QAESEncryption::MODE::CBC, *$deref(input)->$q, *$deref(key)->$q));");
            return (ByteArray)CPP.ReturnObject("array");
        }
        public ByteArray Decode(ByteArray input, ByteArray key) {
            CPP.Add("std::shared_ptr<ByteArray> array = ByteArray::$new();");
            CPP.Add("array->$base(QAESEncryption::Decrypt(QAESEncryption::AES::AES_128, QAESEncryption::MODE::CBC, *$deref(input)->$q, *$deref(key)->$q));");
            return (ByteArray)CPP.ReturnObject("array");
        }
    }
}
