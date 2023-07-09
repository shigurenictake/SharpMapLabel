using GeoAPI.Geometries;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration;
using System.Drawing;
using System.Windows.Forms;

namespace SharpMapLabel
{
    public partial class MainForm : Form
    {
        //コンストラクタ
        public MainForm()
        {
            InitializeComponent();
        }

        //イベント フォーム ロード完了 
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            var map = InitMap();
            InitBackground(map);
            InitVariableLayers(map);

            this.mb.Map = map;
            this.mb.Map.Center = new Coordinate(135, 36);
            this.mb.Refresh();
        }

        //マップ初期化
        private Map InitMap()
        {
            var res = new SharpMap.Map();

            res.Decorations.Add(new NorthArrow {ForeColor = Color.DarkSlateBlue}); //方位記号
            return res;
        }

        //★初期化 バリアブルレイヤ
        private void InitVariableLayers(Map map)
        {
            LayerGroup lyrGrp = null;
            VectorLayer lyr = null;

            // group layer with single target + labels
            lyrGrp = new LayerGroup("Fast Boats Group");
            //ターゲットレイヤの生成 {カラム1,カラム2,…}
            string name = "Fast Boats";
            System.Data.DataColumn[] columns = new[] { new System.Data.DataColumn("Name", typeof(string)) };
            {
                var fdt = new FeatureDataTable();

                fdt.Columns.AddRange(columns);
                lyr = new VectorLayer(name, new GeometryFeatureProvider(fdt));
            }

            lyr.Style.PointColor = new SolidBrush(Color.Yellow);
            //ラベルレイヤ生成、ターゲットレイヤに紐づけ
            LabelLayer llyr;
            string column = "Name";
            bool enabled = true;
            {
                var lblLayer = new LabelLayer(lyr.LayerName + " Labels");
                lblLayer.DataSource = lyr.DataSource;

                lblLayer.LabelColumn = column;
                lblLayer.Enabled = enabled;

                llyr = lblLayer;
            }

            //文字列をポイント(ジオメトリ)に付与
            name = "Fast 1あああ";
            //ポイント(ジオメトリ)生成
            NetTopologySuite.Geometries.Point startAt = new NetTopologySuite.Geometries.Point(new Coordinate(135.3, 36));
            //オブジェクト生成
            {
                var fp = (GeometryFeatureProvider)lyr.DataSource;
                var fdr = fp.Features.NewRow();
                //カラムと同順に情報を設定
                fdr[0] = name; //★Name ラベル
                fdr.Geometry = startAt; //ジオメトリを設定
                fp.Features.AddRow(fdr);
            }

            //グループレイヤにまとめてマップに追加
            lyrGrp.Layers.Add(lyr);
            lyrGrp.Layers.Add(llyr);
            map.VariableLayers.Add(lyrGrp);
        }

        //初期化 背景レイヤ
        private void InitBackground(Map map)
        {
            //レイヤーの作成
            VectorLayer baseLayer = new VectorLayer("baseLayer");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");

            baseLayer.Style.Fill = Brushes.LimeGreen;
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;

            map.BackgroundLayer.Add(baseLayer);
        }
    }
}

