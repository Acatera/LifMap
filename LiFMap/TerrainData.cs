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
        public IList<TerrainCell> Cells { get; private set; }

        public TerrainColumn()
        {
            Cells = new List<TerrainCell>();
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

    public class TerrainCell
    {
        public int MaterialId { get; set; }
        public int Elevation { get; set; }
        public int Flags { get; set; }
        public bool IsTopMost { get; set; }
        public short Quality { get; internal set; }
    }

    public class Terrain
    {
        public int Size { get; private set; }
        public List<int[][]> Data { get; set; }

        private List<int[]> indexData;
        private List<TerrainColumn> data;

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
            Data = new List<int[][]>();
            data = new List<TerrainColumn>();
            using (var stream = new FileStream(datPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    for (int i = 0; i < indexData.Count; i++)
                    {
                        var depth = indexData[i][1];
                        //var point = new int[depth][];
                        var column = new TerrainColumn();
                        for (int d = 0; d < depth; d++)
                        {
                            var cell = new TerrainCell
                            {
                                Elevation = reader.ReadInt16(),
                                MaterialId = reader.ReadInt16(),
                                Flags = reader.ReadInt16(),
                                Quality = reader.ReadInt16(),
                                IsTopMost = d == 0
                            };
                            //var pe = new int[3];
                            //pe[0] = reader.ReadInt16();
                            //pe[1] = reader.ReadInt16();
                            //pe[2] = reader.ReadInt32();
                            //point[d] = pe;
                            column.Cells.Add(cell);
                        }
                        data.Add(column);
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

                    stream.Position += 6;
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
