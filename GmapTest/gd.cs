using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using System.Drawing;
using GMap.NET.WindowsForms;
namespace GmapTest
{
    public abstract class AMapProviderBase : GMapProvider
    {
        public AMapProviderBase()
        {
            MaxZoom = null;
            RefererUrl = "http://www.amap.com/";
            //Copyright = string.Format("©{0} 高德 Corporation, ©{0} NAVTEQ, ©{0} Image courtesy of NASA", DateTime.Today.Year);    
        }

        public override PureProjection Projection
        {
            get { return MercatorProjection.Instance; }
        }

        GMapProvider[] overlays;
        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { this };
                }
                return overlays;
            }
        }
    }

    public class AMapProvider : AMapProviderBase
    {
        public static readonly AMapProvider Instance;

        readonly Guid id = new Guid("EF3DD303-3F74-4938-BF40-232D0595EE88");
        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "AMap";
        public override string Name
        {
            get
            {
                return name;
            }
        }

        static AMapProvider()
        {
            Instance = new AMapProvider();
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            try
            {
                string url = MakeTileImageUrl(pos, zoom, LanguageStr);
                return GetTileImageUsingHttp(url);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            var num = (pos.X + pos.Y) % 4 + 1;
            //string url = string.Format(UrlFormat, num, pos.X, pos.Y, zoom);
            string url = string.Format(UrlFormat, pos.X, pos.Y, zoom);
            return url;
        }

        //static readonly string UrlFormat = "http://webrd04.is.autonavi.com/appmaptile?x={0}&y={1}&z={2}&lang=zh_cn&size=1&scale=1&style=7";
        static readonly string UrlFormat = "http://webrd01.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=7&x={0}&y={1}&z={2}";
    }
    //class GMapMarkerImage : GMapMarker
    //{
    //    private Image image;
    //    public Image Image
    //    {
    //        get
    //        {
    //            return image;
    //        }
    //        set
    //        {
    //            image = value;
    //            if (image != null)
    //            {
    //                this.Size = new Size(image.Width, image.Height);
    //            }
    //        }
    //    }

    //    public Pen Pen
    //    {
    //        get;
    //        set;
    //    }

    //    public Pen OutPen
    //    {
    //        get;
    //        set;
    //    }

    //    public GMapMarkerImage(GMap.NET.PointLatLng p, Image image)
    //        : base(p)
    //    {
    //        Size = new System.Drawing.Size(image.Width, image.Height);
    //        Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);
    //        this.image = image;
    //        Pen = null;
    //        OutPen = null;
    //    }

    //    public override void OnRender(Graphics g)
    //    {
    //        if (image == null)
    //            return;

    //        Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
    //        g.DrawImage(image, rect);

    //        if (Pen != null)
    //        {
    //            g.DrawRectangle(Pen, rect);
    //        }

    //        if (OutPen != null)
    //        {
    //            g.DrawEllipse(OutPen, rect);
    //        }
    //    }

    //    public override void Dispose()
    //    {
    //        if (Pen != null)
    //        {
    //            Pen.Dispose();
    //            Pen = null;
    //        }

    //        if (OutPen != null)
    //        {
    //            OutPen.Dispose();
    //            OutPen = null;
    //        }

    //        base.Dispose();
    //    }
    //}
}
