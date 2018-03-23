using Qt.Core;

namespace Qt.Controls.jfc {

/** jfControls Packet
 *
 * @author pquiring
 */

public class JFPacket {
    public static JFTag decodeAddress(String name) {
        return new JFTag(name);
    }

    public static byte[] makeWritePacket(JFTag tag, byte[] tagdata) {
        int strlen = tag.tag.Length;
        byte[] data = new byte[8 + 2 + 1 + strlen + 2 + 2 + tagdata.Length];
        LE.setuint16(data, 0, 0x0004);    //write tag cmd
        LE.setuint16(data, 2, 0x1234);    //unique id
        LE.setuint32(data, 4, 2 + strlen);    //data Length
        LE.setuint16(data, 8, 1);    //count
        LE.setuint8(data, 10, strlen);    //strlen
        Arrays<byte>.Copy(tag.tag.ToByteArray(), 0, data, 11, strlen);
        int pos = 11 + strlen;
        LE.setuint16(data, pos, tag.type);    //tagtype
        pos += 2;
        LE.setuint16(data, pos, tagdata.Length);    //tag size
        pos += 2;
        Arrays<byte>.Copy(tagdata, 0, data, pos, tagdata.Length);
        return data;
    }

    public static byte[] makeReadPacket(JFTag tag) {
        int strlen = tag.tag.Length;
        byte[] data = new byte[8 + 2 + 1 + strlen];
        LE.setuint16(data, 0, 0x0003);    //read tag cmd
        LE.setuint16(data, 2, 0x1234);    //unique id
        LE.setuint32(data, 4, 2 + strlen);    //data Length
        LE.setuint16(data, 8, 1);    //count
        LE.setuint8(data, 10, strlen);    //strlen
        Arrays<byte>.Copy(tag.tag.ToByteArray(), 0, data, 11, strlen);
        return data;
    }

    public static byte[] makeReadPacket(JFTag[] tags) {
        int size = 8 + 2;
        for(int a=0;a<tags.Length;a++) {
            int strlen = tags[a].tag.Length;
            size += 1 + strlen;
        }
        byte[] data = new byte[size];
        LE.setuint16(data, 0, 0x0003);    //read tag cmd
        LE.setuint16(data, 2, 0x1234);    //unique id
        LE.setuint32(data, 4, size - 8);    //data Length
        LE.setuint16(data, 8, tags.Length);    //count
        int pos = 12;
        for(int a=0;a<tags.Length;a++) {
            int strlen = tags[a].tag.Length;
            LE.setuint8(data, pos, strlen);    //strlen
            pos++;
            Arrays<byte>.Copy(tags[a].tag.ToByteArray(), 0, data, pos, strlen);
            pos += strlen;
        }
        return data;
    }

    public static byte[] makeWritePacket(JFTag[] tags, byte[][] tagdata) {
        int size = 8 + 2;
        for(int a=0;a<tags.Length;a++) {
            int strlen = tags[a].tag.Length;
            size += 1 + strlen;
            size += tagdata[a].Length;
        }
        byte[] data = new byte[size];
        LE.setuint16(data, 0, 0x0004);    //write tag cmd
        LE.setuint16(data, 2, 0x1234);    //unique id
        LE.setuint32(data, 4, size - 8);    //data Length
        LE.setuint16(data, 8, tags.Length);    //count
        int pos = 12;
        for(int a=0;a<tags.Length;a++) {
            int strlen = tags[a].tag.Length;
            LE.setuint8(data, pos, strlen);    //strlen
            pos++;
            Arrays<byte>.Copy(tags[a].tag.ToByteArray(), 0, data, pos, strlen);
            pos += strlen;
            LE.setuint16(data, pos, tags[a].type);    //tag type
            pos += 2;
            LE.setuint16(data, pos, tagdata[a].Length);    //tag size
            pos += 2;
            Arrays<byte>.Copy(tagdata[a], 0, data, pos, tagdata[a].Length);
            pos += tagdata[a].Length;
        }
        return data;
    }

    public static bool isPacketComplete(byte[] data) {
        if (data.Length < 8) return false;
        //int cmd = LE.getuint16(data, 0);
        //int id = LE.getuint16(data, 2);
        int len = LE.getuint32(data, 4);
        return (data.Length >= 8 + len);
    }

    public static JFTag decodePacket(byte[] data) {
        int cmd = LE.getuint16(data, 0);
        int id = LE.getuint16(data, 2);
        int len = LE.getuint32(data, 4);
        int cnt = LE.getuint16(data, 8);
        if (cnt != 1) return null;
        int type = LE.getuint16(data, 10);
        int size = LE.getuint16(data, 12);
        int datalen = len - 6;
        JFTag tag = new JFTag(null);
        tag.data = Arrays<byte>.CopyOf(data, 12, 12 + datalen);
        return tag;
    }

    public static JFTag[] decodeMultiPacket(byte[] data, int tagcnt) {
        int cmd = LE.getuint16(data, 0);
        int id = LE.getuint16(data, 2);
        int len = LE.getuint32(data, 4);
        int cnt = LE.getuint16(data, 8);
        if (cnt != tagcnt) return null;
        JFTag[] tags = new JFTag[tagcnt];
        int pos = 10;
        for(int a=0;a<tagcnt;a++) {
            tags[a] = new JFTag(null);
            int type = LE.getuint16(data, pos);
            pos += 2;
            int datalen = LE.getuint16(data, pos);
            pos += 2;
            JFTag tag = new JFTag(null);
            tag.data = Arrays<byte>.CopyOf(data, pos, pos + datalen);
            pos += datalen;
        }
        return tags;
    }
}

}
