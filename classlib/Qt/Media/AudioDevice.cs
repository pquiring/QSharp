using Qt.Core;
using Qt.QSharp;

namespace Qt.Media {
    public enum AudioMode {
        AudioModeInput = 0,
        AudioModeOutput = 1
    }
    [CPPClass(
        "private: QAudioDeviceInfo $q;" +
        "public: QAudioDeviceInfo $value() {return $q;}"
    )]
    public class AudioDevice {
        [CPPReplaceArgs("QAudioDeviceInfo device")]
        private AudioDevice(NativeArg1 arg1) {
            CPP.Add("$q = device;");
        }
        public String GetName() {
            return CPP.ReturnString("Qt::Core::String::$new($q.deviceName())");
        }
        public static AudioDevice[] ListDevices(AudioMode mode) {
            AudioDevice[] list = null;
            CPP.Add("QList<QAudioDeviceInfo> devlist = QAudioDeviceInfo::availableDevices((QAudio::Mode)mode);");
            CPP.Add("int cnt = devlist.size();");
            CPP.Add("list = Qt::QSharp::FixedArray<std::shared_ptr<AudioDevice>>::$new(cnt);");
            CPP.Add("for(int a=0;a<cnt;a++) {list->at(a) = Qt::Media::AudioDevice::$new(devlist.at(a));}");
            return list;
        }
    }
}
