using System;

namespace testunsafe
{
    public class Program
    {
        public unsafe static void Main(string[] args)
        {
            int[] x = new int[100];
            for(int i=0;i<99;i++) {
                x[i] = i+1;
            }
            int cnt = 0;
            fixed (int* s = x)
            {
                int* ptr = s;
                while (*ptr != 0)
                {
                    cnt+=*ptr;
                    ++ptr;
                }
            }
        }
    }
}