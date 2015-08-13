using System;
using System.Drawing;

namespace CardMaker
{
    class ColorCalculate
    {
        public static int Darken(int A, int B)
        {
            return Math.Min(A, B);
        }

        public static int VividLight(int A, int B)
        {
            if (B >= 128)
            {
                return ColorDodge(A, (2 * (B - 128)));
            }
            else
            {
                return ColorBurn(A, 2 * B);
            }
        }

        public static int ColorDodge(int A, int B)
        {
            return ((B == 255) ? B : Math.Min(255, ((A << 8) / (255 - B))));
        }

        public static int ColorBurn(int A, int B)
        {
            return ((B == 0) ? B : Math.Max(0, (255 - ((255 - A) << 8) / B)));
        }

        public static Color Mix(Color B, Color A)
        {
            int rA = A.R;
            int gA = A.G;
            int bA = A.B;
            int aA = A.A;
            int rB = B.R;
            int gB = B.G;
            int bB = B.B;
            int aB = B.A;
            int rOut = (rA * aA / 255) + (rB * aB * (255 - aA) / (255 * 255));
            int gOut = (gA * aA / 255) + (gB * aB * (255 - aA) / (255 * 255));
            int bOut = (bA * aA / 255) + (bB * aB * (255 - aA) / (255 * 255));
            int aOut = aA + (aB * (255 - aA) / 255);
            return Color.FromArgb(aOut, rOut, gOut, bOut);
        }
    }
}
