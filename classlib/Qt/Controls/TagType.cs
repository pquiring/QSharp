namespace Qt.Controls {

/** Tag Types (0x00 - 0xff)
 *
 * @author pquiring
 */

public class TagType {
    public const int Unknown = 0;
    public const int Bit = 1;
    public const int Int8 = 2;
    public const int Int16 = 3;
    public const int Int32 = 4;
    public const int Int64 = 5;

    public const int Float32 = 8;
    public const int Float64 = 9;

    public const int Char8 = 16;
    public const int Char16 = 17;
    public const int String = 18;    //UTF-8

    public const int Function = 32;

    public const int Any = 64;
    public const int AnyInt = 65;
    public const int AnyFloat = 66;
    public const int AnyArray = 67;
}

}
