using System;

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
    }
}
