using Qt.QSharp;
using Qt.Core;

namespace Qt.Media {
    /** Media encoder. */
    [CPPClass("public: std::shared_ptr<FFContext> ctx;")]
    public class MediaEncoder : MediaCoder {
        //these must be set BEFORE you call start()
        public bool FPS_1000_1001;
        public int FramesPerKeyFrame = 12;
        public int VideoBitRate = 400000;
        public int AudioBitRate = 128000;
        [CPPOmitBody]
        public bool Start(MediaIO io, int width, int height, int fps, int chs, int freq, String codec, bool doVideo, bool doAudio) {return false;}
        /** Sets frame rate = fps * 1000 / 1001 (default = false) */
        public void Set1000over1001(bool state) {
            FPS_1000_1001 = state;
        }
        /** Sets frames per key frame (gop) (default = 12) */
        public void SetFramesPerKeyFrame(int count) {
            FramesPerKeyFrame = count;
        }
        /** Sets video bit rate (default = 400000) */
        public void SetVideoBitRate(int rate) {
            VideoBitRate = rate;
        }
        /** Sets audio bit rate (default = 128000) */
        public void SetAudioBitRate(int rate) {
            AudioBitRate = rate;
        }
        [CPPOmitBody]
        public bool AddAudio(short[] sams, int offset, int length) {return false;}
        public bool AddAudio(short[] sams) {
            return AddAudio(sams, 0, sams.Length);
        }
        [CPPOmitBody]
        public bool AddVideo(int[] px) {return false;}
        [CPPOmitBody]
        public int GetAudioFramesize() {return 0;}
        [CPPOmitBody]
        public void Stop() {}
    }
}
