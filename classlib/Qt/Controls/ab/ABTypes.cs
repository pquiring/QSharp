namespace Qt.Controls.ab {

/**

    Allen Bradley data types

    see https://github.com/pjkundert/cpppo/blob/c627406e821f81a76f1562655b72d37d089df410/server/enip/parser.py

 */

public class ABTypes {
    public const byte BOOL = (byte)0xc1;
    public const byte BYTE = (byte)0xc2;
    public const byte INT = (byte)0xc3;
    public const byte DINT = (byte)0xc4;
    public const byte LINT = (byte)0xc4;  //long
    public const byte UBYTE = (byte)0xc6;
    public const byte WORD = (byte)0xc7;
    public const byte DWORD = (byte)0xc8;
    public const byte REAL = (byte)0xca;
    public const byte BOOLx16 = (byte)0xd2;  //16bit bool array
    public const byte BOOLx32 = (byte)0xd3;  //32bit bool array
    public const byte STRING = (byte)0xd0;  //2 byte length + data (rounded up to 2 bytes)
    public const byte STRING1 = (byte)0xda;  //1 byte length + data

}

}
