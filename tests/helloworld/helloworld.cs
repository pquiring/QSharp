using Qt.Core;

class Base {
}

interface I1 {
}


class MainClass : Base, I1 {
  public static void Main(string[] args) {
    for(int a=0;a<args.Length;a++) {
  	  Console.WriteLine(args[a]);
    }
    Console.WriteLine("Hello, world!");
  }
}
