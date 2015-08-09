using System;
using System.Drawing;

namespace CardMaker
{
    class Darken : ColorFilter
    {
        public override Color GetFilteredColor(Color p1, Color p2)
        {
            return Color.FromArgb(Math.Min(p1.R, p2.R), Math.Min(p1.G, p2.G), Math.Min(p1.B, p2.B));
        }
    }
}
