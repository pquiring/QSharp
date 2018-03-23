using Qt.Core;
using Qt.Network;

using Qt.Controls.jfc;
using Qt.Controls.ab;
using Qt.Controls.s7;
using Qt.Controls.mod;

namespace Qt.Controls {

/**
 * Controller
 *
 * Connects to PLCs and read/write data.
 *
 * @author pquiring
 */

public class Controller {
    private bool connected;
    private TcpSocket socket;
    private int plc;
    private ThreadLock tlock = new ThreadLock();    //read/write tlock
    private static ThreadLock s7_connect_lock = new ThreadLock();

    private ABContext ab_context;

    public static double rate;    //sample rate for all controllers (set before connecting to any controllers)

    public Exception lastException;

    public void SetRate(float rate) {
        Controller.rate = rate;
    }

    /** Connects to a PLC:
     *
     * url = "S7:host"
     * url = "MODBUS:host"
     * url = "AB:host"
     * url = "NI:device/options"
     *
     */
    public bool Connect(String url) {
        Console.WriteLine("Info:" + DateTime.GetMilliSecondsSinceEpoch() + ":Controller.connect():" + url);
        connected = false;
        if (url == null) {
            Console.WriteLine("Error:url == null");
            return false;
        }
        if (url.StartsWith("S7:")) {
            plc = ControllerType.S7;
            String host = url.Substring(3);
            lock(s7_connect_lock) {
                try {
                    socket = new TcpSocket();
//                    socket.setSoTimeout(3000);
                    socket.Connect(host, 102);

                    //connect1
                    {
                        byte[] packet = S7Packet.makeConnectPacket1();
                        socket.Write(packet);

                        byte[] reply = new byte[1500];
                        int replySize = 0;
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!S7Packet.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    }

                    //connect2
                    {
                        byte[] packet = S7Packet.makeConnectPacket2();
                        socket.Write(packet);

                        byte[] reply = new byte[1500];
                        int replySize = 0;
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!S7Packet.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    }

                } catch (Exception e) {
                    Console.WriteLine("Error:" + e.ToString());
                    return false;
                }
            }
            connected = true;
            return true;
        }
        if (url.StartsWith("MODBUS:")) {
            plc = ControllerType.MB;
            String host = url.Substring(7);
            try {
                socket = new TcpSocket();
                socket.Connect(host, 502);
//                socket.setSoTimeout(3000);
            } catch (Exception e) {
                Console.WriteLine("Error:" + e.ToString());
                return false;
            }
            connected = true;
            return true;
        }
        if (url.StartsWith("AB:")) {
            ab_context = new ABContext();
            plc = ControllerType.AB;
            String host = url.Substring(3);
            try {
                socket = new TcpSocket();
                socket.Connect(host, 44818);
//                socket.setSoTimeout(3000);

                //connect1
                {
                    byte[] packet = ABPacket.makeConnectPacket(ab_context);
                    socket.Write(packet);

                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    do {
                        int read = socket.Read(reply, replySize, 1500 - replySize);
                        if (read == -1) throw new Exception("bad read");
                        replySize += read;
                    } while (!ABPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    ENIP ip = new ENIP();
                    ip.read(reply, 0);
                    ab_context.session = ip.session;
                }

            } catch (Exception e) {
                Console.WriteLine("Error:" + e.ToString());
                return false;
            }
            connected = true;
            return true;
        }
        return false;
    }

    /** Disconnects from PLC. */
    public bool Disconnect() {
        if (!connected) return false;
        switch (plc) {
            case ControllerType.JF:
            case ControllerType.S7:
            case ControllerType.MB:
            case ControllerType.AB:
                try {
                    if (socket != null) {
                        socket.Close();
                        socket = null;
                    }
                } catch (Exception e) {
                    Console.WriteLine("Error:" + e.ToString());
                    return false;
                }
                break;
        }
        connected = false;
        return true;
    }

    /** Writes data to PLC. */
    public bool Write(String addr, byte[] data) {
        return Write(addr, data, DataType.ANY);
    }

    /** Writes data to PLC.
     *
     * datatype is required for AB controllers.
     */
    public bool Write(String addr, byte[] data, DataType type) {
        addr = addr.ToUpperCase();
        lock(tlock) {
            if (!connected) return false;
            switch (plc) {
                case ControllerType.S7: {
                    S7Data s7 = S7Packet.decodeAddress(addr);
                    s7.data = data;
                    byte[] packet = S7Packet.makeWritePacket(s7);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return false;
                    }
                    return true;
                }
                case ControllerType.MB: {
                    ModAddr ma = ModPacket.decodeAddress(addr);
                    ma.state = data[0] != 0;
                    byte[] packet = ModPacket.makeWritePacket(ma);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return false;
                    }
                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    try {
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!ModPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    } catch (Exception e) {
                        lastException = e;
                        return false;
                    }
                    return true;
                }
                case ControllerType.AB: {
                    if (type == DataType.ANY) return false;
                    byte[] packet = ABPacket.makeWritePacket(addr, ABPacket.getType(type), data, ab_context);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return false;
                    }
                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    try {
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!ABPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    } catch (Exception e) {
                        lastException = e;
                        return false;
                    }
                    return true;
                }
                case ControllerType.JF: {
                    JFTag tag = JFPacket.decodeAddress(addr);
                    tag.data = data;
                    byte[] packet = JFPacket.makeWritePacket(tag, data);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return false;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    /** Reads data from PLC. */
    public byte[] Read(String addr) {
        addr = addr.ToUpperCase();
        lock(tlock) {
            if (!connected) return null;
            switch (plc) {
                case ControllerType.S7: {
                    S7Data s7 = S7Packet.decodeAddress(addr);
                    if (s7 == null) return null;
                    byte[] packet = S7Packet.makeReadPacket(s7);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    try {
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!S7Packet.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    s7 = S7Packet.decodePacket(Arrays<byte>.CopyOf(reply, replySize));
                    return s7.data;
                }
                case ControllerType.MB: {
                    ModAddr ma = ModPacket.decodeAddress(addr);
                    byte[] packet = ModPacket.makeReadPacket(ma);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    try {
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!ModPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    ModData data = ModPacket.decodePacket(Arrays<byte>.CopyOf(reply, replySize));
                    return data.data;
                }
                case ControllerType.AB: {
                    byte[] packet = ABPacket.makeReadPacket(addr, ab_context);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    try {
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!ABPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                        return ABPacket.decodePacket(reply);
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                }
                case ControllerType.JF: {
                    JFTag tag = JFPacket.decodeAddress(addr);
                    if (tag == null) return null;
                    byte[] packet = JFPacket.makeReadPacket(tag);
                    try {
                        socket.Write(packet);
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    byte[] reply = new byte[1500];
                    int replySize = 0;
                    try {
                        do {
                            int read = socket.Read(reply, replySize, 1500 - replySize);
                            if (read == -1) throw new Exception("bad read");
                            replySize += read;
                        } while (!JFPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    } catch (Exception e) {
                        lastException = e;
                        return null;
                    }
                    tag = JFPacket.decodePacket(Arrays<byte>.CopyOf(reply, replySize));
                    return tag.data;
                }
            }
        }
        return null;
    }

    /** Reads multiple data tags from PLC. (only S7 is currently supported) */
    public byte[][] Read(String[] addr) {
        if (!connected) return null;
        for(int a=0;a<addr.Length;a++) {
            addr[a] = addr[a].ToUpperCase();
        }
        switch (plc) {
            case ControllerType.S7: {
                S7Data[] s7 = new S7Data[addr.Length];
                for(int a=0;a<addr.Length;a++) {
                    s7[a] = S7Packet.decodeAddress(addr[a]);
                }
                byte[] packet = S7Packet.makeReadPacket(s7);
                try {
                    socket.Write(packet);
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                byte[] reply = new byte[1500];
                int replySize = 0;
                try {
                    do {
                        int read = socket.Read(reply, replySize, 1500 - replySize);
                        if (read == -1) throw new Exception("bad read");
                        replySize += read;
                    } while (!S7Packet.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                s7 = S7Packet.decodeMultiPacket(Arrays<byte>.CopyOf(reply, replySize), addr.Length);
                byte[][] ret = new byte[addr.Length][];
                for(int a=0;a<addr.Length;a++) {
                    ret[a] = s7[a].data;
                }
                return ret;
            }
/*
            case ControllerType.MODBUS: {
                ModAddr ma = ModPacket.decodeAddress(addr);
                byte[] packet = ModPacket.makeReadPacket(ma);
                try {
                    socket.Write(packet);
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                byte[] reply = new byte[1500];
                int replySize = 0;
                try {
                    do {
                        int read = socket.Read(reply, replySize, 1500 - replySize);
                        if (read == -1) throw new Exception("bad read");
                        replySize += read;
                    } while (!ModPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                ModData data = ModPacket.decodePacket(Arrays<byte>.CopyOf(reply, replySize));
                return data.data;
            }
            case AB: {
                byte[] packet = ABPacket.makeReadPacket(addr, ab_context);
                try {
                    socket.Write(packet);
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                byte[] reply = new byte[1500];
                int replySize = 0;
                try {
                    do {
                        int read = socket.Read(reply, replySize, 1500 - replySize);
                        if (read == -1) throw new Exception("bad read");
                        replySize += read;
                    } while (!ABPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                    return ABPacket.decodePacket(reply);
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
            }
*/
            case ControllerType.JF: {
                JFTag[] tags = new JFTag[addr.Length];
                for(int a=0;a<addr.Length;a++) {
                    tags[a] = JFPacket.decodeAddress(addr[a]);
                }
                byte[] packet = JFPacket.makeReadPacket(tags);
                try {
                    socket.Write(packet);
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                byte[] reply = new byte[1500];
                int replySize = 0;
                try {
                    do {
                        int read = socket.Read(reply, replySize, 1500 - replySize);
                        if (read == -1) throw new Exception("bad read");
                        replySize += read;
                    } while (!JFPacket.isPacketComplete(Arrays<byte>.CopyOf(reply, replySize)));
                } catch (Exception e) {
                    lastException = e;
                    return null;
                }
                tags = JFPacket.decodeMultiPacket(Arrays<byte>.CopyOf(reply, replySize), addr.Length);
                byte[][] ret = new byte[addr.Length][];
                for(int a=0;a<addr.Length;a++) {
                    ret[a] = tags[a].data;
                }
                return ret;
            }
        }
        return null;
    }

    public bool IsConnected() {
        if (plc == 0) return false;
        try {
            switch (plc) {
                case ControllerType.JF:
                case ControllerType.S7:
                case ControllerType.AB:
                case ControllerType.MB:
                default:
                    return connected;
            }
        } catch (Exception e) {
            Console.WriteLine("Error:" + e.ToString());
            return false;
        }
    }
}

}
