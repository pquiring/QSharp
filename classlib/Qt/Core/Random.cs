namespace Qt.Core {
    /** Random number generator. */
    public class Random {
        //NOTE : The QRandomGenerator (QT 5.10) does not support 64bit seeds, so I 'borrowed' code from Java.
        private const long multiplier = 0x5DEECE66DL;
        private const long addend = 0xBL;
        private const long mask = (1L << 48) - 1;
        private static long seedUniquifier = 8682522807148012L;
        private long seed;

        public Random() {
            SetSeed(DateTime.GetMilliSecondsSinceEpoch() + seedUniquifier++);
        }
        public Random(long seed) {
            SetSeed(seed);
        }
        public void SetSeed(long seed) {
            seed = (seed ^ multiplier) & mask;
            this.seed = seed;
        }
        private int next(int bits) {
            long nextseed;
            nextseed = (seed * multiplier + addend) & mask;
            return (int)(nextseed >> (48 - bits));
        }
        public int NextInt() {
            return next(32);
        }
        public int NextInt(int bound) {
            if (bound <= 0) return -1;  //throw new IllegalArgumentException("bound must be positive");

            if ((bound & -bound) == bound)  // bound is a power of 2
                return (int)((bound * (long)next(31)) >> 31);

            int bits, val;
            do {
                bits = next(31);
                val = bits % bound;
            } while (bits - val + (bound-1) < 0);
            return val;
        }
        public long NextLong() {
            return ((long)next(32) << 32) + next(32);
        }
        public float NextFloat() {
            return next(24) / ((float)(1 << 24));
        }
        public double NextDouble() {
            return ((((long)(next(26))) << 27) + next(27)) / (double)(1L << 53);
        }
    }
}
