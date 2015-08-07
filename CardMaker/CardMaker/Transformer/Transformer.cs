using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    abstract class Transformer
    {
        public abstract void DrawShape(Bitmap origLogo, Bitmap newLogo, Shape origShape, Shape warpedShape, Dictionary<string, Point> mapping);
    }
}
