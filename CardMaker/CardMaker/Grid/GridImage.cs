using System.Collections.Generic;
using System.Drawing;

namespace CardMaker
{
    class GridImage : Grid
    {
        public override KeyValuePair<List<Shape>, int[]> GetShapes(string gridPath)
        {
            Bitmap origImage = new Bitmap(gridPath);
            int width = origImage.Width;
            int height = origImage.Height;

            List<Shape> gridShapes = SquareDetector.FindSquare(origImage, null);
            origImage.Dispose();

            int[] dim = new int[] { width, height };

            return new KeyValuePair<List<Shape>, int[]>(gridShapes, dim);
        }
    }
}
