using System.Drawing;

namespace CardMaker
{
    class FindDark
    {
        public static void FindDarkParts(string logoFilePath)
        {
            Bitmap logoImage = new Bitmap(logoFilePath);
            int w = logoImage.Width;
            int h = logoImage.Height;
            Bitmap flag = new Bitmap(w, h);

            for (int x = 0; x < w; x += 1)
            {
                for (int y = 0; y < h; y += 1)
                {
                    Color pixel = logoImage.GetPixel(x, y);
                    double brightness = 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;

                    if (brightness < 150)
                    {
                        flag.SetPixel(x, y, Color.Red);
                    }
                    else
                    {
                        flag.SetPixel(x, y, pixel);
                    }
                }
            }

            flag.Save("newlogo.png");
            flag.Dispose();
        }
    }
}
