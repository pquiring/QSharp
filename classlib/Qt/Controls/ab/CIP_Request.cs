using Qt.Core;

namespace Qt.Controls.ab {

/** CIP : Connection Manager : Request
 *
 * @author pquiring
 */

public class CIP_Request {
    //CIP header
    public byte cmd = (byte)0x52;
    public byte count = 2;
    public byte path_1 = 0x20;    //8bit class segment
    public byte class_1 = 0x06;    //connection manager
    public byte path_2 = 0x24;    //8bit instance segment
    public byte class_2 = 0x01;    //instance

    //connection manager header
    public byte ticktime = 0x07;
    public byte ticktimeout = (byte)0xf9;
    public short len;    //size of following {} in bytes
    //{
        public byte service;
        public byte tag_size;    //size of following {} in 16bit words
        //{
            public byte tag_type = (byte)0x91;    //ascii tag
            public byte tag_len;
            public byte[] tag_chars;
            //1 byte padding if tag_type/tag_len/tag is not multiple of 16bit words
        //}
        public byte[] tagdata;
    //}
    public byte route_size = 0x01;    //size of following {} in 16bit words
    public byte route_res = 0x00;    //reserved
    //{
        public byte route_seg = 0x01;
        public byte route_addr = 0x00;
    //}

    public const byte SERVICE_READTAG = 0x4c;
    public const byte SERVICE_WRITETAG = 0x4d;

    public int size() {
        int size = 18 + tag_chars.Length + tagdata.Length;
        if (tag_chars.Length % 2 != 0) {
            size++;    //padding
        }
        return size;
    }

    public void write(byte[] data, int offset) {
        data[offset++] = cmd;
        data[offset++] = count;
        data[offset++] = path_1;
        data[offset++] = class_1;
        data[offset++] = path_2;
        data[offset++] = class_2;
        data[offset++] = ticktime;
        data[offset++] = ticktimeout;
        LE.setuint16(data, offset, len); offset += 2;
        data[offset++] = service;
        data[offset++] = tag_size;
        data[offset++] = tag_type;
        data[offset++] = tag_len;
        Arrays.Copy(tag_chars, 0, data, offset, tag_chars.Length); offset += tag_chars.Length;
        if (tag_chars.Length % 2 != 0) {
            data[offset++] = 0;    //padding
        }
        Arrays.Copy(tagdata, 0, data, offset, tagdata.Length); offset += tagdata.Length;
        data[offset++] = route_size;
        data[offset++] = route_res;
        data[offset++] = route_seg;
        data[offset++] = route_addr;
    }

    public void setRead(String tag) {
        service = SERVICE_READTAG;
        tag_len = (byte)tag.Length;
        tag_chars = tag.ToByteArray();
        tagdata = new byte[] {0x01, 0x00};
        setLengths();
    }

    public void setWrite(String tag, byte type, byte[] data) {
        service = SERVICE_WRITETAG;
        tag_len = (byte)tag.Length;
        tag_chars = tag.ToByteArray();
        tagdata = new byte[4 + data.Length];
        tagdata[0] = type;
        tagdata[1] = 0;
        tagdata[2] = 1;
        tagdata[3] = 0;
        Arrays.Copy(data, 0, tagdata, 4, data.Length);
        setLengths();
    }

    private void setLengths() {
        len = (short)(4 + tag_chars.Length + tagdata.Length);
        if (tag_chars.Length % 2 != 0) {
            len++;    //padding
        }
        tag_size = (byte)((2 + tag_chars.Length + 1) >> 1);
    }
}

}
