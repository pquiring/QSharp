using Qt.QSharp;

namespace Qt.Core {
    public class Math {
        public const double PI = 3.14159265358979323846;
        public const double E = 2.7182818284590452354;
        public const double LOG2E = 1.4426950408889634074;
        public const double LOG10E = 0.43429448190325182765;
        public const double LN2 = 0.69314718055994530942;
        public const double LN10 = 2.30258509299404568402;

        public static double Sin(double x) {
            return CPP.ReturnDouble("qSin(x)");
        }
        public static double Cos(double x) {
            return CPP.ReturnDouble("qCos(x)");
        }
        public static double Tan(double x) {
            return CPP.ReturnDouble("qTan(x)");
        }
        public static double Asin(double x) {
            return CPP.ReturnDouble("qAsin(x)");
        }
        public static double Acos(double x) {
            return CPP.ReturnDouble("qAcos(x)");
        }
        public static double Atan(double x) {
            return CPP.ReturnDouble("qAtan(x)");
        }
        public static double Sqrt(double x) {
            return CPP.ReturnDouble("qSqrt(x)");
        }
        public static double Pow(double x, double y) {
            return CPP.ReturnDouble("qPow(x, y)");
        }
        public static double Abs(double x) {
            return CPP.ReturnDouble("qFabs(x)");
        }
        public static double Min(double v1, double v2) {
            return v1 < v2 ? v1 : v2;
        }
        public static double Max(double v1, double v2) {
            return v1 > v2 ? v1 : v2;
        }
        public static int Floor(double x) {
            return CPP.ReturnInt("qFloor(x)");
        }
        public static int Ceil(double x) {
            return CPP.ReturnInt("qCeil(x)");
        }
        public static double Ln(double x) {
            return CPP.ReturnDouble("qLn(x)");
        }
        public static double Exp(double x) {
            return CPP.ReturnDouble("qExp(x)");
        }
        public static double DegreesToRadians(double x) {
            return CPP.ReturnDouble("qDegreesToRadians(x)");
        }
        public static double RadiansToDegrees(double x) {
            return CPP.ReturnDouble("qRadiansToDegrees(x)");
        }
    }
}