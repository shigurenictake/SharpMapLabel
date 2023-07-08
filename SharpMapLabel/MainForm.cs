using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration;
using SharpMap.Rendering.Symbolizer;
using SharpMap.Styles;
using System;
using System.Drawing;
using System.Windows.Forms;
using SharpMapLabel.Properties;
using Point = NetTopologySuite.Geometries.Point;

namespace SharpMapLabel
{
    public partial class MainForm : Form
    {
        private MovingObjects _fastBoats;
        private static Image _boat;

        //コンストラクタ
        public MainForm()
        {
            InitializeComponent();
        }

        //イベント フォーム ロード完了 
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            this.SizeChanged += Form_SizeChanged;

            using (var renderer = SharpMap.Forms.MapBox.MapImageGeneratorFunction(new SharpMap.Forms.MapBox(), null))
            {
                if (renderer is SharpMap.Forms.ImageGenerator.LegacyMapBoxImageGenerator)
                {
                    this.txtImgGeneration.Text = (this.txtImgGeneration.Text + "\n.    LegacyMapImageRenderer");
                }
                else
                {
                    this.txtImgGeneration.Text = (this.txtImgGeneration.Text + "\n.    LayerListImageGenerator");
                }
            }

            _boat = Resources.vessel_01;

            var map = InitMap();
            InitBackground(map);
            InitVariableLayers(map);

            this.mb.Map = map;

            this.mb.Map.Center = new Coordinate(135, 36);

            this.mb.Refresh();
        }
        //イベント フォーム 閉じる
        private void MainForm_Closing(object sender, EventArgs e)
        {
            this.SizeChanged -= Form_SizeChanged;
        }
        //イベント フォーム リサイズ
        private void Form_SizeChanged(object sender, EventArgs e)
        {
            this.mb.Refresh();
        }

        //マップ初期化
        private Map InitMap()
        {
            var res = new SharpMap.Map()
            {
                SRID = 3857,
                BackColor = System.Drawing.Color.AliceBlue,
            };

            res.Decorations.Add(new NorthArrow {ForeColor = Color.DarkSlateBlue});
            return res;
        }

        //★初期化 バリアブルレイヤ
        private void InitVariableLayers(Map map)
        {
            LayerGroup lyrGrp = null;
            VectorLayer lyr = null;

            // group layer with single target + labels
            lyrGrp = new LayerGroup("Fast Boats Group");
            lyr = CreateGeometryFeatureProviderLayer("Fast Boats", new[] {
                    new System.Data.DataColumn("Name",typeof(string)),
                    new System.Data.DataColumn("Heading",typeof(float)),
                    new System.Data.DataColumn("Scale",typeof(float)),
                    new System.Data.DataColumn("ARGB",typeof(int))
                });
            lyr.Style.PointColor = new SolidBrush(Color.Green);
            var llyr = CreateLabelLayer(lyr, "Name", true);
            _fastBoats = new MovingObjects(7, lyr, llyr, map, 0.8f, Color.Green);
            _fastBoats.AddObject("Fast 1あああ", GetRectangleCenter(map, MapDecorationAnchor.LeftTop));  //(Fast Boats Labels)ラベル追加「Fast 1」
            InitRasterPointSymbolizer(lyr, 0);
            lyrGrp.Layers.Add(lyr);
            lyrGrp.Layers.Add(llyr);
            map.VariableLayers.Add(lyrGrp);
        }

        //★ラベルレイヤ生成 (ターゲットのレイヤ、カラム、有効判定)
        private LabelLayer CreateLabelLayer(VectorLayer lyr, string column, bool enabled)
        {
            var lblLayer = new LabelLayer( lyr.LayerName + " Labels");
            lblLayer.DataSource = lyr.DataSource;
            lblLayer.LabelColumn = column;
            //lblLayer.Style.BackColor = Brushes.LightPink;
            lblLayer.SRID = lblLayer.SRID;
            lblLayer.Enabled = enabled;
            return lblLayer;
        }

        //初期化 背景レイヤ
        private void InitBackground(Map map)
        {
            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("baseLayer");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
            //baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\ne_10m_coastline\ne_10m_coastline.shp");
            //baseLayer.DataSource = new ShapeFile(@"ShapeFiles\polbnda_jpn\polbnda_jpn.shp");

            baseLayer.Style.Fill = Brushes.LimeGreen;
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            baseLayer.SRID = 3857;
            map.BackgroundLayer.Add(baseLayer);
        }


        //シンボルのスタイル設定
        private void InitRasterPointSymbolizer(VectorLayer lyr, int style)
        {
            if (style == 1)
                lyr.Theme = new SharpMap.Rendering.Thematics.CustomTheme(GetRasterPointSymbolizerStyle);
            else
                lyr.Theme = null;
        }
        private static readonly object _boatKey = new object();
        private static VectorStyle GetRasterPointSymbolizerStyle(FeatureDataRow row)
        {
            // NB - this is for testing only.
            RasterPointSymbolizer rps;
            lock (_boatKey)
            {
                rps = new RasterPointSymbolizer()
                {
                    Symbol = (Image) _boat.Clone(),
                    Rotation = (float) row[2], //図形の回転
                    RemapColor = Color.White,
                    Scale = (float) row[3],
                    SymbolColor = Color.FromArgb((int) row[4])
                };
            }

            return new VectorStyle() {PointSymbolizer = rps};
        }

        //レイヤ生成(レイヤ名,カラム(特徴情報)の見出し)
        private static VectorLayer CreateGeometryFeatureProviderLayer(string name, System.Data.DataColumn[] columns)
        {
            var fdt = new FeatureDataTable();
            fdt.Columns.Add("Oid", typeof(uint));
            var con = new System.Data.UniqueConstraint(fdt.Columns[0]);
            con.Columns[0].AutoIncrement = true;
            con.Columns[0].AutoIncrementSeed = 1000;
            fdt.Constraints.Add(con);
            fdt.PrimaryKey = new System.Data.DataColumn[]{fdt.Columns[0]};

            fdt.Columns.AddRange(columns);

            return new VectorLayer(name, new GeometryFeatureProvider(fdt));
        }

        //四角の枠の配置を設定
        private static Coordinate[] GetRectanglePoints(Map map, MapDecorationAnchor anchor)
        {
            var env = map.Envelope;
            env.ExpandBy(-env.Width * 0.05);

            var coords = new Coordinate[5];

            Coordinate tl = null;
            switch (anchor)
            {
                case MapDecorationAnchor.LeftTop:
                    tl = env.TopLeft();
                    break;
                case MapDecorationAnchor.LeftCenter:
                    tl = new Coordinate(env.MinX, env.MaxY - env.Height * 0.375);
                    break;
                case MapDecorationAnchor.LeftBottom:
                    tl = new Coordinate(env.MinX, env.MinY + env.Height * 0.25);
                    break;
                case MapDecorationAnchor.CenterTop:
                    tl = new Coordinate(env.Centre.X - env.Width * 0.125, env.MaxY);
                    break;
                case MapDecorationAnchor.CenterBottom:
                    tl = new Coordinate(env.Centre.X - env.Width * 0.125, env.MinY + env.Height * 0.25);
                    break;
                case MapDecorationAnchor.RightTop:
                    tl = new Coordinate(env.MaxX - env.Width * 0.25, env.MaxY);
                    break;
                case MapDecorationAnchor.RightCenter:
                    tl = new Coordinate(env.MaxX - env.Width * 0.25, env.MaxY - env.Height * 0.375);
                    break;
                case MapDecorationAnchor.RightBottom:
                    tl = new Coordinate(env.MaxX - env.Width * 0.25, env.MinY + env.Height * 0.25);
                    break;
                default:
                    tl = new Coordinate(env.Centre.X - env.Width * 0.125, env.Centre.Y + env.Height * 0.125);
                    break;
            }

            coords[0] = tl;
            coords[1] = new Coordinate(tl.X + env.Width * 0.25, coords[0].Y);
            coords[2] = new Coordinate(coords[1].X, tl.Y - env.Height * 0.25);
            coords[3] = new Coordinate(coords[0].X, coords[2].Y);
            coords[4] = tl;

            for (int i=0;  i<5;i++)
            {
                coords[i] = new Coordinate(coords[i].X+135, coords[i].Y+36);
            }

            return coords;
        }

        //四角の枠の中心を取得
        private Point GetRectangleCenter(Map map, MapDecorationAnchor anchor)
        {
            var coords = GetRectanglePoints(map, anchor);
            return new Point((coords[0].X + coords[1].X) / 2.0,
                             (coords[0].Y + coords[2].Y) / 2.0);
        }

        //点のシンボルのスタイルを取得
        public static VectorStyle GetCharacterPointStyle(FeatureDataRow row)
         {
            var cps = new CharacterPointSymbolizer();
            cps.CharacterIndex = (int)row[1];
            cps.Font = new System.Drawing.Font("Wingdings", (float)row[2]);
            cps.Offset = new System.Drawing.PointF((float)row[3], (float)row[4]);
            return new VectorStyle() {PointSymbolizer = cps};
         }

    }


}

