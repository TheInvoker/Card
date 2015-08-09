using System;
using System.Drawing;

namespace CardMaker
{
    class VividLight : ColorFilter
    {
        public override Color GetFilteredColor(Color p1, Color p2)
        {
            int newR = Math.Max(0, Math.Min(255, Convert.ToInt32(ColorCalculate.VividLight(p1.R, p2.R))));
            int newG = Math.Max(0, Math.Min(255, Convert.ToInt32(ColorCalculate.VividLight(p1.G, p2.G))));
            int newB = Math.Max(0, Math.Min(255, Convert.ToInt32(ColorCalculate.VividLight(p1.B, p2.B))));

            return Color.FromArgb(newR, newG, newB);
        }
    }
}
