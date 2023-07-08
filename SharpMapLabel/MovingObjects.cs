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
        private static readonly Random Rnd = new Random(17);

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

        public void Start() => IsRunning = true;

        public void Stop() => IsRunning = false;

        public bool IsRunning { get; private set; }

        //オブジェクト追加
        public void AddObject(string name, Point startAt)
        {
            lock (((ICollection)_movingObjects).SyncRoot)
            {
                var fp = (GeometryFeatureProvider)_lyr.DataSource;
                var fdr = fp.Features.NewRow();
                float heading = (float)Rnd.Next(0, 359);
                //カラムと同順に情報を設定
                fdr[1] = name; //★Name ラベル
                fdr[2] = MovingObject.NormalizePositive(90f - heading); //Heading
                fdr[3] = _scale; //Scale
                fdr[4] = _color.ToArgb(); //ARGB
                fdr.Geometry = startAt;
                fp.Features.AddRow(fdr);
                fp.Features.AcceptChanges();

                var obj = new MovingObject(Convert.ToUInt32(fdr[0]), startAt, heading);
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

        public float Heading { get; set; }

        public MovingObject(uint oid, Point startAt, float initialHeading)
        {
            Oid = oid;
            Position = startAt;
            Heading = initialHeading;
        }

        private const double DegToRad = Math.PI / 180d;

        public void Step(Envelope currentExtent, double stepSize)
        {
            double heading = DegToRad * Heading;
            double dx = Math.Cos(heading) * stepSize;
            double dy = Math.Sin(heading) * stepSize;

            var cs = Position.CoordinateSequence;
            cs.SetOrdinate(0, Ordinate.X, cs.GetOrdinate(0, Ordinate.X) + dx);
            cs.SetOrdinate(0, Ordinate.Y, cs.GetOrdinate(0, Ordinate.Y) + dy);
            Position.GeometryChanged();


            if (currentExtent.Contains(Position.Coordinate))
                return;

            if (Position.X < currentExtent.MinX || currentExtent.MaxX < Position.X)
            {
                dx = -dx;
            }
            else if (Position.Y < currentExtent.MinY || currentExtent.MinY < Position.Y)
            {
                dy = -dy;
            }

            Heading = NormalizePositive(90f - (float)Math.Atan2(dx, dy) * 180f / (float)Math.PI);
            //Step(currentExtent, stepSize);
        }

        internal static float NormalizePositive(float angle)
        {
            if (angle < 0.0)
            {
                while (angle < 0.0)
                    angle += 360f;
                if (angle >= 360f)
                    angle = 0.0f;
            }
            else
            {
                while (angle >= 360f)
                    angle -= 360f;
                if (angle < 0.0f)
                    angle = 0.0f;
            }
            return angle;
        }
    }
}
