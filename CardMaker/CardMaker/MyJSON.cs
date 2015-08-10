using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CardMaker
{
    class MyJSON
    {
        public static void SaveMapping(Dictionary<Point, Point> dict, string transformer, string outPath, string metadataPath)
        {
            Dictionary<int, Dictionary<int, Point>> data = new Dictionary<int, Dictionary<int, Point>>();

            foreach (KeyValuePair<Point, Point> entry in dict)
            {
                int x = entry.Key.X;
                int y = entry.Key.Y;

                if (data.ContainsKey(x))
                {
                     if (!data[x].ContainsKey(y))
                    {
                        data[x].Add(y, entry.Value);
                    }
                } else
                {
                    data.Add(x, new Dictionary<int, Point>());
                    data[x].Add(y, entry.Value);
                }
            }

            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(outPath, json);

            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("transform", transformer);

            json = JsonConvert.SerializeObject(metadata);
            File.WriteAllText(metadataPath, json);
        }

        public static Dictionary<Point, Point> ReadMapping(string mappingPath)
        {
            Dictionary<int, Dictionary<int, Point>> files = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, Point>>>(File.ReadAllText(mappingPath));

            Dictionary<Point, Point> data = new Dictionary<Point, Point>();

            foreach (KeyValuePair<int, Dictionary<int, Point>> entry in files)
            {
                foreach (KeyValuePair<int, Point> entry2 in entry.Value)
                {
                    data.Add(new Point(entry.Key, entry2.Key), entry2.Value);
                }
            }

            return data;
        }
    }
}
