using Qt.QSharp;

namespace Qt.Media {
    /** Media "raw" video decoder. */
    [CPPClass("std::shared_ptr<FFContext> ctx;")]
    [CPPOmitBodies]
    public class MediaVideoDecoder : MediaCoder {
        public bool Start(int codec_id, int new_width, int new_height) {return false;}
        public void Stop() {}
        public int[] Decode(byte[] data) {return null;}
        public int GetWidth() {return 0;}
        public int GetHeight() {return 0;}
        public float GetFrameRate() {return 0;}
    }
}
