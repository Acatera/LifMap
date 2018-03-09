using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiFMap
{
    internal class TerrainRenderer
    {
        internal Bitmap Render(Terrain terrain)
        {
            var image = new Bitmap(terrain.Size, terrain.Size);

            for (int x = 0; x < terrain.Size; x++)
            {
                for (int y = 0; y < terrain.Size; y++)
                {
                    var cell = terrain.GetTopElevation(x, y);
                    var intensity = (int)MapValue(0, 7000, 0, 255, cell.Elevation);
                    var color = Color.FromArgb(intensity, intensity, intensity);
                    image.SetPixel(x, y, color);
                }
            }

            return image;
        }

        private static double MapValue(double a0, double a1, double b0, double b1, double a)
        {
            return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
        }

        internal Bitmap RenderLayer(Terrain terrain, int elevation, int depth)
        {
            var image = new Bitmap(terrain.Size, terrain.Size);

            var colorMap = new Dictionary<int, Color>
            {
                { 1, Color.LawnGreen },
                { 2, Color.LemonChiffon },
                { 3, Color.DarkGray },
                { 7, Color.ForestGreen }
            };

            var veinMap = new Dictionary<int, Color>
            {
                { 4,  Color.SaddleBrown }, //Iron
                { 5,  Color.Gold },
                { 21, Color.Silver },
                { 23, Color.FromArgb(218, 138, 103) } //Copper
            };

            for (int x = 0; x < terrain.Size; x++)
            {
                for (int y = 0; y < terrain.Size; y++)
                {
                    var color = Color.Black;
                    var cell = terrain.GetMaterialAt(x, y, elevation, depth);
                    if (cell.IsTopMost)
                    {
                        if (cell.Elevation < 5000)
                        {
                            var brightness = cell.Elevation / 5000f;
                            color = Color.FromArgb((int)(30f * brightness), (int)(144f * brightness), (int)(255f * brightness));
                        }
                        else
                        {
                            if (colorMap.ContainsKey(cell.MaterialId))
                            {
                                color = colorMap[cell.MaterialId];
                                color = ApplyElevation(color, elevation, 7000);
                            }
                            //var intensity = (int)MapValue(0, 7000, 0, 255, cell.Elevation);
                            //color = Color.FromArgb(intensity, intensity, intensity);
                        }
                    }
                    else
                    {
                        if (veinMap.ContainsKey(cell.MaterialId))
                        {
                            color = veinMap[cell.MaterialId];
                        }
                    }
                    image.SetPixel(x, terrain.Size - y - 1, color);
                }
            }

            return image;
        }

        private Color ApplyElevation(Color baseColor, double elevation, double maxElevation)
        {
            var brightness = elevation / maxElevation;

            var r = (int)(baseColor.R * brightness);
            var g = (int)(baseColor.G * brightness);
            var b = (int)(baseColor.B * brightness);

            return Color.FromArgb(r, g, b);
        }
    }
}