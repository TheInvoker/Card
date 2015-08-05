using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CardMaker
{
    class Pixel
    {
        Color color;
        int x;
        int y;

        public Pixel(Color color, int x, int y)
        {
            this.color = color;
            this.x = x;
            this.y = y;
        }

        public Color GetColor()
        {
            return color;
        }

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
    }
}
