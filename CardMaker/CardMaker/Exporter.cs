using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class Exporter
    {
        public static Bitmap GenerateWarpedLogo(string logoFilePath, string maskPath, Dictionary<Point, Point> mapping)
        {
            Bitmap logoImage = new Bitmap(logoFilePath);
            int w = logoImage.Width;
            int h = logoImage.Height;
            Bitmap flag = new Bitmap(w, h);

            Bitmap maskfile = null;
            if (System.IO.File.Exists(maskPath))
            {
                maskfile = new Bitmap(maskPath);
            }

            foreach (KeyValuePair<Point, Point> entry in mapping)
            {
                int x = entry.Key.X;
                int y = entry.Key.Y;
                Point point = entry.Value;
                Color pixel = logoImage.GetPixel(point.X, point.Y);

                int newA = pixel.A;
                if (maskfile != null)
                {
                    int maskA = maskfile.GetPixel(x, y).A;
                    if (maskA != 0)
                    {
                        newA *= maskfile.GetPixel(x, y).A / 255;
                    } else
                    {
                        continue;
                    }
                }

                flag.SetPixel(x, y, Color.FromArgb(newA, pixel.R, pixel.G, pixel.B));
            }

            if (maskfile != null)
            {
                maskfile.Dispose();
            }

            logoImage.Dispose();
            return flag;
        }

        public static void StampLogo(string templatePath, string outPath, int xStart, int yStart, int width, int height, Bitmap logo, ColorFilter filter)
        {
            Bitmap templateImage = new Bitmap(templatePath);
            int w = logo.Width;
            int h = logo.Height;

            Bitmap newLogo = null;
            if (w != width || h != height)
            {
                newLogo = ResizeImage(logo, width, height);
                logo = newLogo;
                w = logo.Width;
                h = logo.Height;
            }

            for(int x = 0; x < w; x += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    if (x + xStart < templateImage.Width && y + yStart < templateImage.Height)
                    {
                        Color pixel = logo.GetPixel(x, y);
                        if (pixel.A != 0)
                        {
                            Color currentPixel = templateImage.GetPixel(x + xStart, y + yStart);
                            Color newColor = ColorCalculate.Mix(currentPixel, pixel);
                            templateImage.SetPixel(x + xStart, y + yStart, filter.GetFilteredColor(currentPixel, newColor));
                        }
                    }
                }
            }

            if (newLogo != null)
            {
                newLogo.Dispose();
            }

            templateImage.Save(outPath);
            templateImage.Dispose();
        }

        private static Bitmap ResizeImage(Bitmap imgToResize, int newWidth, int newHeight)
        {
            Bitmap b = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, newWidth, newHeight);
            }
            return b;
        }
    }
}
