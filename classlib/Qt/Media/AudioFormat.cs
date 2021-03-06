using Qt.Core;
using Qt.QSharp;

namespace Qt.Media {
    [CPPClass(
        "std::qt_ptr<QAudioFormat> $q;" +
        "QAudioFormat $value() {return *$q.get();}"
    )]
    public class AudioFormat {
        public AudioFormat() {
            CPP.Add("$q = new QAudioFormat();");
            CPP.Add("$q->setCodec(\"audio/pcm\");");
            CPP.Add("$q->setSampleType(QAudioFormat::SignedInt);");
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
