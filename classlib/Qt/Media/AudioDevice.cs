using Qt.Core;
using Qt.QSharp;

namespace Qt.Media {
    [CPPEnum("QAudio::Mode")]
    public enum AudioMode {
        AudioModeInput = 0,
        AudioModeOutput = 1
    }
    [CPPClass(
        "QAudioDeviceInfo $q;" +
        "QAudioDeviceInfo $value() {return $q;}"
    )]
    public class AudioDevice {
        [CPPReplaceArgs("QAudioDeviceInfo device")]
        private AudioDevice(NativeArg1 arg1) {
            CPP.Add("$q = device;");
        }
        public String GetName() {
            return CPP.ReturnString("new Qt::Core::String($q.deviceName())");
        }
        public static AudioDevice[] ListDevices(AudioMode mode) {
            AudioDevice[] list = null;
            CPP.Add("QList<QAudioDeviceInfo> devlist = QAudioDeviceInfo::availableDevices((QAudio::Mode)mode);");
            CPP.Add("int cnt = devlist.size();");
            CPP.Add("list = new Qt::QSharp::FixedArray<AudioDevice*>(cnt);");
            CPP.Add("for(int a=0;a<cnt;a++) {list->at(a) = new Qt::Media::AudioDevice(devlist.at(a));}");
            return list;
        }
    }
}
