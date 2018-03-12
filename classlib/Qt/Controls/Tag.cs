using Qt.Core;

namespace Qt.Controls {

/** Monitors a PLC Tag.
 *
 * Auto-reconnects when disconnects.
 *
 * @author pquiring
 */

public class Tag {
    /** Host (usually IP Address) */
    public String Host;
    /** Type of host (S7, AB, MB, NI) */
    public int Type;
    /** Tag name. */
    public String Name;
    /** Size of tag. */
    public int Size;
    /** Color of tag (for reporting) */
    public int Color;
    /** int min/max values (for reporting) */
    public int Min, Max;
    /** float min/max values (for reporting) */
    public float FloatMin, FloatMax;
    /** Speed to poll data (delay = ms delay between polls) (min = 25ms) */
    public int Delay;

    private byte[] pending;
    private ThreadLock pendingLock = new ThreadLock();

    /** Get user data. */
    public Object GetData(String key) {
        return user.Get(key);
    }
    /** Set user data. */
    public void SetData(String key, Object value) {
        user.Set(key, value);
    }
    /** Set host,type,tag,Size,delay(ms). */
    public void SetTag(String host, int type, String _tag, int Size, int delay) {
        this.Host = host;
        this.Type = type;
        this.Name = _tag;
        this.Size = Size;
        this.Delay = delay;
    }

    private Controller c;
    private Timer timer;
    private Reader reader;
    private TagListener listener;
    private Map<String, Object> user = new Map<String, Object>();
    private Tag parent;
    private int childIdx;
    private ThreadSignal tlock = new ThreadSignal();
    private ArrayList<Tag> children = new ArrayList<Tag>();
    private byte[][] childData;
    private ArrayList<Tag> queue = new ArrayList<Tag>();
    private ThreadLock queueLock = new ThreadLock();
    private bool multiRead = true;

    /** Returns true if data type is float32 or float64 */
    public bool IsFloat() {
        return Size == TagType.Float32 || Size == TagType.Float64;
    }

    /** Returns true is controller is Big Endian byte order. */
    public bool IsBE() {
        switch (Type) {
            case ControllerType.JF: return false;
            case ControllerType.S7: return true;
            case ControllerType.AB: return false;
            case ControllerType.MB: return true;
            default: return true;
        }
    }

    /** Returns true is controller is Little Endian byte order. */
    public bool IsLE() {
        return !IsBE();
    }

    /** Enables reading multiple tags in one request (currently only S7 supported) */
    public void SetMultiRead(bool state) {
        if (Type != ControllerType.S7) return;
        //multiRead = state;  //TODO
    }

    /** Adds a child tag and returns index. */
    public int AddChild(Tag child) {
        children.Add(child);
        return children.Size() - 1;
    }

    private void AddQueue(Tag tag) {
        lock(queueLock) {
            queue.Add(tag);
        }
    }

    /** Returns # of bytes tag uses. */
    public int GetSize() {
        switch (Size) {
            case TagType.Bit: return 1;
            case TagType.Int8: return 1;
            case TagType.Int16: return 2;
            case TagType.Int32: return 4;
            case TagType.Float32: return 4;
            case TagType.Float64: return 8;
        }
        return 0;
    }

    public String GetURL() {
        switch (Type) {
            case ControllerType.JF: return "JF:" + Host;
            case ControllerType.S7: return "S7:" + Host;
            case ControllerType.AB: return "AB:" + Host;
            case ControllerType.MB: return "MB:" + Host;
        }
        Console.WriteLine("Tag:Error:type unknown");
        return null;
    }

    public Controller GetController() {
        if (parent != null) {
            return parent.c;
        } else {
            return c;
        }
    }

    public void SetListener(TagListener listener) {
        this.listener = listener;
    }

    public new String ToString() {
        return Name;
    }

    public String GetMin() {
        if (IsFloat()) {
            return Float.ToString(FloatMin);
        } else {
            return Int32.ToString(Min);
        }
    }

    public String GetMax() {
        if (IsFloat()) {
            return Float.ToString(FloatMax);
        } else {
            return Int32.ToString(Max);
        }
    }

    private bool startTimer() {
        if (parent == null) {
            childData = null;
            c = new Controller();
        } else {
            c = null;
        }
        timer = new Timer();
        reader = new Reader();
        reader.tag = this;
        if (Delay < 25) Delay = 25;
        timer.OnEvent(() => {reader.run();});
        timer.Start(Delay);
        return true;
    }

    /** Start reading tag at interval (delay). */
    public bool Start() {
        parent = null;
        return startTimer();
    }

    /** Start reading tag at interval (delay) using another Tags connection. */
    public bool Start(Tag parent) {
        this.parent = parent;
        if (parent != null) {
            if (parent.Type != Type) return false;
            childIdx = parent.AddChild(this);
        }
        return startTimer();
    }

    /** Stop monitoring tag value. */
    public void Stop() {
        if (timer != null) {
            timer.Stop();
            timer = null;
        }
        if (reader != null) {
            reader = null;
        }
        Disconnect();
    }

    public bool Connect() {
        if (parent != null) return false;    //wait for parent to connect
        if (c.Connect(GetURL())) return true;
        return false;
    }

    public void Disconnect() {
        if (parent != null) {
            parent = null;
            return;
        }
        if (c != null) {
            c.Disconnect();
            c = null;
        }
        children.Clear();
    }

    private String value = "0";

    /** Returns current value (only valid if start() has been called). */
    public String GetValue() {
        return value;
    }

    /** Queues pending data to be written on next cycle. (only valid if start() has been called). */
    public void SetValue(String value) {
        byte[] data = null;
        if (IsBE()) {
            switch (Size) {
                case TagType.Bit: data = new byte[] {(byte)(value.Equals("0") ? 0 : 1)}; break;
                case TagType.Int8: data = new byte[] {Byte.ValueOf(value)}; break;
                case TagType.Int16: data = new byte[2]; BE.setuint16(data, 0, Int32.ValueOf(value)); break;
                case TagType.Int32: data = new byte[4]; BE.setuint32(data, 0, Int32.ValueOf(value)); break;
                case TagType.Float32: data = new byte[4]; BE.setuint32(data, 0, Float.FloatToIntBits(Float.ValueOf(value))); break;
                case TagType.Float64: data = new byte[4]; BE.setuint64(data, 0, Double.DoubleToLongBits(Double.ValueOf(value))); break;
            }
        } else {
            switch (Size) {
                case TagType.Bit: data = new byte[] {(byte)(value.Equals("0") ? 0 : 1)}; break;
                case TagType.Int8: data = new byte[] {Byte.ValueOf(value)}; break;
                case TagType.Int16: data = new byte[2]; LE.setuint16(data, 0, Int32.ValueOf(value)); break;
                case TagType.Int32: data = new byte[4]; LE.setuint32(data, 0, Int32.ValueOf(value)); break;
                case TagType.Float32: data = new byte[4]; LE.setuint32(data, 0, Float.FloatToIntBits(Float.ValueOf(value))); break;
                case TagType.Float64: data = new byte[4]; LE.setuint64(data, 0, Double.DoubleToLongBits(Double.ValueOf(value))); break;
            }
        }
        lock(pendingLock) {
            pending = data;
        }
    }

    /** Returns current value as int (only valid if start() has been called). */
    public int IntValue() {
        return Int32.ValueOf(value);
    }

    /** Returns current value as float (only valid if start() has been called). */
    public float FloatValue() {
        return Float.ValueOf(value);
    }

    /** Returns current value as double (float64) (only valid if start() has been called). */
    public double DoubleValue() {
        return Double.ValueOf(value);
    }

    /** Reads value directly. */
    public byte[] Read() {
        if (parent != null) {
            if (parent.c == null) return null;
            if (multiRead) {
                return parent.Read(childIdx);
            } else {
                //queue read with parent to prevent some threads from starving
                lock(tlock) {
                    parent.AddQueue(this);
                    try {tlock.Wait();} catch (Exception e) {Console.WriteLine(e.ToString());}
                    return parent.c.Read(Name);
                }
            }
        } else {
            if (multiRead && Type == ControllerType.S7 && children.Size() > 0) {
                int cnt = children.Size();
                String[] tags = new String[cnt+1];
                tags[cnt] = Name;
                for(int a=0;a<cnt;a++) {
                    tags[a] = children.Get(a).Name;
                }
                childData = c.Read(tags);
                if (childData == null) return null;
                return childData[cnt];
            } else {
                //allow queued children to proceed
                lock(queueLock) {
                    while (queue.Size() > 0) {
                        Tag child = queue.Get(0);
                        queue.RemoveAt(0);
                        lock(child.tlock) {
                            child.tlock.WakeOne();
                        }
                    }
                }
                return c.Read(Name);
            }
        }
    }

    /** Writes data to tag. */
    public void Write(byte[] data) {
        if (parent != null) {
            if (parent.c == null) return;
            parent.c.Write(Name, data);
        } else {
            c.Write(Name, data);
        }
    }

    private byte[] Read(int idx) {
        if (childData == null || idx >= childData.Length) return null;
        return childData[idx];
    }

    private class Reader {
        public Tag tag;
        public byte[] data;
        public void run() {
            try {
                String lastValue = tag.value;
                if (tag.parent == null) {
                    if (!tag.c.IsConnected()) {
                        if (!tag.Connect()) {
                            return;
                        }
                    }
                }
                data = tag.Read();
                if (data == null) {
                    Console.WriteLine("Error:" + DateTime.GetMilliSecondsSinceEpoch() + ":data==null:host=" + tag.Host + ":tag=" + tag.Name);
                    return;
                }
                if (tag.IsBE()) {
                    switch (tag.Size) {
                        case TagType.Bit: tag.value = data[0] == 0 ? "0" : "1"; break;
                        case TagType.Int8: tag.value = Byte.ToString(data[0]); break;
                        case TagType.Int16: tag.value = Int32.ToString(BE.getuint16(data, 0)); break;
                        case TagType.Int32: tag.value = Int32.ToString(BE.getuint32(data, 0)); break;
                        case TagType.Float32: tag.value = Float.ToString(Float.IntBitsToFloat(BE.getuint32(data, 0))); break;
                        case TagType.Float64: tag.value = Double.ToString(Double.LongBitsToDouble(BE.getuint64(data, 0))); break;
                    }
                } else {
                    switch (tag.Size) {
                        case TagType.Bit: tag.value = data[0] == 0 ? "0" : "1"; break;
                        case TagType.Int8: tag.value = Byte.ToString(data[0]); break;
                        case TagType.Int16: tag.value = Int32.ToString((short)LE.getuint16(data, 0)); break;
                        case TagType.Int32: tag.value = Int32.ToString(LE.getuint32(data, 0)); break;
                        case TagType.Float32: tag.value = Float.ToString(Float.IntBitsToFloat(LE.getuint32(data, 0))); break;
                        case TagType.Float64: tag.value = Double.ToString(Double.LongBitsToDouble(LE.getuint64(data, 0))); break;
                    }
                }
                lock(tag.pendingLock) {
                    if (tag.pending != null) {
                        tag.Write(tag.pending);
                    }
                }
                if (tag.listener == null) return;
                if (lastValue == null || !tag.value.Equals(lastValue)) {
                    tag.listener.TagChanged(tag, tag.value);
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

}
