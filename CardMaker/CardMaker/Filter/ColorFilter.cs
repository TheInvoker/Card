using System.Drawing;

namespace CardMaker
{
    abstract class ColorFilter
    {
        public abstract Color GetFilteredColor(Color p1, Color p2);
    }
}
