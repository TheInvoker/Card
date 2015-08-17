using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    abstract class Transformer
    {
        public abstract void GenerateMapping(int w, int h, Shape origShape, Shape warpedShape, Dictionary<Point, Point> mapping);
    }
}
