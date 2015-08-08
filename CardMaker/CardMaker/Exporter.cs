using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class Exporter
    {
        public static void ExportLogo(string logoFilePath, string newName, Dictionary<Point, Point> mapping)
        {
            Bitmap logoImage = new Bitmap(logoFilePath);
            int w = logoImage.Width;
            int h = logoImage.Height;
            Bitmap flag = new Bitmap(w, h);

            foreach (KeyValuePair<Point, Point> entry in mapping)
            {
                int x = entry.Key.X;
                int y = entry.Key.Y;
                Point point = entry.Value;

                Color pixel = logoImage.GetPixel(point.X, point.Y);
                flag.SetPixel(x, y, pixel);
            }

            flag.Save(newName);
            flag.Dispose();
            logoImage.Dispose();
        }
    }
}
