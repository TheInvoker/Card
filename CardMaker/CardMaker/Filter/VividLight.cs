using System;
using System.Drawing;

namespace CardMaker
{
    class VividLight : ColorFilter
    {
        public override Color GetFilteredColor(Color p1, Color p2)
        {
            int newR = Math.Max(0, Math.Min(255, Convert.ToInt32(GetColor(p1.R, p2.R))));
            int newG = Math.Max(0, Math.Min(255, Convert.ToInt32(GetColor(p1.G, p2.G))));
            int newB = Math.Max(0, Math.Min(255, Convert.ToInt32(GetColor(p1.B, p2.B))));

            return Color.FromArgb(newR, newG, newB);
        }

        private static double GetColor(int A, int B)
        {
            if (B >= 128)
            {
                return GetColorDodgeValue(A, (2 * (B - 128)));
            }
            else
            {
                return GetColorBurnValue(A, 2 * B);
            }
        }

        private static int GetColorBurnValue(int A, int B)
        {
            return ((B == 0) ? B : Math.Max(0, (255 - ((255 - A) << 8) / B)));
        }

        private static int GetColorDodgeValue(int A, int B)
        {
            return ((B == 255) ? B : Math.Min(255, ((A << 8) / (255 - B))));
        } 
    }
}
