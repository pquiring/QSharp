namespace Qt.Controls {

/** Tag Types (0x00 - 0xff)
 *
 * @author pquiring
 */

public class TagType {
    public const int unknown = 0;
    public const int bit = 1;
    public const int int8 = 2;
    public const int int16 = 3;
    public const int int32 = 4;
    public const int int64 = 5;

    public const int float32 = 8;
    public const int float64 = 9;

    public const int char8 = 16;
    public const int char16 = 17;
    public const int String = 18;    //UTF-8

    public const int function = 32;

    public const int any = 64;
    public const int anyint = 65;
    public const int anyfloat = 66;
    public const int anyarray = 67;
}

}
