using Qt.Core;

class MainClass : Object {
    public static void Main(string[] args) {
        for(int a=0;a<args.Length;a++) {
            Console.WriteLine(args[a]);
        }
        Console.WriteLine("Hello, world!");
    }
}
