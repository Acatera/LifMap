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
        private Bitmap renderedImage;
        private float zoomFactor = 1;
        private List<Point> extraData = new List<Point>();

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

        private Point MousePosToWorldCoords(Point location)
        {
            return new Point((int)(location.X / zoomFactor), (int)(location.Y / zoomFactor));
        }

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

        private DataTable ExecuteToDataTable(string command)
        {
            var cmd = new MySqlCommand(command, connection);
            var adapter = new MySqlDataAdapter(cmd);
            var data = new DataTable();
            adapter.Fill(data);

            return data;
        }

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
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetData();

            if (cbFollowPlayer.Checked)
            {
                panel2.AutoScrollPosition = charPos;
            }
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            label3.Text = $"MouseCoords: {e.X}x{e.Y}";
            var worldPos = MousePosToWorldCoords(e.Location);
            if (terrain == null)
                return;

            var chunkX = worldPos.X / 511;
            var chunkY = worldPos.Y / 511;

            var cellX = worldPos.X % 511;
            var cellY = 511 - (worldPos.Y % 511) - 1;

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

            var materialId = ((KeyValuePair<int, string>)cbSubstances.SelectedItem).Key;

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
