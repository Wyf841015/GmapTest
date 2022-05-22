using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using GMap.NET.WindowsForms;
using System.Drawing;
using GMap.NET;
/// <summary>
/// 该文件来源自CSDN金左手，几处方法可能因版本不同进行了修改
/// http://blog.csdn.net/qq_17371831
/// 再次特别感谢！
/// </summary>
namespace GMapMarkerExt.Markers
{
    [Serializable]
    public class GMapMarkerEllipse : GMapMarker, ISerializable
    {
        [NonSerialized]
        private Brush _fill = null;
        [NonSerialized]
        private GMapMarker _innerMarker = null;
        [NonSerialized]
        private System.Drawing.Pen _pen = null;

        public Brush Fill { get { return _fill; } set { _fill = value; } }
        public GMapMarker InnerMarker { get { return _innerMarker; } set { _innerMarker = value; } }
        public Pen Pen { get { return _pen; } set { _pen = value; } }

        public GMapMarkerEllipse(PointLatLng pointLatLng)
            : base(pointLatLng)
        {
            this._fill = new SolidBrush(Color.FromArgb(0xff, Color.Blue));
            this._pen = new System.Drawing.Pen(Brushes.Blue, 2f);
            base.Size = new Size(10, 10);
            base.Offset = new Point(-base.Size.Width / 2, -base.Size.Height / 2);
        }

        public GMapMarkerEllipse(PointLatLng pointLatLng, Size size, Pen pen, Brush fill)
            : base(pointLatLng)
        {
            this._fill = new SolidBrush(Color.FromArgb(0xff, Color.Blue));
            this._pen = new System.Drawing.Pen(Brushes.Blue, 2f);
            base.Size = size;
            base.Offset = new Point(-base.Size.Width / 2, -base.Size.Height / 2);
        }

        protected GMapMarkerEllipse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._fill = new SolidBrush(Color.FromArgb(0xff, Color.Blue));
        }

        protected GMapMarkerEllipse(SerializationInfo info, StreamingContext context, Brush fill = null)
            : base(info, context)
        {
            if (fill == null)
                this._fill = new SolidBrush(Color.FromArgb(0xff, Color.Blue));
            else
                this._fill = fill;
        }

        public override void Dispose()
        {
            if (this._pen != null)
            {
                this._pen.Dispose();
                this._pen = null;
            }
            if (this._innerMarker != null)
            {
                this._innerMarker.Dispose();
                this._innerMarker = null;
            }
            base.Dispose();
        }

        public override void OnRender(Graphics graphics)
        {
            Rectangle rect = new Rectangle(base.LocalPosition.X, base.LocalPosition.Y, base.Size.Width, base.Size.Height);
            graphics.DrawEllipse(this._pen, rect);
            graphics.FillEllipse(this._fill, rect);
        }

        #region ISerializable
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}