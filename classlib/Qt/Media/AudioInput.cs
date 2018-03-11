using Qt.Core;
using Qt.QSharp;

namespace Qt.Media {
    public delegate void InputInterval(AudioInput ai);
    [CPPClass(
        "private: std::unique_ptr<QAudioInput> $q;" +
        "private: QIODevice *$io;"
    )]
    public class AudioInput {
        private InputInterval iv;
        public AudioInput(AudioFormat format) {
            CPP.Add("$q = std::make_unique<QAudioInput>($check(format)->$value());");
            CPP.Add("$io = nullptr;");
            CPP.Add("QObject::connect($q.get(), &QAudioInput::notify, [=] () {this->SlotNotify();});");
        }
        public AudioInput(AudioDevice device, AudioFormat format) {
            CPP.Add("$q = std::make_unique<QAudioInput>($check(device)->$value(), $check(format)->$value());");
            CPP.Add("$io = nullptr;");
            CPP.Add("QObject::connect($q.get(), &QAudioInput::notify, [=] () {this->SlotNotify();});");
        }
        private void SlotNotify() {
            try {
                if (iv != null) { iv(this); }
            } catch {}
        }
        public int GetInterval() {
            return CPP.ReturnInt("$q->notifyInterval()");
        }
        public void SetInterval(int ms) {
            CPP.Add("$q->setNotifyInterval(ms);");
        }
        public void OnInterval(InputInterval iv) {
            this.iv = iv;
        }
        public int GetBufferSize() {
            return CPP.ReturnInt("$q->bufferSize()");
        }
        public void SetBufferSize(int bytes) {
            CPP.Add("$q->setBufferSize(bytes);");
        }
        public float GetVolume() {
            return CPP.ReturnFloat("$q->volume()");
        }
        public void SetVolume(float level) {
            CPP.Add("$q->setVolume(level);");
        }
        public void Start() {
            CPP.Add("$io = $q->start();");
        }
        public void Stop() {
            CPP.Add("$q->stop();");
            CPP.Add("$io = nullptr;");
        }
        public int Read(byte[] data, int offset = 0, int length = -1) {
            if (length == -1) length = data.Length;
            CPP.Add("if ($io == nullptr) return -1;");
            CPP.Add("$check(data, offset, length);");
            return CPP.ReturnInt("$io->read((char*)data->data() + offset, length)");
        }
    }
}
