using Qt.QSharp;
using Qt.Core;

namespace Qt.Media {
    public class Wav {
        private short[] sams = null;
        private int channels = 0;  //1 or 2
        private int rate = 0;  //44.1k
        private int bits = 0;  //16bit only supported
        private int length = 0;  //# of samples combined (length of sams)

        public bool Load(String filename) {
            CPP.Add("QAudioDecoder decoder;");
            CPP.Add("decoder.setSourceFilename($check(filename)->qstring());");
            CPP.Add("QAudioBuffer buffer = decoder.read();");
            CPP.Add("QAudioFormat format = buffer.format();");
            CPP.Add("length = buffer.sampleCount();");
            CPP.Add("channels = format.channelCount();");
            CPP.Add("rate = format.sampleRate();");
            CPP.Add("bits = format.sampleSize();");
            if (bits != 16) return false;  //only 16bit supported
            sams = new short[length];
            CPP.Add("std::memcpy(sams->data(), buffer.data(), length * 2);");
            return true;
        }
        public short[] GetSamples() {
            return sams;
        }
        public short[] GetSamples(int offset, int length) {
            if (sams == null) return null;
            if (length == sams.Length) {
                return sams;
            }
            if (offset + length >= sams.Length) {
                return null;
            }
            short[] frag = new short[length];
            CPP.Add("std::memcpy(frag->data(), sams->data() + offset, length * 2);");
            return frag;
        }
        public int GetSamplesCount() {
            return length;
        }
        public int GetChannels() {
            return channels;
        }
        public int GetRate() {
            return rate;
        }
        public int GetBits() {
            return bits;
        }
    }
}
