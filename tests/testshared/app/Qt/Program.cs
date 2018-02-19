using Qt.Core;

namespace Qt
{
    public class Program
    {
        public static void Main(string[] args)
        {
        	Library lib = new Library("dll.dll");
        	if (!lib.Load()) {
        		Console.WriteLine("Load() failed!");
        		return;
        	}
            LibraryMain main = lib.GetLibraryMain();
            if (main == null) {
        		Console.WriteLine("GetLibraryMain() failed!");
        		return;
            }
            main(null);
        }
    }
}
