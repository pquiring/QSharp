using Qt.Core;

namespace testzip
{
    class Program
    {
        static void Main(string[] args)
        {
            ZipFile zf = new ZipFile("test.zip");
            zf.Open(ZipMode.Unzip);
            IEnumerator<ZipEntry> e = zf.GetEnumerator();
            while (e.MoveNext()) {
                ZipEntry ze = e.Current;
                Console.WriteLine("File=" + ze.Filename);
                ze.Open(OpenMode.ReadOnly);
                Console.WriteLine("Size=" + ze.GetAvailable());
                ze.Close();
            }
            Console.WriteLine("Listing complete!");
        }
    }
}
