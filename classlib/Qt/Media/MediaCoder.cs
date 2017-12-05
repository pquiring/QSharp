using Qt.QSharp;
using Qt.Core;

namespace Qt.Media {

    /** Base class for Media Coders. */
    public class MediaCoder {
        public static bool loaded = false;

        /** Loads the media framework native libraries. */
        [CPPOmitBody]
        public static bool Init() {return false;}

        //returned by MediaDecoder.Read()
        public const int END_FRAME = -1;
        public const int NULL_FRAME = 0;    //could be metadata frame
        public const int AUDIO_FRAME = 1;
        public const int VIDEO_FRAME = 2;

        //video codecs
        public const int AV_CODEC_ID_NONE = 0;
        public const int AV_CODEC_ID_MPEG1VIDEO = 1;
        public const int AV_CODEC_ID_MPEG2VIDEO = 2;
        public const int AV_CODEC_ID_H263 = 5;
        public const int AV_CODEC_ID_MPEG4 = 13;
        public const int AV_CODEC_ID_H264 = 28;
        public const int AV_CODEC_ID_THEORA = 31;
        public const int AV_CODEC_ID_VP8 = 141;

        //audio codecs
        public const int AV_CODEC_ID_PCM_S16LE = 0x10000;    //wav file
        public const int AV_CODEC_ID_FLAC = 0x1500c;
    }

}