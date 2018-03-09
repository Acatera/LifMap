using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TerViewer
{
    internal class HeightMap
    {
        public List<Image> data;

        public HeightMap(string path)
        {
            data = LoadFile(path);
        }

        private List<Image> LoadFile(string path)
        {
            if (!File.Exists(path))
                return new List<Image>();

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            //stream.Position = 11;
            stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            //stream.ReadByte(); stream.ReadByte(); stream.ReadByte(); stream.ReadByte();
            var image = new Bitmap(1024, 1024);
            var image2 = new Bitmap(1024, 1024);

            for (int y = 0; y < 1024; y++)
            {
                for (int x = 0; x < 1024; x++)
                {
                    if (stream.Position >= stream.Length)
                        continue;
                    var nib1 = stream.ReadByte();// | stream.ReadByte() << 8; // << 8 | stream.ReadByte();

                    var nib2 = stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16; 
                    //stream.ReadByte(); //stream.ReadByte();// stream.ReadByte();
                    //stream.ReadByte(); stream.ReadByte();// stream.ReadByte();

                    //nib = (stream.ReadByte() << 8) | nib;
                    //nib = nib >> 4 + nib << 4;
                    //if ((y / 2) % 2 == 0)
                    //{
                    //    continue;
                    //}
                    var lum1 = (int)MapValue(0, 255, 0, 255, nib1);
                    image.SetPixel(x, 1024 - y - 1, Color.FromArgb(lum1, lum1, lum1));

                    var lum2 = (int)MapValue(0, 16777216, 0, 255, nib2);
                    //var lum2 = (int)MapValue(0, 65535, 0, 255, nib2);
                    image2.SetPixel(x, 1024 - y - 1, Color.FromArgb(lum2, lum2, lum2));
                }
                //stream.ReadByte();
            }
            return new List<Image> { image, image2 };
        }

        public double MapValue(double a0, double a1, double b0, double b1, double a)
        {
            return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
        }
    }
}