using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Newtonsoft.Json;
using System.IO;

namespace CardMaker
{
    class Program
    {
        private const string root = "../../../../../PlutoMakeJava/PlutoMakeJava/bin/";

        static void Main(string[] args)
        {
            List<TemplateRefList> files = JsonConvert.DeserializeObject<List<TemplateRefList>>(File.ReadAllText(root + "master.js"));

            foreach (TemplateRefList templatelist in files)
            {
                foreach (TemplateRef template in templatelist.angles)
                {
                    if (template.active)
                    {
                        BatchGenerateMapping(
                            template.grid.StartsWith("{") ? template.grid : root + template.grid,
                            template.warp.StartsWith("{") ? template.warp : root + template.warp,
                            template.transformer,
                            root + template.mapping,
                            root + template.metadata
                        );
                    }
                }
            }

            Console.WriteLine("Complete!");
            Console.ReadLine();
        }

        private static Boolean CanSkip(string gridPath, string warpPath, string transformer, string mappingPath, string metadataPath)
        {
            if (File.Exists(mappingPath) && File.Exists(metadataPath))
            {
                FileInfo fi1 = new FileInfo(mappingPath);
                DateTime mappingLastWriteTime = fi1.LastWriteTime;

                FileInfo fi2 = new FileInfo(gridPath);
                DateTime gridLastWriteTime = fi2.LastWriteTime;

                FileInfo fi3 = new FileInfo(warpPath);
                DateTime warpLastWriteTime = fi3.LastWriteTime;

                MetaDataRef metadata = JsonConvert.DeserializeObject<MetaDataRef>(File.ReadAllText(metadataPath));

                if (gridLastWriteTime >= mappingLastWriteTime || warpLastWriteTime >= mappingLastWriteTime || !transformer.Equals(metadata.transform))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private static void BatchGenerateMapping(string gridPath, string warpPath, string transformer, string mappingPath, string metadataPath)
        {
            Boolean isGridJSON = gridPath.Contains("{");
            Boolean isWarpJSON = warpPath.Contains("{");

            if (!CanSkip(isGridJSON ? null : gridPath, isWarpJSON ? null : warpPath, transformer, mappingPath, metadataPath))
            {
                KeyValuePair<List<Shape>, int[]> gridData = GetShapes(isGridJSON, gridPath);
                KeyValuePair<List<Shape>, int[]> warpData = GetShapes(isWarpJSON, warpPath);

                List<Shape> gridShapes = gridData.Key;
                int[] gridDim = gridData.Value;

                List<Shape> warpShapes = warpData.Key;
                int[] warpDim = warpData.Value;

                List<KeyValuePair<Shape, Shape>> ShapeList = FindPairing(gridShapes, warpShapes);

                Transformer transformerobj;
                switch (transformer)
                {
                    case "1.5orderpoly":
                        transformerobj = new OneFiveOrderPoly();
                        break;
                    case "bilinear":
                        transformerobj = new Bilinear();
                        break;
                    case "perspective":
                        transformerobj = new Perspective();
                        break;
                    default:
                        throw new InvalidDataException("Invalid transformer argument");
                }

                Dictionary<Point, Point> mapping = GenerateMapping(ShapeList, transformerobj, gridDim[0], gridDim[1]);
                MyJSON.SaveMapping(gridDim[0], gridDim[1], mapping, transformer, mappingPath, metadataPath);
            }
        }


        private static KeyValuePair<List<Shape>, int[]> GetShapes(Boolean isJSON, string gridPath)
        {
            Grid grid;
            if (isJSON)
            {
                grid = new GridJson();
            }
            else
            {
                grid = new GridImage();
            }
            return grid.GetShapes(gridPath);
        }

        private static List<KeyValuePair<Shape, Shape>> FindPairing(List<Shape> OriginalShapes, List<Shape> WarpedSquares)
        {
            List<KeyValuePair<Shape, Shape>> ShapeList = new List<KeyValuePair<Shape, Shape>>();
            foreach (Shape shape in OriginalShapes)
            {
                Shape warpedShape = WarpedSquares.First(x => x.GetTopLeftPixel().GetColor().Equals(shape.GetTopLeftPixel().GetColor()));
                ShapeList.Add(new KeyValuePair<Shape, Shape>(shape, warpedShape));
            }

            Console.WriteLine(string.Format("Found {0} shape pairs", ShapeList.Count));
            return ShapeList;
        }

        private static Dictionary<Point, Point> GenerateMapping(List<KeyValuePair<Shape, Shape>> ShapeList, Transformer transformer, int w, int h)
        {
            Dictionary<Point, Point> mapping = new Dictionary<Point, Point>();

            foreach (KeyValuePair<Shape, Shape> pair in ShapeList)
            {
                transformer.GenerateMapping(w, h, pair.Key, pair.Value, mapping);
            }

            return mapping;
        }
    }
}
