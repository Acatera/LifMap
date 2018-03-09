using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace TerViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            var basepath = @"D:\LifLayers\data\";
            var files = Directory.GetFiles(basepath, "*ter");
            //var fileName = "t442.ter";
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (!fileName.EndsWith("ter"))
                    continue;
                Console.WriteLine($"Rendering chunk {fileName}...");
                RenderLayers(basepath, fileName);
            }

            //for (int i = 0; i < size; i++)
            //{
            //    if (datclData[i] != datData[i][0][0])
            //    {
            //        throw new Exception();
            //    }
            //}

            //GeneratePng(datData.Select(e => e[0][0]).ToArray(), $@"D:\LifLayers\t446dat_layer{0}.png");
            //GeneratePng(datclData.ToArray(), @"D:\LifLayers\t446datcl.png");

            return;

            #region obsolete
            NewMethod();


            //var files = Directory.GetFiles(@"E:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\terrains\", "*.ter2dat*");
            //var images = new List<List<Image>>();
            //foreach (var file in files)
            //{
            //    images.Add(Bla(file));
            //}

            //var heightMap = new Bitmap(511 * 3, 511 * 3);
            //var materialMap = new Bitmap(511 * 3, 511 * 3);
            //var heightMapGraphics = Graphics.FromImage(heightMap);
            //var materialMapGraphics = Graphics.FromImage(materialMap);
            //for (int x = 0; x < 3; x++)
            //{
            //    for (int y = 0; y < 3; y++)
            //    {
            //        heightMapGraphics.DrawImage(images[y * 3 + x][0], x * 511, 1533 - (y + 1) * 511);
            //        materialMapGraphics.DrawImage(images[y * 3 + x][1], x * 511, 1533 - (y + 1) * 511);
            //    }
            //}

            //heightMap.Save(@"D:\LiFLayers\heightmap.png", ImageFormat.Png);
            //materialMap.Save(@"D:\LiFLayers\materiamap.png", ImageFormat.Png);


            var largeImages = new List<List<Image>>();
            files = Directory.GetFiles(@"E:\SteamLibrary\steamapps\common\Life is Feudal Your Own\data\terrains\", "*.ter");
            foreach (var file in files)
            {
                var heightmap = new HeightMap(file);
                //var exportPath1 = Path.Combine(@"D:\LiFLayers", $"x{Path.GetFileName(file)}.png");
                //var exportPath2 = Path.Combine(@"D:\LiFLayers", $"h{Path.GetFileName(file)}.png");
                //heightmap.data[0].Save(exportPath1, ImageFormat.Png);
                //heightmap.data[1].Save(exportPath2, ImageFormat.Png);
                largeImages.Add(heightmap.data);
            }

            var map = new Bitmap(1024 * 3, 1024 * 3);
            var data = new Bitmap(1024 * 3, 1024 * 3);
            var mapGr = Graphics.FromImage(map);
            var dataGr = Graphics.FromImage(data);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    mapGr.DrawImage(largeImages[y * 3 + x][0], x * 1024, 3072 - (y + 1) * 1024);
                    dataGr.DrawImage(largeImages[y * 3 + x][1], x * 1024, 3072 - (y + 1) * 1024);
                }
            }

            map.Save(@"D:\LiFLayers\largeheightmap.png", ImageFormat.Png);
            data.Save(@"D:\LiFLayers\largemateriamap.png", ImageFormat.Png);
            #endregion
        }

        private static void RenderLayers(string basepath, string fileName)
        {
            var datFile = Path.ChangeExtension(fileName, "ter2dat");
            var indexFile = Path.ChangeExtension(fileName, "ter2idx");
            var datclFile = Path.ChangeExtension(fileName, "ter2datcl");

            var indexPath = Path.Combine(basepath, indexFile);
            var datPath = Path.Combine(basepath, datFile);
            var datclPath = Path.Combine(basepath, datclFile);

            //Comparing *datcl with dat
            //using 446
            var size = 511 * 511;

            //load indexes. They look like a tuple <index-in-dat-file, entry-len>
            var indexData = new List<int[]>();
            using (var stream = new FileStream(indexPath, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 48;
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < size; i++)
                    {
                        var point = new int[2];
                        point[0] = reader.ReadInt32();
                        point[1] = reader.ReadInt32();
                        indexData.Add(point);
                    }
                }
            }

            //size = indexData.Sum(i => i[1]);
            var datData = new List<int[][]>();
            using (var stream = new FileStream(datPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < size; i++)
                    {
                        var depth = indexData[i][1];
                        var point = new int[depth][];
                        for (int d = 0; d < depth; d++)
                        {
                            var pe = new int[3];
                            pe[0] = reader.ReadInt16();
                            pe[1] = reader.ReadInt16();
                            pe[2] = reader.ReadInt32();
                            point[d] = pe;
                        }

                        datData.Add(point);
                    }
                }
            }

            var datclData = new List<int>();
            using (var stream = new FileStream(datclPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < size; i++)
                    {
                        datclData.Add(reader.ReadInt32());
                    }
                }
            }

            var maxDepth = indexData.Select(e => e[1]).Max();

            var colorDict = new Dictionary<int, Color>
            {
                //{ 2, Color.LemonChiffon },
                //{ 3, Color.LightGray },
                //{ 7, Color.ForestGreen },
                //{ 14, Color.Silver },

                { 5, Color.Gold },
                { 4, Color.Brown},
                { 21, Color.DodgerBlue },
                { 23, Color.SaddleBrown }
            };

            for (int i = 0; i < maxDepth; i++)
            {
                GeneratePng(datData.Select(e => { return e.Length > i ? e[i][1] : 0; }).ToArray(), $@"D:\LifLayers\{fileName}_layer{i}.png",
                    //(min, max, value) =>
                    //{
                    //    var lum = (int)MapValue(min, max, 0, 255, value);
                    //    return Color.FromArgb(lum, lum, lum);
                    //});
                    (min, max, value) =>
                    {
                        if (colorDict.ContainsKey(value))
                            return colorDict[value];
                        else
                            return Color.Black;
                    });
            }
        }

        private static void GeneratePng(int[] data, string path, Func<int, int, int, Color> colorCalc)
        {
            var size = (int)Math.Sqrt(data.Length);
            //if (size != (int)size)
            //    throw new Exception("Size is not a square");

            var min = data.Min();
            var max = data.Max();

            var image = new Bitmap((int)size, (int)size);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    //var lum = (int)MapValue(min, max, 0, 255, data[x + y * (int)size]);
                    var color = colorCalc(min, max, data[x + y * (int)size]);// Color.FromArgb(lum, lum, lum);
                    image.SetPixel(x, size - y - 1, color);
                }
            }

            image.Save(path, ImageFormat.Png);
        }

        private static void NewMethod()
        {
            var stream = new FileStream(@"D:\LifLayers\446.ter", FileMode.Open, FileAccess.Read);
            var img = new Bitmap(1024, 1024);
            for (int x = 0; x < 1024; x++)
            {
                for (int y = 0; y < 1024; y++)
                {
                    var nib = stream.ReadByte();
                    img.SetPixel(x, 1024 - y - 1, Color.FromArgb(nib, nib, nib));
                }
            }
            img.Save(@"D:\LifLayers\446.png", ImageFormat.Png);
        }

        static List<Image> Bla(string path)
        {
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var ints = new List<int>();
            var __ = new List<int>();

            while (stream.Position < stream.Length)
            {
                var val = stream.ReadByte() | stream.ReadByte() << 8;
                var _ = stream.ReadByte() | stream.ReadByte() << 8;
                ints.Add(val);
                __.Add(_);
            }

            var result = new List<Image>();

            var image = new Bitmap(511, 511);

            for (int x = 0; x < 511; x++)
            {
                for (int y = 0; y < 511; y++)
                {
                    var amplitude = (int)MapValue(3000, 10083, 0, 255, ints[y * 511 + x]);
                    var color = Color.FromArgb(amplitude, amplitude, amplitude);
                    image.SetPixel(x, 511 - y - 1, color);
                }
            }
            result.Add(image);

            var colors = new Dictionary<int, Color>()
            {
                { 1, Color.LawnGreen},
                { 2, Color.LemonChiffon},
                { 3, Color.Gray},

                { 4, Color.Red},
                { 5, Color.Red},
                { 21, Color.Red},
                { 23, Color.Red},

                { 7, Color.ForestGreen},
                { 19, Color.Brown}
            };

            image = new Bitmap(511, 511);

            for (int x = 0; x < 511; x++)
            {
                for (int y = 0; y < 511; y++)
                {
                    var color = Color.White;
                    if (colors.ContainsKey(__[y * 511 + x]))
                        color = colors[__[y * 511 + x]];

                    image.SetPixel(x, 511 - y - 1, color);
                }
            }
            result.Add(image);

            return result;
        }

        public static double MapValue(double a0, double a1, double b0, double b1, double a)
        {
            return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
        }
    }
}
