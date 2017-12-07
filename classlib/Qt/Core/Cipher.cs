using Qt.QSharp;

namespace Qt.Core {
    public class Cipher {
        public static String AES = "AES";
        private String type;
        public Cipher(String type) {
            this.type = type;
        }
        public ByteArray Encode(ByteArray input, ByteArray key) {
            return (ByteArray)CPP.ReturnObject("ByteArray::$new(QAESEncryption::Crypt(QAESEncryption::AES::AES_128, QAESEncryption::MODE::CBC, *$deref(input)->$value(), *$deref(key)->$value()))");
        }
        public ByteArray Decode(ByteArray input, ByteArray key) {
            return (ByteArray)CPP.ReturnObject("ByteArray::$new(QAESEncryption::Decrypt(QAESEncryption::AES::AES_128, QAESEncryption::MODE::CBC, *$deref(input)->$value(), *$deref(key)->$value()))");
        }
    }
}
