using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiFMap
{
    public class TerrainColumn
    {
        //public IList<TerrainCell> Cells { get; private set; }
        public TerrainCell[] Cells { get; private set; }

        public TerrainColumn(int depth)
        {
            Cells = new TerrainCell[depth];
        }

        public TerrainCell GetTopCell()
        {
            return Cells.DefaultIfEmpty(new TerrainCell()).FirstOrDefault();
        }

        public TerrainCell GetCellAtElevation(int elevation, int depth)
        {
            var validCells = Cells.Where(c => c.Elevation <= elevation && c.Elevation > elevation - depth)
                .OrderByDescending(c => c.Elevation);
            return validCells.DefaultIfEmpty(new TerrainCell { Elevation = elevation })
                .FirstOrDefault();
        }
    }

    public struct TerrainCell
    {
        public int MaterialId { get; set; }
        public int Elevation { get; set; }
        public int Flags { get; set; }
        public bool IsTopMost { get; set; }
        public int Quality { get; internal set; }
    }

    public class Terrain
    {
        public int Size { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Terrain()
        {

        }

        public Terrain(int x, int y)
        {
            X = x;
            Y = y;
        }

        private List<int[]> indexData;
        //private List<TerrainColumn> data;
        private TerrainColumn[] data;

        public Terrain LoadData(string path)
        {
            var indexPath = Path.ChangeExtension(path, "ter2idx");
            LoadIndexData(indexPath);

            var datPath = Path.ChangeExtension(path, "ter2dat");
            LoadTerrainData(datPath);

            return this;
        }

        private void LoadTerrainData(string datPath)
        {
            //data = new List<TerrainColumn>();
            data = new TerrainColumn[indexData.Count];
            using (var stream = new FileStream(datPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var buffer = new byte[stream.Length];
                    reader.Read(buffer, 0, buffer.Length);
                    var offset = 0;
                    for (int i = 0; i < indexData.Count; i++)
                    {
                        var depth = indexData[i][1];

                        //var buffer = new byte[depth * 8];
                        var column = new TerrainColumn(depth);
                        for (int d = 0; d < depth; d++)
                        {
                            var cell = new TerrainCell
                            {
                                Elevation = buffer[(offset) + 1] << 8 | buffer[(offset) + 0],
                                MaterialId = buffer[(offset) + 3] << 8 | buffer[(offset) + 2],
                                Flags = buffer[(offset) + 5] << 8 | buffer[(offset) + 4],
                                Quality = buffer[(offset) + 7] << 8 | buffer[(offset) + 6],
                                IsTopMost = d == 0
                            };
                            offset += 8;
                            //var cell = new TerrainCell
                            //{
                            //    Elevation = buffer[(d * 8) + 1] << 8 | buffer[(d * 8) + 0],
                            //    MaterialId = buffer[(d * 8) + 3] << 8 | buffer[(d * 8) + 2],
                            //    Flags = buffer[(d * 8) + 5] << 8 | buffer[(d * 8) + 4],
                            //    Quality = buffer[(d * 8) + 7] << 8 | buffer[(d * 8) + 6],
                            //    IsTopMost = d == 0
                            //};
                            //var cell = new TerrainCell
                            //{
                            //    Elevation = reader.ReadInt16(),
                            //    MaterialId = reader.ReadInt16(),
                            //    Flags = reader.ReadInt16(),
                            //    Quality = reader.ReadInt16(),
                            //    IsTopMost = d == 0
                            //};
                            //column.Cells.Add(cell);
                            column.Cells[d] = cell;
                        }
                        //data.Add(column);
                        data[i] = column;
                        //Data.Add(point);
                    }
                }
            }
        }

        private void LoadIndexData(string indexPath)
        {
            indexData = new List<int[]>();
            using (var stream = new FileStream(indexPath, FileMode.Open, FileAccess.Read))
            {
                stream.Position = 40;
                using (var reader = new BinaryReader(stream))
                {
                    Size = reader.ReadInt16();
                    var size = Size * Size;

                    stream.Position += 2;

                    var terrainId = reader.ReadInt32();
                    X = (terrainId - 442) % 3;
                    Y = 3 - (terrainId - 442) / 3 - 1;

                    for (int i = 0; i < size; i++)
                    {
                        var point = new int[2];
                        point[0] = reader.ReadInt32();
                        point[1] = reader.ReadInt32();
                        indexData.Add(point);
                    }
                }
            }
        }

        public TerrainCell GetTopElevation(int x, int y)
        {
            return data[y * Size + x].GetTopCell();
        }

        public TerrainCell GetMaterialAt(int x, int y, int elevation, int depth)
        {
            return data[y * Size + x].GetCellAtElevation(elevation, depth);

            //return Data[y * Size + x][0][1];
        }
    }
}
