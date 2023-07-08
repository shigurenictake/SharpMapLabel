using SharpMap.Data.Providers;
using SharpMap.Layers;
using System.Collections;
using System.Collections.Generic;

namespace SharpMapLabel
{
    //オブジェクトクラス
    public class MovingObjects
    {
        private readonly List<object> _movingObjects = new List<object>();

        private readonly VectorLayer _lyr;

        public MovingObjects(VectorLayer lyr)
        {
            _lyr = lyr;
        }

        //オブジェクト追加
        public void AddObject(string name, NetTopologySuite.Geometries.Point startAt)
        {
            lock (((ICollection)_movingObjects).SyncRoot)
            {
                var fp = (GeometryFeatureProvider)_lyr.DataSource;
                var fdr = fp.Features.NewRow();
                //カラムと同順に情報を設定
                fdr[0] = name; //★Name ラベル
                fdr.Geometry = startAt; //ジオメトリを設定
                fp.Features.AddRow(fdr);
            }
        }
    }
}
