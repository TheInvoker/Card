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

            foreach (KeyValuePair<String, Point> entry in mapping)
            {
                String[] xy = entry.Key.Split(',');
                Point point = entry.Value;

                int x = Convert.ToInt32(xy[0]);
                int y = Convert.ToInt32(xy[1]);

                Color pixel = logoImage.GetPixel(point.X, point.Y);
                flag.SetPixel(x, y, pixel);
            }

            flag.Save(newName);
            flag.Dispose();
            logoImage.Dispose();
        }
    }
}
