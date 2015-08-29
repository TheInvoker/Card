using System.Collections.Generic;

namespace CardMaker
{
    abstract class Grid
    {
        public abstract KeyValuePair<List<Shape>,int[]> GetShapes(string gridPath);
    }
}
