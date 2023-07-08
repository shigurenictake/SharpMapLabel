using GeoAPI.Geometries;
using SharpMap;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Point = NetTopologySuite.Geometries.Point;

namespace SharpMapLabel
{
    //オブジェククラス
    public class MovingObjects
    {
        private readonly List<MovingObject> _movingObjects = new List<MovingObject>();

        private readonly VectorLayer _lyr;
        private readonly LabelLayer _llyr;
        private readonly Map _map;

        public double StepSize { get; set; }
        private float _scale;
        private Color _color;

        public MovingObjects(double stepSize, VectorLayer lyr, LabelLayer llyr, Map map, float scale, Color color)
        {
            StepSize = stepSize;
            _lyr = lyr;
            _llyr = llyr;
            _map = map;
            _scale = scale;
            _color = color;
        }

        //オブジェクト追加
        public void AddObject(string name, Point startAt)
        {
            lock (((ICollection)_movingObjects).SyncRoot)
            {
                var fp = (GeometryFeatureProvider)_lyr.DataSource;
                var fdr = fp.Features.NewRow();
                //カラムと同順に情報を設定
                fdr[1] = name; //★Name ラベル
                fdr[2] = _scale; //Scale
                fdr[3] = _color.ToArgb(); //ARGB
                fdr.Geometry = startAt;
                fp.Features.AddRow(fdr);
                fp.Features.AcceptChanges();

                var obj = new MovingObject(Convert.ToUInt32(fdr[0]), startAt);
                _movingObjects.Add(obj);
            }
        }

        public bool DeleteObject(uint oid)
        {
            lock (((ICollection)_movingObjects).SyncRoot)
            {
                var obj = _movingObjects.Find(p => p.Oid == oid);
                if (obj == null) return false;

                var fp = (GeometryFeatureProvider)_lyr.DataSource;
                var fdr = fp.Features.Rows.Find(oid);
                fp.Features.Rows.Remove(fdr);
                fp.Features.AcceptChanges();

                _movingObjects.Remove(obj);
                return true;
            }
        }
    }

    public class MovingObject
    {
        public uint Oid { get; }

        public Point Position { get; private set; }

        public MovingObject(uint oid, Point startAt)
        {
            Oid = oid;
            Position = startAt;
        }
    }
}
