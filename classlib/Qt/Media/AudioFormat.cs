using Qt.Core;
using Qt.QSharp;

namespace Qt.Media {
    [CPPClass(
        "private: std::unique_ptr<QAudioFormat> $q;" +
        "public: QAudioFormat $value() {return *$q.get();}"
    )]
    public class AudioFormat {
        public AudioFormat() {
            CPP.Add("$q = std::make_unique<QAudioFormat>();");
        }
        public int GetBits() {
            return CPP.ReturnInt("$q->sampleSize()");
        }
        public void SetBits(int bits) {
            CPP.Add("$q->setSampleSize(bits);");
        }
        public int GetRate() {
            return CPP.ReturnInt("$q->sampleRate()");
        }
        public void SetRate(int rate) {
            CPP.Add("$q->setSampleRate(rate);");
        }
        public int GetChannels() {
            return CPP.ReturnInt("$q->channelCount()");
        }
        public void SetChannels(int chs) {
            CPP.Add("$q->setChannelCount(chs);");
        }
    }
}
