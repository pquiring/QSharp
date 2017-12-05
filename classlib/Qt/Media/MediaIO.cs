using Qt.QSharp;
using Qt.Core;

namespace Qt.Media {
    /** Interface for reading/writing files for media coders. */
    public interface MediaIO {
        int Read(MediaCoder coder, byte[] data);
        int Write(MediaCoder coder, byte[] data);
        long Seek(MediaCoder coder, long pos);
        long GetPosition(MediaCoder coder);
        long GetSize(MediaCoder coder);
    }
}
