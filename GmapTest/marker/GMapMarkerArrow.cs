
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using GMap.NET;
using System.Drawing;
/// <summary>
/// 该文件来源自CSDN金左手，几处方法可能因版本不同进行了修改
/// http://blog.csdn.net/qq_17371831
/// 再次特别感谢！
/// </summary>
namespace GMapMarkerExt.Markers
{
    [Serializable]
    public class GMapMarkerArrow : GMapMarker, ISerializable
    {
        [NonSerialized]
        private Brush _fill = null;

        static readonly Point[] Arrow = new Point[] { new Point(-7, 7), new Point(0, -22), new Point(7, 7), new Point(0, 2) };

        private float _bearing = 0;
        private float _scale = 1;

        public Brush Fill
        {
            get { return this._fill; }
            set { this._fill = value; }
        }

        public float Bearing
        {
            get { return this._bearing; }
            set { this._bearing = value; }
        }

        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;

                Size = new System.Drawing.Size((int)(14 * _scale), (int)(14 * _scale));
                Offset = new System.Drawing.Point(-Size.Width / 2, (int)(-Size.Height / 1.4));
            }
        }

        public GMapMarkerArrow(PointLatLng p, float scale = 1)
            : base(p)
        {
            this._fill = new SolidBrush(Color.FromArgb(155, Color.Blue));
            this._scale = scale;
        }

        public override void OnRender(Graphics g)
        {
            {
                g.TranslateTransform(ToolTipPosition.X, ToolTipPosition.Y);
                var c = g.BeginContainer();
                {
                    g.RotateTransform(this._bearing - Overlay.Control.Bearing);
                    g.ScaleTransform(this._scale, this._scale);

                    g.FillPolygon(this._fill, Arrow);
                }
                g.EndContainer(c);
                g.TranslateTransform(-ToolTipPosition.X, -ToolTipPosition.Y);
            }
        }

        public override void Dispose()
        {
            if (Fill != null)
            {
                Fill.Dispose();
                Fill = null;
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        protected GMapMarkerArrow(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}