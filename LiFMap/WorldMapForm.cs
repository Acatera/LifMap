using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LiFMap
{
    //public class PictureBoxInterpolatedZoom : PictureBox
    //{
    //    protected override void OnPaint(PaintEventArgs pe)
    //    {
    //        pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
    //        base.OnPaint(pe);
    //    }
    //}

    public partial class WorldMapForm : Form
    {
        private DataSet data = new DataSet();
        private MySqlConnection connection;
        private double minZoomFactor;
        private Point renderOffset = new Point(0, 0);
        private Bitmap renderedImage;
        public WorldMapForm()
        {
            InitializeComponent();

            panel2.MouseWheel += Panel2_MouseWheel;
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            //renderedImage = new Bitmap(@"render.png");

            //pictureBox1.Size = renderedImage.Size;
            //pictureBox1.Image = renderedImage;
            LoadSubstances();
        }

        private void LoadSubstances()
        {
            var substances = new Dictionary<int, string>();
            var substancesFile = @"D:\Steam\steamapps\common\Life is Feudal Your Own\scripts\cm_substances.cs";
            var regex = new Regex(@"ter2_id.+?([0-9]*?);.*?terrainMaterialName.*?\""([A-Za-z]*?)\"";", RegexOptions.Singleline);
            var text = File.ReadAllText(substancesFile);
            foreach (Match match in regex.Matches(text))
            {
                var substId = int.Parse(match.Groups[1].Value);
                var substName = match.Groups[2].Value;
                substances.Add(substId, substName);
            }

            cbSubstances.Items.Clear();
            cbSubstances.DisplayMember = "Value";
            cbSubstances.ValueMember = "Key";
            foreach (var substance in substances)
            {
                cbSubstances.Items.Add(substance);
                
            }
            cbSubstances.Sorted = true;
        }

        private void Panel2_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private float zoomFactor = 1;
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if ((ModifierKeys & Keys.Control) != 0)
            {
                if (e.Delta > 0)
                {
                    if (zoomFactor > 8)
                        return;
                    zoomFactor *= 2;
                    pictureBox1.Width *= 2;
                    pictureBox1.Height *= 2;
                }
                else
                {
                    if (zoomFactor == 1)
                        return;
                    zoomFactor /= 2;
                    pictureBox1.Width /= 2;
                    pictureBox1.Height /= 2;
                }
            }
            else if ((ModifierKeys & Keys.Shift) != 0)
            {
                var newHorizontalPos = Math.Max(0, panel2.HorizontalScroll.Value - e.Delta);
                newHorizontalPos = Math.Min(panel2.HorizontalScroll.Maximum, newHorizontalPos);
                panel2.HorizontalScroll.Value = newHorizontalPos;
            }
            else
            {
                var newVerticalPos = Math.Max(0, panel2.VerticalScroll.Value - e.Delta);
                newVerticalPos = Math.Min(panel2.VerticalScroll.Maximum, newVerticalPos);
                panel2.VerticalScroll.Value = newVerticalPos;
            }
        }

        private void AddOutput(string s)
        {
            lbOutput.Items.Add(s);
        }

        private Point MousePosToWorldCoords(Point location)
        {
            if (zoomFactor > 1)
                return new Point((int)((location.X) / zoomFactor) + renderOffset.X, (int)((location.Y) / zoomFactor) + renderOffset.Y);
            else
                return new Point((int)(location.X / zoomFactor) + renderOffset.X, (int)(location.Y / zoomFactor) + renderOffset.Y);
        }

        List<Point> extraData = new List<Point>();
        #region obsolete
        private void GetData()
        {
            connection = new MySqlConnection("Server=localhost;Database=lif_1;Uid=root;Pwd=LiFroot;");
            connection.Open();

            var charPos = ExecuteToDataTable("select (GeoID >> 18) - 442 as terID, (GeoID & ((1 << 9) - 1)) as `x`, ((GeoID >> 9) & ((1 << (9)) - 1)) as `y` from `character`");
            this.charPos = GeoIdToPoint(charPos.Rows[0]);

            extraData.Clear();
            var data = ExecuteToDataTable("select(f.GeoDataID >> 18) - 442 as terID, (f.GeoDataID & ((1 << 9) - 1)) as `x`, ((f.GeoDataID >> 9) & ((1 << (9)) - 1)) as `y`, f.treetype, f.Quality from forest f inner join forest_patch fp on f.GeoDataID = fp.GeoDataID and f.Quality > 90");
            foreach (DataRow row in data.Rows)
            {
                var point = GeoIdToPoint(row);
                extraData.Add(point);
            }

            connection.Close();

            return;
            var layers = new List<Image>();

            layers.Add(ExecuteToImage(
                "select(GeoDataID >> 18) - 442 as terID, (GeoDataID & ((1 << 9) - 1)) as `x`, ((GeoDataID >> 9) & ((1 << (9)) - 1)) as `y` " +
                "from forest_patch " +
                "where GeoDataId IS NOT NULL", Color.ForestGreen));
            layers.Add(ExecuteToImage(
            "select(f.GeoDataID >> 18) - 442 as terID, (f.GeoDataID & ((1 << 9) - 1)) as `x`, ((f.GeoDataID >> 9) & ((1 << (9)) - 1)) as `y`, f.treetype, f.Quality from forest f inner join forest_patch fp on f.GeoDataID = fp.GeoDataID and f.Quality > 80", Color.Red));

            layers.Add(ExecuteToImage(
                "select IfNull(GeoDataID, 0)," +
                    "(GeoDataID >> 18) - 442 as terID, " +
                    "(GeoDataID & ((1 << 9) - 1)) as `x`, " +
                    "((GeoDataID >> 9) & ((1 << (9)) - 1)) as `y`, " +
                    "IFNULL(Substance, 0) Substance " +
                "from geo_patch gp " +
                "Where substance < 64", Color.DodgerBlue));

            layers.Add(ExecuteToImage(
                "select (GeoDataID >> 18) - 442 as terID, (GeoDataID& ((1 << 9) - 1)) as `x`, ((GeoDataID >> 9) & ((1 << (9)) - 1)) as `y` from objects_patch", Color.Gray));

            if (!Directory.Exists(@"D:\LiFLayers"))
                Directory.CreateDirectory(@"D:\LiFLayers");

            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].Save($@"D:\LiFLayers\layer{i}.png", ImageFormat.Png);
            }
            //for (int i = 0; i < 16; i++)
            //{

            //ExecuteCommand("select IfNull(GeoDataID, 0), 	(GeoDataID >> 18) - 442 as terID, (GeoDataID & ((1 << 9) - 1)) as `x`, ((GeoDataID >> 9) & ((1 << (9)) - 1)) as `y`, IFNULL(Substance, 0) Substance from geo_patch gp Where substance < 64", connection);



            //}

            //ExecuteCommand("select (GeoDataID >> 18) - 442 as terID, (GeoDataID& ((1 << 9) - 1)) as `x`, ((GeoDataID >> 9) & ((1 << (9)) - 1)) as `y` from geo_patch", connection);

        }

        private Point GeoIdToPoint(DataRow row)
        {
            var terId = (ulong)row["terid"];
            var chunkX = terId % 3;
            var chunkY = 3 - terId / 3 - 1;
            var actualX = chunkX * 511 + (ulong)row["x"];
            var actualY = chunkY * 511 + 511 - (ulong)row["y"];
            var x = (int)(actualX * zoomFactor);
            var y = (int)(actualY * zoomFactor);
            return new Point(x, y);
        }

        private Image ExecuteToImage(string command, Color color)
        {
            var data = ExecuteToDataTable(command);
            return GenerateLayer(data, color);
        }

        private Image GenerateLayer(DataTable data, Color color)
        {
            var image = new Bitmap(511 * 3, 511 * 3);
            foreach (DataRow dataPoint in data.Rows)
            {
                int x = int.Parse(dataPoint["x"].ToString()) + (int.Parse(dataPoint["terid"].ToString()) % 3) * 511;
                int y = 1533 - (int.Parse(dataPoint["y"].ToString()) + (int.Parse(dataPoint["terid"].ToString()) / 3) * 511);

                image.SetPixel(x, y, color);// (brush, x, y, 1, 1);
            }

            return image;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //return;
            //if (charPos != null)
            //{
            //    var actualX = ((ulong)charPos.Rows[0]["terid"] % 3) * 511 + (ulong)charPos.Rows[0]["x"];
            //    var actualY = ((ulong)charPos.Rows[0]["terid"] % 3) * 511 + (ulong)charPos.Rows[0]["y"];

            //    var x = (float)(actualX * zoomFactor);
            //    var y = (float)(actualY * zoomFactor);
            //    e.Graphics.FillRectangle(Brushes.Red, new RectangleF(x - 1, y - 1, 3, 3));
            //}
        }

        public double MapValue(double a0, double a1, double b0, double b1, double a)
        {
            return b0 + (b1 - b0) * ((a - a0) / (a1 - a0));
        }

        private DataTable ExecuteToDataTable(string command)
        {
            var cmd = new MySqlCommand(command, connection);
            var adapter = new MySqlDataAdapter(cmd);
            var data = new DataTable();
            adapter.Fill(data);

            return data;
        }

        private void ExecuteCommand(string command)
        {
            var data = ExecuteToDataTable(command);
            this.data.Tables.Add(data);
        }
        #endregion
        private void LoadData()
        {
            terrain = new List<Terrain>();
            var basepath = string.Empty;
            if (Directory.Exists(@"D:\Dev\Lif\"))
                basepath = @"D:\Dev\Lif\";
            else
                basepath = @"D:\LifLayers\data";
            var files = Directory.GetFiles(basepath, "*.ter").OrderBy(f => f).Where(f => f.EndsWith("ter"));

            var tasks = new List<Task>();
            foreach (var file in files)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    terrain.Add(new Terrain().LoadData(file));
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private List<Terrain> terrain;
        private Point charPos;

        private void button1_Click(object sender, EventArgs e)
        {
            if (terrain == null)
            {
                LoadData();
            }

            zoomFactor = 1;
            var renderer = new TerrainRenderer();
            var images = new List<Bitmap>();
            foreach (var chunk in terrain)
            {
                images.Add(renderer.Render(chunk));
            }

            var chunkSize = images[0].Width; //or height. They are square;
            renderedImage = new Bitmap(chunkSize * 3, chunkSize * 3);
            var graphics = Graphics.FromImage(renderedImage);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    graphics.DrawImage(images[y * 3 + x], new Point(x * chunkSize, (3 - y - 1) * chunkSize));
                }
            }

            pictureBox1.Size = renderedImage.Size;
            pictureBox1.Image = renderedImage;
        }

        private void btnRender_Click(object sender, EventArgs e)
        {
            var watch = new Stopwatch();
            watch.Start();
            if (terrain == null)
            {
                LoadData();
            }
            lbOutput.Items.Add($"Loading terrrain = {watch.ElapsedMilliseconds}");

            watch.Restart();
            var renderer = new TerrainRenderer();
            var images = new Dictionary<Tuple<int, int>, Bitmap>();
            var tasks = new List<Task>();


            foreach (var chunk in terrain)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    images.Add(new Tuple<int, int>(chunk.X, chunk.Y),
                        renderer.RenderLayer(chunk, int.Parse(textBox1.Text), int.Parse(textBox2.Text)));
                }));
            }
            Task.WaitAll(tasks.ToArray());

            CombineRenderedChunks(images);

            pictureBox1.Size = renderedImage.Size;
            pictureBox1.Image = renderedImage;
            lbOutput.Items.Add($"Rendering terrrain = {watch.ElapsedMilliseconds}");

            //label1.Text = $"Elevation: {int.Parse(textBox1.Text)}";
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (terrain == null)
                return;

            var worldPos = MousePosToWorldCoords(e.Location);

            var chunkX = worldPos.X / 511;
            var chunkY = worldPos.Y / 511;

            var cellX = worldPos.X % 511;
            var cellY = 511 - (worldPos.Y % 511) - 1;
            //var cellY = (worldPos.Y % 511);

            var chunk = terrain.Where(t => t.X == chunkX && t.Y == chunkY).First();

            var cell = chunk.GetCellAt(cellX, cellY, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            label5.Text = $"Cell at {worldPos.X}, {worldPos.Y}: elevation: {cell.Elevation - 5000}\nMaterial: {cell.MaterialId}\nQuality:{cell.Quality}";
            label2.Text = $"Cell at {worldPos.X}, {worldPos.Y}: elevation: {cell.Elevation}";
        }

        Timer timer;
        private void button3_Click(object sender, EventArgs e)
        {
            if (timer == null)
            {
                timer = new Timer { Interval = 2000 };
                timer.Tick += Timer_Tick;
                timer.Start();
                (sender as Button).Text = "Stop";
            }
            else
            {
                timer.Stop();
                timer = null;
                (sender as Button).Text = "Start";
            }
            //renderedImage.Save(@"render.png", ImageFormat.Png);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetData();
            
            if (cbFollowPlayer.Checked)
            {
                panel2.AutoScrollPosition = charPos;
                //panel2.HorizontalScroll.Value = charPos.X;
                //panel2.VerticalScroll.Value = charPos.Y;
            }
            pictureBox1.Refresh();
        }

        //Point oldMousePos;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //var mousePos = PointToClient(Cursor.Position);
            label3.Text = $"MouseCoords: {e.X}x{e.Y}";
            var worldPos = MousePosToWorldCoords(e.Location);
            if (terrain == null)
                return; 

            var chunkX = worldPos.X / 511;
            var chunkY = worldPos.Y / 511;

            var cellX = worldPos.X % 511;
            var cellY = 511 - (worldPos.Y % 511) - 1;
            //var cellY = (worldPos.Y % 511);

            var chunk = terrain.Where(t => t.X == chunkX && t.Y == chunkY).First();

            var cell = chunk.GetCellAt(cellX, cellY, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            label5.Text = $"Cell at {worldPos.X}, {worldPos.Y}: elevation: {cell.Elevation - 5000}\nMaterial: {cell.MaterialId}\nQuality:{cell.Quality}";
            label2.Text = $"Cell at {worldPos.X}, {worldPos.Y}: elevation: {cell.Elevation}";
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (charPos != null)
                e.Graphics.FillRectangle(Brushes.Red, new RectangleF(charPos.X - 1, charPos.Y - 1, 3, 3));

            var rect = new Rectangle(0, 0, 3, 3);
            foreach (var point in extraData)
            {
                rect.X = point.X;
                rect.Y = point.Y;
                e.Graphics.FillRectangle(Brushes.DarkViolet, rect);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var watch = new Stopwatch();
            watch.Start();
            if (terrain == null)
            {
                LoadData();
                lbOutput.Items.Add($"Loading terrrain = {watch.ElapsedMilliseconds}");
            }

            watch.Restart();
            var renderer = new TerrainRenderer();
            var images = new Dictionary<Tuple<int, int>, Bitmap>();
            var tasks = new List<Task>();

            var materialId = ((KeyValuePair < int, string> )cbSubstances.SelectedItem).Key;

            var minQuality = 0;
            if (!int.TryParse(txtMinQuality.Text, out minQuality))
                return;

            foreach (var chunk in terrain)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    images.Add(new Tuple<int, int>(chunk.X, chunk.Y),
                        renderer.RenderSearchResults(chunk, int.Parse(textBox1.Text), int.Parse(textBox2.Text), materialId, minQuality));
                }));
            }
            Task.WaitAll(tasks.ToArray());

            CombineRenderedChunks(images);

            pictureBox1.Size = renderedImage.Size;
            pictureBox1.Image = renderedImage;
            lbOutput.Items.Add($"Rendering terrrain = {watch.ElapsedMilliseconds}");
        }

        private void CombineRenderedChunks(Dictionary<Tuple<int, int>, Bitmap> images)
        {
            var chunkSize = 511;//hardcoded for now;
            renderedImage = new Bitmap(chunkSize * 3, chunkSize * 3);
            var graphics = Graphics.FromImage(renderedImage);
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    var tuple = new Tuple<int, int>(x, y);
                    if (images.ContainsKey(tuple))
                        graphics.DrawImage(images[tuple], new Point(x * chunkSize, y * chunkSize));
                }
            }
        }
    }
}
