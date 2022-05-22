using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using GMap.NET.WindowsForms;
using System.Drawing;
using System.Timers;
using GMap.NET;
using System.ComponentModel;
using Svg;
using Sunny.UI;
using System.Drawing.Imaging;
/// <summary>
/// 该文件来源自CSDN金左手，几处方法可能因版本不同进行了修改，并填充了必要的功能
/// http://blog.csdn.net/qq_17371831
/// 再次特别感谢！
/// </summary>
namespace GMapMarkerExt.Markers
{
    [Serializable]
    public class GMapMarkerImage : GMapMarker, ISerializable
    {
        [NonSerialized]
        private Image _image = null;
        private bool _isHighlight = true;
        [NonSerialized]
        private Pen _highlightPen = null;
        [NonSerialized]
        private Pen _flashPen = null;
        [NonSerialized]
        private Timer _flashTimer = null;
        private int _radius;
        private int _flashRadius;
        private Guid guid;
        private string _name = null;
        private Color _color;
        private string _imagePath;
        private float _opacity;
        private Color _fillColor;
        private float _fillOpacity;
        //旋转角度
        private float _angle;
        [Browsable(true), Category("基本属性"), Description("ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("颜色")]
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {

                base.Size = value;
                if (_imagePath != null)
                {
                        SvgDocument svgDocument = SvgDocument.Open(_imagePath);
                    if (svgDocument != null)
                    {

                        SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                        svgElement.Transforms.Add(new Svg.Transforms.SvgRotate(this._angle, svgDocument.ViewBox.Height / 2, svgDocument.ViewBox.Width / 2));
                        svgElement.Stroke = new SvgColourServer(this._color);
                        svgElement.Opacity = this._opacity;
                        svgElement.Fill = new SvgColourServer(this._fillColor);
                        svgElement.FillOpacity = this._fillOpacity;

                        this._image = svgDocument.Draw(this.Size.Width, this.Size.Height);
                        //大小改变时，保持中心点位置不变
                        this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
                        this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
                    }
                    
                  
                }
            }

        }
        [Browsable(true), Category("基本属性"), Description("旋转角度")]
        public float Angle
        {
            get
            {
                return this._angle;
            }
            set
            {
               
                if(value<0||value>360)
                {
                    return;
                }
                 this._angle=value;
                if (_imagePath != null)
                {
                    SvgDocument svgDocument = SvgDocument.Open(_imagePath);
                    if (svgDocument != null)
                    {
                        SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                        svgElement.Transforms.Add(new Svg.Transforms.SvgRotate(this._angle, svgDocument.ViewBox.Height / 2, svgDocument.ViewBox.Width / 2));
                        svgElement.Stroke = new SvgColourServer(this._color);
                        svgElement.Opacity = this._opacity;
                        svgElement.Fill = new SvgColourServer(this._fillColor);
                        svgElement.FillOpacity = this._fillOpacity;

                        this._image = svgDocument.Draw(this.Size.Width, this.Size.Height);
                        //大小改变时，保持中心点位置不变
                        this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
                        this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
                    }
                   
                }
            }

        }
        [Browsable(true), Category("基本属性"), Description("颜色")]
        public  Color Color
        {
            get
            {
                return this._color;
            }
            set
            {

                this._color = value;

                if (_imagePath != null)
                {
                    SvgDocument svgDocument = SvgDocument.Open(_imagePath);
                    if (svgDocument != null)
                    {
                        SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                        svgElement.Transforms.Add(new Svg.Transforms.SvgRotate(this._angle, svgDocument.ViewBox.Height / 2, svgDocument.ViewBox.Width / 2));
                        svgElement.Stroke = new SvgColourServer(this._color);
                        svgElement.Opacity = this._opacity;
                        svgElement.Fill = new SvgColourServer(this._fillColor);
                        svgElement.FillOpacity = this._fillOpacity;

                        this._image = svgDocument.Draw(this.Size.Width, this.Size.Height);
                        //大小改变时，保持中心点位置不变
                        this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
                        this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
                    }
                   
                }
            }

        }
        [Browsable(true), Category("基本属性"), Description("透明度")]
        public float Opacity
        {
            get
            {
                return this._opacity*100;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                this._opacity = value/100;
                if (_imagePath != null)
                {
                    SvgDocument svgDocument = SvgDocument.Open(_imagePath);
                    if (svgDocument != null)
                    {
                        SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                        svgElement.Transforms.Add(new Svg.Transforms.SvgRotate(this._angle, svgDocument.ViewBox.Height / 2, svgDocument.ViewBox.Width / 2));
                        svgElement.Stroke = new SvgColourServer(this._color);
                        svgElement.Opacity = this._opacity;
                        svgElement.Fill = new SvgColourServer(this._fillColor);
                        svgElement.FillOpacity = this._fillOpacity;

                        this._image = svgDocument.Draw(this.Size.Width, this.Size.Height);
                        //大小改变时，保持中心点位置不变
                        this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
                        this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
                    }
                   
                }
            }

        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return this._fillColor;
            }
            set
            {

                this._fillColor = value;

                if (_imagePath != null)
                {
                    SvgDocument svgDocument = SvgDocument.Open(_imagePath);
                    if (svgDocument != null)
                    {
                        SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                        svgElement.Transforms.Add(new Svg.Transforms.SvgRotate(this._angle, svgDocument.ViewBox.Height / 2, svgDocument.ViewBox.Width / 2));
                        svgElement.Stroke = new SvgColourServer(this._color);
                        svgElement.Opacity = this._opacity;
                        svgElement.Fill = new SvgColourServer(this._fillColor);
                        svgElement.FillOpacity = this._fillOpacity;

                        this._image = svgDocument.Draw(this.Size.Width, this.Size.Height);
                        //大小改变时，保持中心点位置不变
                        this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
                        this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
                    }
                  
                }
            }

        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public float FillOpacity
        {
            get
            {
                return this._fillOpacity * 100;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                this._fillOpacity = value / 100;
                if (_imagePath != null)
                {
                    SvgDocument svgDocument = SvgDocument.Open(_imagePath);
                    if (svgDocument != null)
                    {
                        SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                        svgElement.Transforms.Add(new Svg.Transforms.SvgRotate(this._angle, svgDocument.ViewBox.Height / 2, svgDocument.ViewBox.Width / 2));
                        svgElement.Stroke = new SvgColourServer(this._color);
                        svgElement.Opacity = this._opacity;
                        svgElement.Fill = new SvgColourServer(this._fillColor);
                        svgElement.FillOpacity = this._fillOpacity;
                        
                        this._image = svgDocument.Draw(this.Size.Width, this.Size.Height);
                        //大小改变时，保持中心点位置不变
                        this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
                        this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
                    }
                    
                }
            }

        }
        [Browsable(true), Category("基本属性"), Description("是否可见")]
        public new bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }
            set
            {

                base.IsVisible = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("是否鼠标进入显示边框")]
        public bool IsHighlight
        {
            get { return this._isHighlight; }
            set { this._isHighlight = value; }
        }
        [Browsable(true), Category("基本属性"), Description("提示")]
        public new string ToolTipText
        {
            get
            {
                return base.ToolTipText;
            }
            set
            {

                base.ToolTipText = value;
            }

        }
        
        [Browsable(true), Category("基本属性"), Description("位置")]
        public new PointLatLng Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
            }


        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new Point ToolTipPosition
        {
            get { return base.ToolTipPosition; }
           // set { base.ToolTipPosition = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new GMapOverlay Overlay
        {
            get { return base.Overlay; }
            //set { base.Overlay = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new Point Offset
        {
            get { return base.Offset; }
            set { base.Offset = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new Rectangle LocalArea
        {
            get { return base.LocalArea; }
            //set { base.LocalArea = value; }
        }
            [Browsable(false), Category("基本属性"), Description("")]
        public new bool IsMouseOver
        {
            get { return base.IsMouseOver; }
            //set { base.IsMouseOver = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new Point LocalPosition
        {
            get { return base.LocalPosition ; }
            set { base.LocalPosition = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public Pen HighlightPen
        {
            get { return this._highlightPen; }
            set { this._highlightPen = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public Pen FlashPen
        {
            get { return this._flashPen; }
            set
            {
                this._flashPen = value;
            }
        }
        [Browsable(false), Category("基本属性"), Description("")]
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

        public GMapMarkerImage(PointLatLng pointLatLng, Guid guid, string imagePath,string  name= "GMapMarkerImage")
            : base(pointLatLng)
        {
            SvgDocument svgDocument = SvgDocument.Open(imagePath);
           // SvgColourServer scs = (SvgColourServer)svgDocument.Children[0].Stroke;
            if (svgDocument != null)
            {
                SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                this._color = ((SvgColourServer)(svgElement.Stroke)).Colour;
                this._fillOpacity = svgElement.FillOpacity;
                this._fillColor =((SvgColourServer)svgElement.Fill).Colour;


            }
            var bitmap = svgDocument.Draw(48, 48);
        
            this._image = bitmap; 
            if (this._image != null)
                this.Size = new Size(this._image.Width, this._image.Height);
            this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height / 2);
            this._highlightPen = new System.Drawing.Pen(Brushes.Red, 2);
            this._radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
            this._flashTimer = new Timer();
            _flashTimer.Interval = 10;
            this._name = name;
            this.guid = guid;
            this._imagePath = imagePath;
            this._opacity = 1;
            this._angle = 0;
            _flashTimer.Elapsed += flashTimer_Elapsed;
        }
        
        public void StartFlash()
        {
            this._flashTimer.Start();
        }

        private void flashTimer_Elapsed(object sender, EventArgs e)
        {
            if (this._flashPen == null)
            {
                _flashPen = new Pen(Brushes.Red, 3);
                this._flashRadius = this._radius;
            }
            else
            {
                this._flashRadius += this._radius / 4;
                if (this._flashRadius >= 2 * this._radius)
                {
                    this._flashRadius = this._radius;
                    FlashPen.Color = Color.FromArgb(255, Color.Red);
                }
                else
                {
                    Random rand = new Random();
                    int alpha = rand.Next(255);
                    FlashPen.Color = Color.FromArgb(alpha, Color.Red);
                }
            }
            this.Overlay.Control.Refresh();
        }

        public void StopFlash()
        {
            this._flashTimer.Stop();
            if (FlashPen != null)
            {
                FlashPen.Dispose();
                FlashPen = null;
            }
            this.Overlay.Control.Refresh();
        }

        public override void OnRender(Graphics g)
        {
            if (_image == null)
                return;

            Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            g.DrawImage(_image, rect);

            if (IsMouseOver && this._isHighlight)
            {
                g.DrawRectangle(this._highlightPen, rect);
            }

            if (this._flashPen != null)
            {
                g.DrawEllipse(this._flashPen,
                    new Rectangle(LocalPosition.X - this._flashRadius / 2 + Size.Width / 2, LocalPosition.Y - this._flashRadius / 2 + Size.Height / 2, _flashRadius, _flashRadius));
            }
        }
       
        public override void Dispose()
        {
            if (_highlightPen != null)
            {
                _highlightPen.Dispose();
                _highlightPen = null;
            }

            if (_flashPen != null)
            {
                _flashPen.Dispose();
                _flashPen = null;
            }

            base.Dispose();
        }
    }
}