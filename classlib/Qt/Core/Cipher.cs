using Qt.QSharp;

namespace Qt.Core {
    public class Cipher {
        public static String AES = "AES";
        private String type;
        public Cipher(String type) {
            this.type = type;
        }
        public ByteArray Encode(ByteArray input, ByteArray key) {
            return (ByteArray)CPP.ReturnObject("std::make_shared<ByteArray>(QAESEncryption::Crypt(QAESEncryption::AES::AES_256, QAESEncryption::MODE::CFB, *input->$q, *key->$q))");
        }
        public ByteArray Decode(ByteArray input, ByteArray key) {
            return (ByteArray)CPP.ReturnObject("std::make_shared<ByteArray>(QAESEncryption::Decrypt(QAESEncryption::AES::AES_256, QAESEncryption::MODE::CFB, *input->$q, *key->$q))");
        }
    }
}
