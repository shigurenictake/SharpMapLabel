using GeoAPI.Geometries;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration;
using System.Drawing;
using System.Reflection;
using System.Security.Policy;
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
            var map = new SharpMap.Map(); //マップ初期化
            InitBackground(map);
            InitVariableLayer1(map);
            InitVariableLayer2(map);

            this.mb.Map = map;
            this.mb.Map.Center = new Coordinate(135, 36);
            this.mb.Refresh();
        }

        //初期化 背景レイヤ生成
        private void InitBackground(Map map)
        {
            VectorLayer baseLayer = new VectorLayer("baseLayer");
            baseLayer.DataSource = new ShapeFile(@"..\..\ShapeFiles\polbnda_jpn\polbnda_jpn.shp");
            baseLayer.Style.Fill = Brushes.LimeGreen;
            baseLayer.Style.Outline = Pens.Black;
            baseLayer.Style.EnableOutline = true;
            map.BackgroundLayer.Add(baseLayer);
        }

        //★初期化 バリアブルレイヤ
        private void InitVariableLayer1(Map map)
        {
            VectorLayer lyr = null;
            LabelLayer llyr = null;
            LayerGroup lyrGrp = null;

            {
                //ラベルに記載する文字列
                string strLabelText = "らべるテキスト１";

                //グループレイヤ生成
                lyrGrp = new LayerGroup("Test1");

                //カラム生成 {カラム1,カラム2,…}。 下記では0列目をラベル用とした
                System.Data.DataColumn[] columns = new[] { new System.Data.DataColumn("Label", typeof(string)) };

                //ターゲットレイヤ生成し、カラムを設定 (点などの図形はこのレイヤに描画される)
                {
                    var fdt = new FeatureDataTable();
                    fdt.Columns.AddRange(columns);
                    lyr = new VectorLayer(lyrGrp.LayerName + "Vectors", new GeometryFeatureProvider(fdt));
                }
                lyr.Style.PointColor = new SolidBrush(Color.Green);
                lyr.Enabled = true; //false = 非表示 , true = 表示

                //ラベルレイヤ生成、ターゲットレイヤのDataSourceとカラムを共有する
                {
                    var lblLayer = new LabelLayer(lyr.LayerName + "Labels");
                    lblLayer.DataSource = lyr.DataSource; //ターゲットレイヤのDataSourceを共有
                    lblLayer.LabelColumn = columns[0].ColumnName; //0列目をラベル用として紐づけ
                    llyr = lblLayer;
                }
                llyr.Style.BackColor = new SolidBrush(Color.FromArgb(128, 255, 255, 0));//ラベル背景色
                llyr.Style.ForeColor = Color.White;//ラベル文字色
                llyr.Style.Halo = new Pen(Color.Blue, 2); //文字のアウトライン
                llyr.Style.Font = new Font(FontFamily.GenericSerif, 16); //フォント,文字のサイズ
                                                                         //その他スタイル
                llyr.Style.CollisionDetection = false; //true = ラベルが衝突するときは片方を非表示 , false = ラベルが衝突しても重ねて表示
                llyr.Style.CollisionBuffer = new SizeF(10, 10); //衝突判定距離のバッファ
                llyr.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Center; //水平位置合わせ(Left,Center,Right)
                llyr.Style.VerticalAlignment = SharpMap.Styles.LabelStyle.VerticalAlignmentEnum.Middle; //垂直位置合わせ(Top,Middle,Bottom)
                llyr.Style.Offset = new PointF(25, 15);//描画位置のオフセット
                llyr.Style.IgnoreLength = true; //true=ライン上でラベルが長くなってもラベル表示 , false = ライン上でラベルが長くなった時ラベル非表示
                llyr.Style.Rotation = 90.0f; //回転
                llyr.Style.MaxVisible = 10; //このZOOM最大値以下であるならば表示する

                //オブジェクト生成
                {
                    //ポイント(ジオメトリ)生成
                    var geom = new NetTopologySuite.Geometries.Point(new Coordinate(134.7, 36.1)); //点
                    //データソースのFeaturesに新しい行を生成
                    var fp = (GeometryFeatureProvider)llyr.DataSource;
                    var fdr = fp.Features.NewRow();
                    fdr[0] = strLabelText; //★ 0列目にラベル(文字列)を代入
                    fdr.Geometry = geom; //ジオメトリを設定
                    fp.Features.AddRow(fdr);
                }

                {
                    //マルチポイント(ジオメトリ)生成
                    var geom = new NetTopologySuite.Geometries.MultiPoint(
                        new IPoint[] {
                        new NetTopologySuite.Geometries.Point(new Coordinate(135.30, 36.00)),
                        new NetTopologySuite.Geometries.Point(new Coordinate(135.35, 36.05)),
                        new NetTopologySuite.Geometries.Point(new Coordinate(135.40, 36.10)),
                            });//複数の点
                    var fp = (GeometryFeatureProvider)llyr.DataSource;
                    var fdr = fp.Features.NewRow();
                    fdr[0] = "あいう１"; //★ 0列目にラベル(文字列)を代入
                    fdr.Geometry = geom; //ジオメトリを設定
                    fp.Features.AddRow(fdr);
                }

                {
                    //ライン(ジオメトリ)生成
                    var geom = new NetTopologySuite.Geometries.LineString(new Coordinate[] { new Coordinate(135.0, 36), new Coordinate(135.1, 36.1) }); //線
                    var fp = (GeometryFeatureProvider)llyr.DataSource;
                    var fdr = fp.Features.NewRow();
                    fdr[0] = "かきく１"; //★ 0列目にラベル(文字列)を代入
                    fdr.Geometry = geom; //ジオメトリを設定
                    fp.Features.AddRow(fdr);
                }

                //グループレイヤにまとめてマップに追加(跡から追加したレイヤが上に重なる)
                lyrGrp.Layers.Add(lyr);
                lyrGrp.Layers.Add(llyr);
                map.VariableLayers.Add(lyrGrp);
            }
        }

        //★初期化 バリアブルレイヤ
        private void InitVariableLayer2(Map map)
        {
            VectorLayer lyr = null;
            LabelLayer llyr = null;
            LayerGroup lyrGrp = null;

            {
                //ラベルに記載する文字列
                string strLabelText = "らべるテキスト２";

                //グループレイヤ生成
                lyrGrp = new LayerGroup("Test2");

                //カラム生成 {カラム1,カラム2,…}。 下記では0列目をラベル用とした
                System.Data.DataColumn[] columns = new[] { new System.Data.DataColumn("Label", typeof(string)) };

                //ターゲットレイヤ生成し、カラムを設定 (点などの図形はこのレイヤに描画される)
                {
                    var fdt = new FeatureDataTable();
                    fdt.Columns.AddRange(columns);
                    lyr = new VectorLayer(lyrGrp.LayerName + "Vectors", new GeometryFeatureProvider(fdt));
                }
                lyr.Style.PointColor = new SolidBrush(Color.Green);
                lyr.Enabled = true; //false = 非表示 , true = 表示

                //ラベルレイヤ生成、ターゲットレイヤのDataSourceとカラムを共有する
                {
                    var lblLayer = new LabelLayer(lyr.LayerName + "Labels");
                    lblLayer.DataSource = lyr.DataSource; //ターゲットレイヤのDataSourceを共有
                    lblLayer.LabelColumn = columns[0].ColumnName; //0列目をラベル用として紐づけ
                    llyr = lblLayer;
                }
                llyr.Style.BackColor = new SolidBrush(Color.FromArgb(128, 255, 255, 0));//ラベル背景色
                llyr.Style.ForeColor = Color.White;//ラベル文字色
                llyr.Style.Halo = new Pen(Color.Blue, 2); //文字のアウトライン
                llyr.Style.Font = new Font(FontFamily.GenericSerif, 16); //フォント,文字のサイズ
                                                                         //その他スタイル
                llyr.Style.CollisionDetection = false; //true = ラベルが衝突するときは片方を非表示 , false = ラベルが衝突しても重ねて表示
                llyr.Style.CollisionBuffer = new SizeF(10, 10); //衝突判定距離のバッファ
                llyr.Style.HorizontalAlignment = SharpMap.Styles.LabelStyle.HorizontalAlignmentEnum.Center; //水平位置合わせ(Left,Center,Right)
                llyr.Style.VerticalAlignment = SharpMap.Styles.LabelStyle.VerticalAlignmentEnum.Middle; //垂直位置合わせ(Top,Middle,Bottom)
                llyr.Style.Offset = new PointF(25, 15);//描画位置のオフセット
                llyr.Style.IgnoreLength = true; //true=ライン上でラベルが長くなってもラベル表示 , false = ライン上でラベルが長くなった時ラベル非表示
                llyr.Style.Rotation = 45.0f; //回転
                llyr.Style.MaxVisible = 10; //このZOOM最大値以下であるならば表示する

                //オブジェクト生成
                {
                    //ポイント(ジオメトリ)生成
                    var geom = new NetTopologySuite.Geometries.Point(new Coordinate(134.75, 36.1)); //点
                    //データソースのFeaturesに新しい行を生成
                    var fp = (GeometryFeatureProvider)llyr.DataSource;
                    var fdr = fp.Features.NewRow();
                    fdr[0] = strLabelText; //★ 0列目にラベル(文字列)を代入
                    fdr.Geometry = geom; //ジオメトリを設定
                    fp.Features.AddRow(fdr);
                }

                {
                    //マルチポイント(ジオメトリ)生成
                    var geom = new NetTopologySuite.Geometries.MultiPoint(
                        new IPoint[] {
                        new NetTopologySuite.Geometries.Point(new Coordinate(135.35, 36.00)),
                        new NetTopologySuite.Geometries.Point(new Coordinate(135.40, 36.05)),
                        new NetTopologySuite.Geometries.Point(new Coordinate(135.45, 36.10)),
                            });//複数の点
                    var fp = (GeometryFeatureProvider)llyr.DataSource;
                    var fdr = fp.Features.NewRow();
                    fdr[0] = "あいう２"; //★ 0列目にラベル(文字列)を代入
                    fdr.Geometry = geom; //ジオメトリを設定
                    fp.Features.AddRow(fdr);
                }

                {
                    //ライン(ジオメトリ)生成
                    var geom = new NetTopologySuite.Geometries.LineString(new Coordinate[] { new Coordinate(135.05, 36), new Coordinate(135.15, 36.1) }); //線
                    var fp = (GeometryFeatureProvider)llyr.DataSource;
                    var fdr = fp.Features.NewRow();
                    fdr[0] = "かきく２"; //★ 0列目にラベル(文字列)を代入
                    fdr.Geometry = geom; //ジオメトリを設定
                    fp.Features.AddRow(fdr);
                }

                //グループレイヤにまとめてマップに追加(跡から追加したレイヤが上に重なる)
                lyrGrp.Layers.Add(lyr);
                lyrGrp.Layers.Add(llyr);
                map.VariableLayers.Add(lyrGrp);
            }
        }
    }
}

