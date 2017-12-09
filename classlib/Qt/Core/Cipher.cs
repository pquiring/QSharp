using Qt.QSharp;

namespace Qt.Core {
    public class Cipher {
        public static String AES = "AES";
        public static String SHA1 = "SHA1";
        private String algo;
        private String digest;
        public Cipher(string algo, string digest) {
            this.algo = algo;
            this.digest = digest;
        }
        public ByteArray Encode(ByteArray input, ByteArray key) {
            CPP.Add("QByteArray ckey = QCryptographicHash::hash(*$deref(key)->$value(), QCryptographicHash::Sha1);");
            return (ByteArray)CPP.ReturnObject("ByteArray::$new(QAESEncryption::Crypt(QAESEncryption::AES::AES_128, QAESEncryption::MODE::ECB, *$deref(input)->$value(), ckey))");
        }
        public ByteArray Decode(ByteArray input, ByteArray key) {
            CPP.Add("QByteArray ckey = QCryptographicHash::hash(*$deref(key)->$value(), QCryptographicHash::Sha1);");
            return (ByteArray)CPP.ReturnObject("ByteArray::$new(QAESEncryption::Decrypt(QAESEncryption::AES::AES_128, QAESEncryption::MODE::ECB, *$deref(input)->$value(), ckey))");
        }
    }
}
