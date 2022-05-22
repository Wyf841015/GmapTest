using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 该文件来源自CSDN金左手，几处方法可能因版本不同进行了修改
/// http://blog.csdn.net/qq_17371831
/// 再次特别感谢！
/// </summary>
namespace GMapMarkerExt.Markers
{
    [Serializable]
    public class GMapMarkerCircle : GMapMarker, ISerializable
    {
        [NonSerialized]
        private Brush _fill = null;
        private bool _isFilled = false;
        private int _radius = 0;
        [NonSerialized]
        private Pen _stroke = null;

        public Brush Fill
        {
            get { return _fill; }
            set { this._fill = value; }
        }

        public bool IsMeter
        {
            get; set;
        }

        public bool IsFilled
        {
            get { return this._isFilled; }
            set { this._isFilled = value; }
        }

        public int Radius
        {
            get { return this._radius; }
            set { this._radius = value; }
        }

        public Pen Stroke
        {
            get { return _stroke; }
            set { this._stroke = value; }
        }

        public GMapMarkerCircle(PointLatLng pointLatLng)
            : base(pointLatLng)
        {
            this._stroke = new Pen(Color.FromArgb(0x9b, Color.MidnightBlue));
            this._fill = new SolidBrush(Color.FromArgb(0x9b, Color.AliceBlue));
            this._isFilled = true;
            this._radius = 0x378;
            base.IsHitTestVisible = false;
            base.ToolTip = new GMapToolTip(this);
        }

        protected GMapMarkerCircle(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._stroke = new Pen(Color.FromArgb(0x9b, Color.MidnightBlue));
            this._fill = new SolidBrush(Color.FromArgb(0x9b, Color.AliceBlue));
            this._isFilled = true;
        }

        public override void Dispose()
        {
            if (this._stroke != null)
            {
                this._stroke.Dispose();
                this._stroke = null;
            }
            if (this._fill != null)
            {
                this._fill.Dispose();
                this._fill = null;
            }
            base.Dispose();
        }

        public override void OnRender(Graphics g)
        {
            int width = ((int)(((double)this.Radius) / base.Overlay.Control.MapProvider.Projection.GetGroundResolution((int)base.Overlay.Control.Zoom, base.Position.Lat))) * 2;
            if (this._isFilled)
            {
                g.FillEllipse(this._fill, new Rectangle(base.LocalPosition.X - (width / 2), base.LocalPosition.Y - (width / 2), width, width));
            }
            g.DrawEllipse(this._stroke, new Rectangle(base.LocalPosition.X - (width / 2), base.LocalPosition.Y - (width / 2), width, width));
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}