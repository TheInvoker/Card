using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardMaker
{
    class NoFilter : ColorFilter
    {
        public override Color GetFilteredColor(Color p1, Color p2)
        {
            return p2;
        }
    }
}
