using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CardMaker
{
    class GridJson : Grid
    {
        private string gridPath;

        public override KeyValuePair<List<Shape>, int[]> GetShapes(string gridPath)
        {
            QuadRef quadref = JsonConvert.DeserializeObject<QuadRef>(File.ReadAllText(gridPath));
            List<List<int>> points = quadref.points;

            List<int> p1 = points.ElementAt(0);
            List<int> p2 = points.ElementAt(1);
            List<int> p3 = points.ElementAt(2);

            int width = p2.ElementAt(0) - p1.ElementAt(0) + 1;
            int height = p3.ElementAt(1) - p2.ElementAt(1) + 1;

            List<Shape> shapes = SquareDetector.GetShapeFromPoints(points, quadref.hexcolor, width, height, null);
            int[] dim = new int[] { width, height};

            return new KeyValuePair<List<Shape>, int[]>(shapes, dim);
        }
    }
}
