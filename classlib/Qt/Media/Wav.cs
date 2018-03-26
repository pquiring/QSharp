using Qt.QSharp;
using Qt.Core;

namespace Qt.Media {
    public class Wav {
        public String errmsg;    //last errmsg if any
        public int chs = -1;    //# channels
        public int rate = -1;    //sample rate (freq)
        public int bits = -1;    //bits per sample
        public int bytes = -1;    //byte per sample
        public byte[] samples8;    //8bit samples
        public short[] samples16;    //16bit samples
        public int[] samples32;    //32bit samples
        public int dataLength;    //bytes

        public bool Load(String fn) {
            try {
                File wav = new File(fn);
                if (!wav.Open(OpenMode.ReadOnly)) return false;
                return Load(wav);
            } catch (Exception e) {
                Console.WriteLine("Error:" + e.ToString());
                return false;
            }
        }

        public bool Load(IOStream wav) {
            errmsg = "";
            try {
                byte[] data = new byte[30];
                //read RIFF header (20 bytes);
                wav.Read(data, 0, 20);
                if (!LE.getString(data, 0, 4).Equals("RIFF")) throw new Exception("wav is not a valid WAV file (RIFF)");
                if (!LE.getString(data, 8, 4).Equals("WAVE")) throw new Exception("wav is not a valid WAV file (WAVE)");
                if (!LE.getString(data, 12, 4).Equals("fmt ")) throw new Exception("wav is not a valid WAV file (fmt )");
                int fmtsiz = LE.getuint32(data, 16);
                if ((fmtsiz < 16) || (fmtsiz > 30)) throw new Exception("wav is not a valid WAV file (fmtsiz)");
                wav.Read(data, 0, fmtsiz);
                if (LE.getuint16(data, 0) != 1) throw new Exception("wav is not PCM");
                chs = LE.getuint16(data, 2);
                if (chs < 1 || chs > 2) throw new Exception("wav is not supported (# chs)");
                rate = LE.getuint32(data, 4);
                bits = LE.getuint16(data, 14);
                switch (bits) {
    //                case 8: bytes = 1; break;    //can't support 8bit for now (upscale later ???)
                    case 16: bytes = 2; break;
                    case 24: bytes = 3; break;
                    case 32: bytes = 4; break;
                    default: throw new Exception("wav is not supported (bits="+bits+")");
                }
                wav.Read(data, 0, 8);
                while (true) {
                    dataLength = LE.getuint32(data, 4);
                    if (LE.getString(data, 0, 4).Equals("data")) break;
                    //ignore chunk (FACT, INFO, etc.)
                    wav.SetPosition(wav.GetPosition() + dataLength);
                    wav.Read(data, 0, 8);
                }
                samples8 = wav.ReadAll().ToArray();
                wav.Close();
                switch (bits) {
                    case 8: return true;
                    case 24:
                        //TODO!!!???
                        break;
                    case 16:
                        samples16 = new short[dataLength / 2];
                        CPP.Add("std::memcpy(samples16->data(), samples8->data(), dataLength);");
                        break;
                    case 32:
                        samples32 = new int[dataLength / 4];
                        CPP.Add("std::memcpy(samples32->data(), samples8->data(), dataLength);");
                        break;
                }
            } catch (Exception e1) {
                errmsg = e1.ToString();
                wav.Close();
                return false;
            }
            return true;
        }

        public bool Save(String fn) {
            try {
                File fos = new File(fn);
                fos.Open(OpenMode.WriteOnly);
                bool ret = Save(fos);
                fos.Close();
                return ret;
            } catch (Exception e) {
                Console.WriteLine("Error:" + e.ToString());
                return false;
            }
        }

        /** Save entire wav file (supports 16/32bit only) */
        public bool Save(IOStream os) {
            if (bits != 16 && bits != 32) return false;
            int size = 0;
            switch (bits) {
                case 16:
                    bytes = 2;
                    size = samples16.Length * 2;
                    break;
                case 32:
                    bytes = 4;
                    size = samples32.Length * 4;
                    break;
            }
            try {
                byte[] data = new byte[20];
                //write RIFF header (20 bytes);
                LE.setString(data, 0, 4, "RIFF");
                LE.setuint32(data, 4, size + 36);    //rest of file size
                LE.setString(data, 8, 4, "WAVE");
                LE.setString(data, 12, 4, "fmt ");
                LE.setuint32(data, 16, 16);    //fmt size
                os.Write(data, 0, 20);
                //write fmt header (16 bytes)
                data = new byte[16 + 4 + 4];
                LE.setuint16(data, 0, 1);    //PCM
                LE.setuint16(data, 2, chs);
                LE.setuint32(data, 4, rate);
                LE.setuint32(data, 8, bytes * chs * rate);    //bytes rate/sec
                LE.setuint32(data, 12, bytes * chs);    //block align
                LE.setuint16(data, 14, bits);
                LE.setString(data, 16, 4, "data");
                LE.setuint32(data, 20, size);
                os.Write(data, 0, 16 + 4 + 4);
                switch (bits) {
                    case 16: os.Write(LE.shortArray2byteArray(samples16, null)); break;
                    case 32: os.Write(LE.intArray2byteArray(samples32, null)); break;
                }
            } catch (Exception e) {
                errmsg = e.ToString();
                return false;
            }
            return false;
        }
        public byte[] GetSamples8() {
            return samples8;
        }
        public short[] GetSamples16() {
            return samples16;
        }
        public int[] GetSamples32() {
            return samples32;
        }
        public int GetSamplesCount() {
            switch (bits) {
                case 8: return dataLength;
                case 16: return dataLength / 2;
                case 24: return dataLength / 3;
                case 32: return dataLength / 4;
            }
            return -1;
        }
        public int GetChannels() {
            return chs;
        }
        public int GetRate() {
            return rate;
        }
        public int GetBits() {
            return bits;
        }
    }
}
