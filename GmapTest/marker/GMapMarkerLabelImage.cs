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
    public class GMapMarkerLabelImage : GMapMarker, ISerializable
    {
        private string _labelInfo = string.Empty;
        private int _minimumShowLabelZoom = 15;
        private int _maximumShowLabelZoom = 21;

        /// <summary>
        /// 标注颜色
        /// </summary>
        [NonSerialized]
        private Brush _labelBrush = null;
        /// <summary>
        /// 标注字体
        /// </summary>
        [NonSerialized]
        private Font _labelFont = null;
        /// <summary>
        /// 名称
        /// </summary>
        [NonSerialized]
        private string _name = string.Empty;

        [NonSerialized]
        private Image _image = null;
        [NonSerialized]
        private Pen _pen = null;
        [NonSerialized]
        private Pen _outPen = null;

        public string LabelInfo
        {
            get { return this._labelInfo; }
            set { _labelInfo = value; }
        }
        public string Name
        {
            get { return this._name; }
            set { _labelInfo = value; }
        }
        /// <summary>
        /// 最小显示级别
        /// </summary>
        public int MinimumShowLabelZoom
        {
            get { return this._minimumShowLabelZoom; }
            set
            {
                if (value >= 0 && value < 21)
                    this._minimumShowLabelZoom = value;
            }
        }

        /// <summary>
        /// 最大显示级别
        /// </summary>
        public int MaximumShowLabelZoom
        {
            get { return this._maximumShowLabelZoom; }
            set
            {
                if (value >= 0 && value < 21)
                    this._maximumShowLabelZoom = value;
            }
        }

        public Brush LabelBrush
        {
            get { return _labelBrush; }
            set { this._labelBrush = value; }
        }

        public Font LabelFont
        {
            get { return this._labelFont; }
            set { this._labelFont = value; }
        }

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                if (_image != null)
                {
                    this.Size = new Size(_image.Width, _image.Height);
                }
            }
        }

        public Pen Pen
        {
            get { return _pen; }
            set { this._pen = value; }
        }

        public Pen OutPen
        {
            get { return _outPen; }
            set { this._outPen = value; }
        }


        public GMapMarkerLabelImage(PointLatLng pointLatLng, Image image, Pen pen = null, Pen outPen = null,string name= "GMapMarker")
            : base(pointLatLng)
        {
            this._labelBrush = Brushes.Salmon;
            this._labelFont = new Font("simsun", 9.0f);
            this._image = image;
            if (this._image != null)
                this.Size = new Size(this._image.Width, this._image.Height);
            this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
            this._pen = pen;
            this._outPen = outPen;
            this._name = name;
        }

        public override void OnRender(Graphics g)
        {
            if (_image == null)
                return;

            Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            g.DrawImage(_image, rect);

            if (_pen != null)
            {
                g.DrawRectangle(_pen, rect);
            }

            if (_outPen != null)
            {
                g.DrawEllipse(_outPen, rect);
            }

            if (!string.IsNullOrEmpty(_labelInfo))
            {
                if (this.Overlay.Control.Zoom >= MinimumShowLabelZoom && this.Overlay.Control.Zoom <= MaximumShowLabelZoom)
                {
                    SizeF lbSize = g.MeasureString(_labelInfo, _labelFont);
                    g.DrawString(LabelInfo, LabelFont, LabelBrush, LocalPosition.X + 10 - lbSize.Width / 2, LocalPosition.Y + this.Offset.Y);
                }
            }
        }

        public override void Dispose()
        {
            if (Pen != null)
            {
                Pen.Dispose();
                Pen = null;
            }

            if (OutPen != null)
            {
                OutPen.Dispose();
                OutPen = null;
            }

            base.Dispose();
        }
    }
}