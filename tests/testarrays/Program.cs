using Qt.Core;

namespace testarrays
{
    public class A : Object {}
    public class B : A {}
    public class Program
    {
        public static void func(Object[][] array) {
            Console.WriteLine("array.Length=" + array.Length);
        }
        public static int sum(int[] array) {
            int sum = 0;
            for(int a=0;a<array.Length;a++) {
                sum += array[a];
            }
            return sum;
        }
        public static void Main(string[] args)
        {
            int[][] i2 = new int[4][];
            for(int a=0;a<4;a++) {
                i2[a] = new int[4];
            }
            for(int a=0;a<4;a++) {
                i2[3][a] = a;
            }
            Console.WriteLine("sum=" + sum(i2[3]));
            i2[0] = new int[]{1,2,3};
            i2 = new int[][] {new int[]{1,2,3}, new int[] {4,5,6}};

            B[][] b2 = new B[4][];
            func(b2);
            Console.WriteLine("Ok");
        }
    }
}
