using Qt.QSharp;
using Qt.Core;

namespace Qt.Media {
    //returned by MediaDecoder.Read()
    public enum MediaFrameType {
        End = -1,
        Meta = 0,
        Audio = 1,
        Video = 2
    }

    /** Media Decoder. */
    [CPPClass("public: std::shared_ptr<FFContext> ctx;")]
    [CPPOmitBodies]
    public class MediaDecoder : MediaCoder {
        public bool Start(MediaIO io, int new_width, int new_height, int new_chs, int new_freq, bool seekable) {return false;}
        public bool Start(String file, String input_format, int new_width, int new_height, int chs, int new_freq) {return false;}
        public void Stop() {}
        public MediaFrameType Read() {return 0;}
        public int[] GetVideo() {return null;}
        public short[] GetAudio() {return null;}
        public int GetWidth() {return 0;}
        public int GetHeight() {return 0;}
        public float GetFrameRate() {return 0;}
        public long GetDuration() {return 0;}
        public int GetSampleRate() {return 0;}
        public int GetChannels() {return 0;}
        public int GetBitsPerSample() {return 0;}
        public bool Seek(long seconds) {return false;}
        public int GetVideoBitRate() {return 0;}
        public int GetAudioBitRate() {return 0;}
        public bool IsKeyFrame() {return false;}
        public bool Resize(int newWidth, int newHeight) {return false;}
    }
}
