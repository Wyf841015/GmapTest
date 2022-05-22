using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
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
    public class GMapMarkerText : GMapMarker, ISerializable
    {
        private string _tipText;
        [NonSerialized]
        private Brush _tipBrush = null;
        [NonSerialized]
        private Font _tipFont = null;
        [NonSerialized]
        private StringFormat _tipStringFormat = null;

        public string TipText
        {
            get
            {
                return this._tipText;
            }
            set
            {
                this._tipText = value;
            }
        }

        public Brush TipBrush
        {
            get { return this._tipBrush; }
            set { this._tipBrush = value; }
        }
        public Font TipFont
        {
            get { return this._tipFont; }
            set { this._tipFont = value; }
        }
        public StringFormat TipStringFormat
        {
            get { return this._tipStringFormat; }
            set { this._tipStringFormat = value; }
        }

        public GMapMarkerText(PointLatLng pointLatLng, string tipText)
            : base(pointLatLng)
        {
            this._tipFont = new Font(FontFamily.GenericSansSerif, 14f, FontStyle.Regular, GraphicsUnit.Pixel);
            this._tipBrush = new SolidBrush(Color.Navy);
            this._tipStringFormat = new StringFormat();
            this._tipText = tipText;
        }

        public override void Dispose()
        {
            if (_tipFont != null)
            {
                _tipFont.Dispose();
                _tipFont = null;
            }
            if (_tipBrush != null)
            {
                _tipBrush.Dispose();
                _tipBrush = null;
            }
            base.Dispose();
        }

        public override void OnRender(Graphics g)
        {
            Size size = g.MeasureString(this._tipText, this._tipFont).ToSize();
            Point point = new Point(base.LocalPosition.X - (size.Width / 2), base.LocalPosition.Y);
            g.DrawString(this._tipText, this._tipFont, this._tipBrush, (PointF)point, this._tipStringFormat);
        }
    }
}