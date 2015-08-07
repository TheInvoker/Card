using System;
using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class Exporter
    {
        public static void ExportLogo(string logoFilePath, string newName, Dictionary<String, Point> mapping)
        {
            Bitmap logoImage = new Bitmap(logoFilePath);
            int w = logoImage.Width;
            int h = logoImage.Height;
            Bitmap flag = new Bitmap(w, h);

            for (int x = 0; x < w; x += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    string key = string.Format("{0},{1}", x, y);
                    Point point = mapping[key];
                    Color pixel = logoImage.GetPixel(point.X, point.Y);
                    flag.SetPixel(x, y, pixel);
                }
            }

            flag.Save(newName);
            flag.Dispose();
            logoImage.Dispose();
        }
    }
}
