using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LiFMap
{
    public partial class Form1 : Form
    {
        private DataSet data = new DataSet();
        private MySqlConnection connection;
        private double minZoomFactor;
        private double zoomFactor = 1;
        private Bitmap renderedImage;
        public Form1()
        {
            InitializeComponent();

            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            renderedImage = new Bitmap(@"render.png");

            //minZoomFactor = Math.Min((double)pictureBox1.Height / renderedImage.Height, (double)pictureBox1.Width / renderedImage.Width);
            //zoomFactor = minZoomFactor;
            //DisplayRender();
            pictureBox1.Image = renderedImage;
            //GetData();
        }

        private void DisplayRender()
        {
            var size = Math.Min(pictureBox1.Width, pictureBox1.Height);
            var image = new Bitmap(size, size);
            image.SetResolution(renderedImage.HorizontalResolution, renderedImage.VerticalResolution);

            //var cameraBounds = new Rectangle();
            //var cameraBounds = new Rectangle(0, 0, (int)(pictureBox1.Width * zoomFactor), (int)(pictureBox1.Width / zoomFactor));
            var cameraBounds = new Rectangle(0, 0, size, size);
            //var viewportBounds = new Rectangle(0, 0, (int)(renderedImage.Width / zoomFactor), (int)(renderedImage.Width / zoomFactor));
            var viewportBounds = new Rectangle(0, 0, (int)(renderedImage.Width), (int)(renderedImage.Width));

            var g = Graphics.FromImage(image);
            g.Clear(Color.White);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(renderedImage, cameraBounds, viewportBounds, GraphicsUnit.Pixel);

            pictureBox1.Image = image;
        }

        // this tracks the transformation applied to the PictureBox's Graphics
        private Matrix transform = new Matrix();
        public static float s_dScrollValue = 0.5f; // zoom factor

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            var g = pictureBox1.CreateGraphics();
            ZoomScroll(g, e.Location, e.Delta > 0);

            return;

            var worldPos = MousePosToWorldCoords(e.Location);
            zoomFactor += e.Delta / 240f;
            zoomFactor = Math.Max(minZoomFactor, zoomFactor);

            var size = Math.Min(pictureBox1.Width, pictureBox1.Height);
            //var image = new Bitmap(size, size);
            //var zoomedImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            var zoomedImage = new Bitmap(size, size);
            zoomedImage.SetResolution(renderedImage.HorizontalResolution, renderedImage.VerticalResolution);

            //var mouseX = e.X / (double)pictureBox1.Width;
            //var mouseY = e.Y / (double)pictureBox1.Height;
            var worldX = e.X / zoomFactor;
            var worldY = e.Y / zoomFactor;


            var viewportWidth = renderedImage.Width * zoomFactor;
            var viewportHeight = renderedImage.Height * zoomFactor;

            var viewportLeft = Math.Max(0, (worldPos.X) - (viewportWidth / 2));
            var viewportTop = Math.Max(0, (worldPos.Y) - (viewportHeight / 2));

            var graphics = Graphics.FromImage(zoomedImage);
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.DrawImage(renderedImage,
                new Rectangle(0, 0, size, size),
                //new Rectangle(0, 0, (int)(renderedImage.Width / zoomFactor), (int)(renderedImage.Height / zoomFactor)),
                new Rectangle(
                    (int)(viewportLeft * zoomFactor), (int)(viewportTop * zoomFactor),
                    (int)(viewportWidth), (int)(viewportHeight)),
                GraphicsUnit.Pixel);
            pictureBox1.Image = zoomedImage;
        }

        private void ZoomScroll(Graphics g, Point location, bool zoomIn)
        {
            g.ResetTransform();
            g.TranslateTransform(-location.X, -location.Y);

            if (zoomIn)
                g.ScaleTransform(s_dScrollValue, s_dScrollValue, MatrixOrder.Append);
            else
                g.ScaleTransform(1 / s_dScrollValue, 1 / s_dScrollValue);

            g.TranslateTransform(location.X, location.Y, MatrixOrder.Append);

            //pictureBox1.Invalidate();
        }

        private Point MousePosToWorldCoords(Point location)
        {
            if (zoomFactor > 1)
                return new Point((int)(location.X / zoomFactor), (int)(location.Y / zoomFactor));
            else
                return new Point((int)(location.X / zoomFactor), (int)(location.Y / zoomFactor));
        }


        #region obsolete
        private void GetData()
        {
            connection = new MySqlConnection("Server=localhost;Database=lif_1;Uid=root;Pwd=LiFroot;");
            connection.Open();

            charPos = ExecuteToDataTable("select (GeoID >> 18) - 442 as terID, (GeoID & ((1 << 9) - 1)) as `x`, ((GeoID >> 9) & ((1 << (9)) - 1)) as `y` from `character`");

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
            return;
            if (charPos != null)
            {
                var actualX = ((ulong)charPos.Rows[0]["terid"] % 3) * 511 + (ulong)charPos.Rows[0]["x"];
                var actualY = ((ulong)charPos.Rows[0]["terid"] % 3) * 511 + (ulong)charPos.Rows[0]["y"];

                var x = (float)(actualX * zoomFactor);
                var y = (float)(actualY * zoomFactor);
                e.Graphics.FillRectangle(Brushes.Red, new RectangleF(x - 1, y - 1, 3, 3));
            }
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
            foreach (var file in files)
            {
                terrain.Add(new Terrain().LoadData(file));
            }
        }

        private List<Terrain> terrain;
        private DataTable charPos;

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

            pictureBox1.Image = renderedImage;
        }

        private void button2_Click(object sender, EventArgs e)
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
                images.Add(renderer.RenderLayer(chunk, int.Parse(textBox1.Text), int.Parse(textBox2.Text)));
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

            pictureBox1.Image = renderedImage;

            label1.Text = $"Elevation: {int.Parse(textBox1.Text)}";
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (terrain == null)
                return;
            var chunkX = e.X / 511;
            var chunkY = (3 - (e.Y / 511) - 1) * 3;

            var cellX = e.X % 511;
            var cellY = 511 - (e.Y % 511) - 1;

            var cell = terrain[chunkY + chunkX].GetMaterialAt(cellX, cellY, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            label5.Text = $"Cell at {e.X}, {e.Y}: elevation: {cell.Elevation}\nMaterial: {cell.MaterialId}\nQuality:{cell.Quality}";
            label2.Text = $"Cell at {e.X}, {e.Y}: elevation: {cell.Elevation}";
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
            if (charPos != null)
            {
                var terId = (ulong)charPos.Rows[0]["terid"];
                var chunkX = terId % 3;
                var chunkY = 3 - terId / 3 - 1;
                var actualX = chunkX * 511 + (ulong)charPos.Rows[0]["x"];
                var actualY = chunkY * 511 + 511 - (ulong)charPos.Rows[0]["y"];

                var x = (float)(actualX * zoomFactor);
                var y = (float)(actualY * zoomFactor);
                var g = Graphics.FromImage(pictureBox1.Image);
                g.FillRectangle(Brushes.Red, new RectangleF(x - 1, y - 1, 3, 3));
            }
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var worldX = (int)(e.X / zoomFactor);
            var worldY = (int)(e.Y / zoomFactor);
            label3.Text = $"WorldCoords: {worldX}x{worldY}";
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.Transform = transform;
        }
    }
}
