using Qt.Core;

namespace Qt.Controls.s7 {

/** S7 Data Packet
 *
 * Reference : snap7.sf.net
 *
 * @author pquiring
 */

public class S7Packet {

    /** Creates a packet to connect at COTP level (connect step1). */
    public static byte[] makeConnectPacket1() {
        TPKT tpkt = new TPKT();
        COTP cotp = new COTP(COTP.type_connect);
        byte[] data;

        int size = tpkt.size() + cotp.size();
        data = new byte[size];
        int dataoff = 0;
        tpkt.write(data, dataoff, (short)size);
        dataoff += tpkt.size();
        cotp.write(data, dataoff);
        return data;
    }

    /** Creates a packet to connect at S7 level (connect step2). */
    public static byte[] makeConnectPacket2() {
        TPKT tpkt = new TPKT();
        COTP cotp = new COTP(COTP.type_data);
        S7Header header = new S7Header();
        S7Params _params = new S7Params();
        byte[] data;

        _params.makeConnect();
        int size = tpkt.size() + cotp.size() + header.size() + _params.size();
        data = new byte[size];
        int dataoff = 0;
        tpkt.write(data, dataoff, (short)size);
        dataoff += tpkt.size();
        cotp.write(data, dataoff);
        dataoff += cotp.size();
        header.write(data, dataoff, (short)_params.size(), (short)0);
        dataoff += header.size();
        _params.write(data, dataoff);
        return data;
    }

    /** Creates a packet to read data from S7. */
    public static byte[] makeReadPacket(S7Data s7) {
        TPKT tpkt = new TPKT();
        COTP cotp = new COTP(COTP.type_data);
        S7Header header = new S7Header();
        S7Params _params = new S7Params();
        byte[] data;

        _params.makeRead(s7);
        int size = tpkt.size() + cotp.size() + header.size() + _params.size();
        data = new byte[size];
        int dataoff = 0;
        tpkt.write(data, dataoff, (short)size);
        dataoff += tpkt.size();
        cotp.write(data, dataoff);
        dataoff += cotp.size();
        header.write(data, dataoff, (short)_params.size(), (short)0);
        dataoff += header.size();
        _params.write(data, dataoff);
        return data;
    }

    /** Creates a packet to read data from S7. */
    public static byte[] makeReadPacket(S7Data[] s7) {
        TPKT tpkt = new TPKT();
        COTP cotp = new COTP(COTP.type_data);
        S7Header header = new S7Header();
        S7Params _params = new S7Params();
        byte[] data;

        _params.makeRead(s7);
        int size = tpkt.size() + cotp.size() + header.size() + _params.size();
        data = new byte[size];
        int dataoff = 0;
        tpkt.write(data, dataoff, (short)size);
        dataoff += tpkt.size();
        cotp.write(data, dataoff);
        dataoff += cotp.size();
        header.write(data, dataoff, (short)_params.size(), (short)0);
        dataoff += header.size();
        _params.write(data, dataoff);
        return data;
    }

    /** Creates a packet to write data to S7. */
    public static byte[] makeWritePacket(S7Data type) {
        TPKT tpkt = new TPKT();
        COTP cotp = new COTP(COTP.type_data);
        S7Header header = new S7Header();
        S7Params _params = new S7Params();
        byte[] data;

        _params.makeWrite(type.block_type, type.block_number, type.data_type, type.offset, type.length, type.data);
        int size = tpkt.size() + cotp.size() + header.size() + _params.size();
        data = new byte[size];
        int dataoff = 0;
        tpkt.write(data, dataoff, (short)size);
        dataoff += tpkt.size();
        cotp.write(data, dataoff);
        dataoff += cotp.size();
        header.write(data, dataoff, (short)(_params.size() - 4 - type.length), (short)(4 + type.length));
        dataoff += header.size();
        _params.write(data, dataoff);
        return data;
    }

    /** Decodes S7 Address.
     *
     * Supports: DB,M,I,Q
     *
     * Does not support ranges yet.
     */
    public static S7Data decodeAddress(String addr) {
        //DB##.DB?##[.#]
        //M[?]##[.#]
        //I[?]##[.#]
        //Q[?]##[.#]
        S7Data data = new S7Data();
        int idx;
        if (addr.StartsWith("DB")) {
            data.block_type = S7Types.DB;
            idx = addr.IndexOf('.');    //.DB?##[.#]
            data.block_number = (short)Int32.ValueOf(addr.SubstringIdx(2, idx));
            addr = addr.Substring(idx+2);    //B?##[.#]
        } else if (addr.StartsWith("M")) {
            data.block_type = S7Types.M;
        } else if (addr.StartsWith("I")) {
            data.block_type = S7Types.I;
        } else if (addr.StartsWith("Q")) {
            data.block_type = S7Types.Q;
        } else {
            return null;
        }
        data.data_type = S7Types.getType(addr.CharAt(1));
        short offset;
        idx = addr.IndexOf('.');
        if (idx == -1) idx = addr.IndexOf(' ');
        if (idx == -1) idx = addr.Length;
        if (data.data_type == 0) {
            //no type present (assume bit)
            offset = (short)Int32.ValueOf(addr.SubstringIdx(1, idx));
            data.data_type = S7Types.BIT;
        } else {
            offset = (short)Int32.ValueOf(addr.SubstringIdx(2, idx));
        }
        data.offset = (short)(offset << 3);
        if (data.data_type == S7Types.BIT) {
            int idx2 = addr.IndexOf(' ');
            if (idx2 == -1) idx2 = addr.Length;
            byte bit = Byte.ValueOf(addr.SubstringIdx(idx+1, idx2));
            data.offset += bit;
        }
        idx = addr.IndexOf(" BYTE ");
        if (idx == -1) {
            //S7Types.getTypeSize(data.data_type, (short)1);
            data.length = 1;
        } else {
            data.length = (short)Int32.ValueOf(addr.Substring(idx+6));
        }
        return data;
    }

    /** Decodes a packet and returns any data returned. */
    public static S7Data decodePacket(byte[] packet) {
        try {
            S7Data data = new S7Data();
            int offset = 0;
            TPKT tpkt = new TPKT();
            tpkt.read(packet, offset);
            offset += tpkt.size();
            COTP cotp = new COTP();
            cotp.read(packet, offset);
            if (cotp.PDU_type == COTP.type_connect) return data;
            if (cotp.PDU_type == COTP.type_connect_ack) return data;
            offset += cotp.size();
            S7Header header = new S7Header();
            header.read(packet, offset);
            offset += header.size();
            S7Params _params = new S7Params();
            _params.read(packet, offset, data);
            return data;
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
        return null;
    }

    /** Decodes a packet and returns any data returned. */
    public static S7Data[] decodeMultiPacket(byte[] packet, int count) {
        try {
            S7Data[] data = new S7Data[count];
            for(int a=0;a<count;a++) {
                data[a] = new S7Data();
            }
            int offset = 0;
            TPKT tpkt = new TPKT();
            tpkt.read(packet, offset);
            offset += tpkt.size();
            COTP cotp = new COTP();
            cotp.read(packet, offset);
            if (cotp.PDU_type == COTP.type_connect) return data;
            if (cotp.PDU_type == COTP.type_connect_ack) return data;
            offset += cotp.size();
            S7Header header = new S7Header();
            header.read(packet, offset);
            offset += header.size();
            S7Params _params = new S7Params();
            _params.read(packet, offset, data);
            return data;
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
        return null;
    }

    public static bool isPacketComplete(byte[] packet) {
        return decodePacket(packet) != null;
    }
}

}
