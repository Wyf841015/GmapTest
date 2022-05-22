using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.Projections;
using GMap.NET.ObjectModel;
using GMap.NET.Internals;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using ProjNet;
using ProjNet.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.CoordinateSystems;
using GeoAPI.Operation;
using ProjNet.Converters;
using System.Drawing;
using System.Configuration;
using System.Data;
using System.ComponentModel;
using System.Drawing.Drawing2D;
/// <summary>
/// 该文件内容主要算法均来源自CSDN金左手，只做了很少很少很少很少的修改
/// 为每类图元增加了颜色、透明度、填充等基本属性
/// http://blog.csdn.net/qq_17371831
/// 再次特别感谢！
/// </summary>
namespace GmapTest
{
    public enum DrawTypes
    {
        // [SettingsDescription("点")]
        DOT = 1,
        //   [Description("折线")]
        POLYLINE = 2,
        //   [Description("多边形")]
        POLYGON = 3,
        NONE=4,
    }
    public enum LineTypes
    {
        [Description("曲线")]
        Curve = 1,
        [Description("圆弧")]
        Arc = 2,
        [Description("折线")]
        Polyline = 3,
        [Description("自由线")]
        FreehandLine = 4,
      }
    public class GeometryUtil
    {
        private const double EARTH_RADIUS = 6378.137;//地球半径

        /// <summary>
        /// 获取多边形几何中心点
        /// </summary>
        /// <param name="mapPolygon"></param>
        /// <returns></returns>
        public static PointLatLng GetCenterPoint(GMapPolygon mapPolygon)
        {
            if (mapPolygon == null)
            {
                return new PointLatLng();
            }
            List<GeoAPI.Geometries.Coordinate> coordinates = new List<GeoAPI.Geometries.Coordinate>();
            foreach (var value in mapPolygon.Points)
            {
                GeoAPI.Geometries.Coordinate coordinate = new GeoAPI.Geometries.Coordinate(value.Lng, value.Lat);
                coordinates.Add(coordinate);
            }
            if (mapPolygon.Points.Count == 2)
            {

                List<NetTopologySuite.Geometries.Coordinate> tmpCoordinate = new List<NetTopologySuite.Geometries.Coordinate>();
                foreach (var i in coordinates)
                {
                    NetTopologySuite.Geometries.Coordinate tmpC = new NetTopologySuite.Geometries.Coordinate(i.X, i.Y);
                    tmpCoordinate.Add(tmpC);

                }
                NetTopologySuite.Geometries.LineString lineString = new NetTopologySuite.Geometries.LineString(tmpCoordinate.ToArray());
                IPoint point = (IPoint)lineString.Centroid;
                return new PointLatLng(point.Y, point.X);
            }
            else
            {
                if (mapPolygon.Points[0].Lng != mapPolygon.Points[mapPolygon.Points.Count - 1].Lng ||
                mapPolygon.Points[0].Lat != mapPolygon.Points[mapPolygon.Points.Count - 1].Lat)
                {
                    coordinates.Add(new GeoAPI.Geometries.Coordinate(mapPolygon.Points[0].Lng, mapPolygon.Points[0].Lat));
                }
                List<NetTopologySuite.Geometries.Coordinate> tmpCoordinate = new List<NetTopologySuite.Geometries.Coordinate>();
                foreach (var i in coordinates)
                {
                    NetTopologySuite.Geometries.Coordinate tmpC = new NetTopologySuite.Geometries.Coordinate(i.X, i.Y);
                    tmpCoordinate.Add(tmpC);

                }
                NetTopologySuite.Geometries.LinearRing linearRing = new NetTopologySuite.Geometries.LinearRing(tmpCoordinate.ToArray());
                NetTopologySuite.Geometries.Polygon polygon = new NetTopologySuite.Geometries.Polygon(linearRing);
              //  IPoint point = (IPoint)polygon.Centroid;
                return new PointLatLng(polygon.Centroid.Y, polygon.Centroid.X);
            }
        }

        public static List<Coordinate> WGS84ToWebMercator(List<PointLatLng> pointLatLngs)
        {
            if (pointLatLngs == null)
                return null;

            IList<Coordinate> coors = PointsToCoords(pointLatLngs);

            NetTopologySuite.Geometries.PrecisionModel precisionModel = new NetTopologySuite.Geometries.PrecisionModel((double)PrecisionModels.Floating);
            GeoAPI.CoordinateSystems.ICoordinateSystem wgs84 = ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84;
            GeoAPI.CoordinateSystems.ICoordinateSystem webMercator = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator;


            int SRID_wgs84 = Convert.ToInt32(wgs84.AuthorityCode);    //WGS84 SRID
            int SRID_webMercator = Convert.ToInt32(webMercator.AuthorityCode);    //WebMercator SRID

            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory ctFact = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation transformation = ctFact.CreateFromCoordinateSystems(wgs84, webMercator);

            NetTopologySuite.Geometries.GeometryFactory factory_wgs84 = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_wgs84);

            IList<Coordinate> coords = transformation.MathTransform.TransformList(coors);

            //var gcgs2000 = CreateCoordinateSystemFromWkt("GEOGCS[\"China Geodetic Coordinate System 2000\",DATUM[\"China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"1024\"]],AUTHORITY[\"EPSG\",\"1043\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4490\"]]");
            //var gcgs4508 = CreateCoordinateSystemFromWkt("PROJCS[\"CGCS2000 / Gauss-Kruger CM 111E\",GEOGCS[\"China Geodetic Coordinate System 2000\",DATUM[\"China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"1024\"]],AUTHORITY[\"EPSG\",\"1043\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4490\"]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",111],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AUTHORITY[\"EPSG\",\"4508\"]]");
            //IList<Coordinate> temps = ConvertCoordinates(coors.ToList(), gcgs2000,gcgs4508);
            //IList<Coordinate> temps1= ConvertCoordinates(temps.ToList(), gcgs4508,gcgs2000);


            return coords.ToList();
        }

        public static List<PointLatLng> WGS84ToWebMercator2(List<PointLatLng> pointLatLngs)
        {
            List<Coordinate> temps = WGS84ToWebMercator(pointLatLngs);
            List<PointLatLng> points = CoordsToPoints(temps);
            return points;
        }

        public static List<PointLatLng> WebMercatorToWGS84(List<PointLatLng> pointLatLngs)
        {
            return ConvertCoordinates(pointLatLngs, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
        }

        public static List<PointLatLng> CoordsToPoints(List<Coordinate> coords)
        {
            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var entity in coords)
            {
                points.Add(new PointLatLng(entity.Y, entity.X));
            }
            return points;
        }

        public static List<Coordinate> PointsToCoords(List<PointLatLng> points)
        {
            List<Coordinate> coords = new List<Coordinate>();
            foreach (var entity in points)
            {
                coords.Add(new Coordinate(entity.Lng, entity.Lat));
            }
            return coords;
        }

        /// <summary>
        /// 默认创建EPSG:4490 国家2000大地坐标系
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public static GeoAPI.CoordinateSystems.ICoordinateSystem CreateCoordinateSystemFromWkt(string wkt)
        {
            if (string.IsNullOrEmpty(wkt))
                wkt = "GEOGCS[\"China Geodetic Coordinate System 2000\",DATUM[\"China_2000\",SPHEROID[\"CGCS2000\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"1024\"]],AUTHORITY[\"EPSG\",\"1043\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4490\"]]";
            try
            {
                ICoordinateSystemFactory coordinateSystemFactory = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
                GeoAPI.CoordinateSystems.ICoordinateSystem coordinateSystem = coordinateSystemFactory.CreateFromWkt(wkt);
                return coordinateSystem;
            }
            catch
            {
                return null;
            }
        }

        public static List<Coordinate> ConvertCoordinates(List<Coordinate> coordinates, ICoordinateSystem sourceCoordinateSystem, ICoordinateSystem targetCoordinateSystem)
        {
            if (coordinates == null || sourceCoordinateSystem == null || targetCoordinateSystem == null)
                throw new Exception("请输入正确的参数");


            NetTopologySuite.Geometries.PrecisionModel precisionModel = new NetTopologySuite.Geometries.PrecisionModel((double)GeoAPI.Geometries.PrecisionModels.Floating);


            int SRID_source = Convert.ToInt32(sourceCoordinateSystem.AuthorityCode);    //source SRID
            int SRID_target = Convert.ToInt32(targetCoordinateSystem.AuthorityCode);    //target SRID

            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory transformationFactory = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation transformation = transformationFactory.CreateFromCoordinateSystems(sourceCoordinateSystem, targetCoordinateSystem);

            //NetTopologySuite.Geometries.GeometryFactory factory_source = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_source);

            IList<Coordinate> coords = transformation.MathTransform.TransformList(coordinates);
            return coords.ToList();

        }

        public static List<PointLatLng> ConvertCoordinates(List<PointLatLng> coordinates, ICoordinateSystem sourceCoordinateSystem, ICoordinateSystem targetCoordinateSystem)
        {
            List<Coordinate> tempCoordinates = new List<Coordinate>();
            foreach (var entity in coordinates)
            {
                tempCoordinates.Add(new Coordinate(x: entity.Lng, y: entity.Lat));
            }


            if (coordinates == null || sourceCoordinateSystem == null || targetCoordinateSystem == null)
                throw new Exception("请输入正确的参数");


            NetTopologySuite.Geometries.PrecisionModel precisionModel = new NetTopologySuite.Geometries.PrecisionModel((double)GeoAPI.Geometries.PrecisionModels.Floating);


            int SRID_source = Convert.ToInt32(sourceCoordinateSystem.AuthorityCode);    //source SRID
            int SRID_target = Convert.ToInt32(targetCoordinateSystem.AuthorityCode);    //target SRID

            ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory transformationFactory = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation transformation = transformationFactory.CreateFromCoordinateSystems(sourceCoordinateSystem, targetCoordinateSystem);

            //NetTopologySuite.Geometries.GeometryFactory factory_source = new NetTopologySuite.Geometries.GeometryFactory(precisionModel, SRID_source);

            IList<Coordinate> coords = transformation.MathTransform.TransformList(tempCoordinates);

            List<PointLatLng> points = new List<PointLatLng>();
            foreach (var entity in coords)
            {
                if (double.IsNaN(entity.X) || double.IsNaN(entity.Y))
                    continue;
                points.Add(new PointLatLng(entity.Y, entity.X));
            }
            return points;

        }

        public static double GetArea(List<PointLatLng> pointLatLngs)
        {
            if (pointLatLngs == null)
            {
                return 0;
            }
            if (pointLatLngs.Count <= 2)
            {
                return 0;
            }

            List<GeoAPI.Geometries.Coordinate> coordinates = WGS84ToWebMercator(pointLatLngs);

            if (coordinates[0].X != coordinates[coordinates.Count - 1].X ||
                coordinates[0].Y != coordinates[coordinates.Count - 1].Y)
            {
                coordinates.Add(new GeoAPI.Geometries.Coordinate(coordinates[0].X, coordinates[0].Y));
            }
            List<NetTopologySuite.Geometries.Coordinate> tmpCoordinate = new List<NetTopologySuite.Geometries.Coordinate>();
            foreach (var i in coordinates)
            {
                NetTopologySuite.Geometries.Coordinate tmpC = new NetTopologySuite.Geometries.Coordinate(i.X, i.Y);
                tmpCoordinate.Add(tmpC);

            }
            NetTopologySuite.Geometries.LinearRing linearRing = new NetTopologySuite.Geometries.LinearRing(tmpCoordinate.ToArray());
            NetTopologySuite.Geometries.Polygon polygon = new NetTopologySuite.Geometries.Polygon(linearRing);
            IPoint point = (IPoint)polygon.Centroid;
            return polygon.Area;
        }

        public static double GetArea(GMapPolygon mapPolygon)
        {
            if (mapPolygon == null)
            {
                return 0;
            }
            return GetArea(mapPolygon.Points);
        }

        public static RectLatLng GetRegionMaxRect(GMapPolygon polygon)
        {
            double latMin = 90;
            double latMax = -90;
            double lngMin = 180;
            double lngMax = -180;
            foreach (var point in polygon.Points)
            {
                if (point.Lat < latMin)
                {
                    latMin = point.Lat;
                }
                if (point.Lat > latMax)
                {
                    latMax = point.Lat;
                }
                if (point.Lng < lngMin)
                {
                    lngMin = point.Lng;
                }
                if (point.Lng > lngMax)
                {
                    lngMax = point.Lng;
                }
            }

            return new RectLatLng(latMax, lngMin, lngMax - lngMin, latMax - latMin);
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            PointLatLng first = new PointLatLng(lat2, lng1);
            PointLatLng second = new PointLatLng(lat2, lng2);
            List<PointLatLng> pointLatLngs = new List<PointLatLng>();
            pointLatLngs.Add(first);
            pointLatLngs.Add(second);

            var coordinates = WGS84ToWebMercator(pointLatLngs);
            
            List<NetTopologySuite.Geometries.Coordinate> tmpCoordinate = new List<NetTopologySuite.Geometries.Coordinate>();
            foreach (var i in coordinates)
            {
                NetTopologySuite.Geometries.Coordinate tmpC = new NetTopologySuite.Geometries.Coordinate(i.X, i.Y);
                tmpCoordinate.Add(tmpC);

            }
            NetTopologySuite.Geometries.LineString lineString = new NetTopologySuite.Geometries.LineString(tmpCoordinate.ToArray());
            return lineString.Length;
        }

        public static double GetDistance2(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = DegreesToRadians(lat1);
            double radLat2 = DegreesToRadians(lat2);
            double a = radLat1 - radLat2;
            double b = DegreesToRadians(lng1) - DegreesToRadians(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 求B点经纬度
        /// </summary>
        /// <param name="A">已知点的经纬度</param>
        /// <param name="distance">AB两地的距离  单位km</param>
        /// <param name="angle">AB连线与正北方向的夹角（0~360）</param>
        /// <returns>B点的经纬度</returns>
        public static CustomLatLng GetCustomLatLng(CustomLatLng A, double distance, double angle)
        {
            double dx = distance * 1000 * Math.Sin(DegreesToRadians(angle));
            double dy = distance * 1000 * Math.Cos(DegreesToRadians(angle));

            double bjd = (dx / A.Ed + A.m_RadLo) * 180 / Math.PI;
            double bwd = (dy / A.Ec + A.m_RadLa) * 180 / Math.PI;
            return new CustomLatLng(bjd, bwd);
        }

        //度 转换成 弧度
        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        /// <summary>
        /// 弧度 转换成 度
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }

        /// <summary>
        /// 描述：以centerP为圆心，绘制半径为radius的圆
        /// </summary>
        /// <param name="gMapControl">overlay：图层</param>
        /// <param name="overlay">图层</param>
        /// <param name="centerP">圆心点</param>
        /// <param name="radius">圆半径(单位: km)</param>
        /// <param name="name">多边形id</param>
        public static GMapPolygon DrawEllipse(GMapControl gMapControl, GMapOverlay overlay, PointLatLng centerP, int radius, string name)
        {
            try
            {
                if (radius <= 0)
                    return null;

                List<PointLatLng> latLngs = new List<PointLatLng>();
                CustomLatLng centerLatLng = new CustomLatLng(centerP.Lng, centerP.Lat);

                // 0 - 360度 寻找半径为radius，圆心为centerP的圆上点的经纬度
                for (int i = 0; i < 360; i++)
                {
                    //获取目标经纬度
                    CustomLatLng tempLatLng = GetCustomLatLng(centerLatLng, radius, i);
                    //将自定义的经纬度类 转换成 标准经纬度类
                    PointLatLng p = new PointLatLng(tempLatLng.m_Latitude, tempLatLng.m_Longitude);
                    latLngs.Add(p);
                }

                //安全性检查
                if (latLngs.Count < 20)
                {
                    return null;
                }

                //通过绘制多边形的方式绘制圆
                GMapPolygon gpol = new GMapPolygon(latLngs, name);
                gpol.Stroke = new Pen(Color.Red, 1.0f);
                gpol.Fill = new SolidBrush(Color.FromArgb(20, Color.Red));
                gpol.IsHitTestVisible = true;
                overlay.Polygons.Add(gpol);
                return gpol;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }

    //自定义经纬度类
    public class CustomLatLng
    {
        public double Rc = 6378137;     //赤道半径
        public double Rj = 6356725;     //极半径
        public double m_LoDeg, m_LoMin, m_LoSec;
        public double m_LaDeg, m_LaMin, m_LaSec;
        public double m_Longitude, m_Latitude;
        public double m_RadLo, m_RadLa;
        public double Ec;
        public double Ed;
        public CustomLatLng(double longitude, double latitude)
        {
            m_LoDeg = (int)longitude;
            m_LoMin = (int)((longitude - m_LoDeg) * 60);
            m_LoSec = (longitude - m_LoDeg - m_LoMin / 60) * 3600;

            m_LaDeg = (int)latitude;
            m_LaMin = (int)((latitude - m_LaDeg) * 60);
            m_LaSec = (latitude - m_LaDeg - m_LaMin / 60) * 3600;

            m_Longitude = longitude;
            m_Latitude = latitude;
            m_RadLo = longitude * Math.PI / 180;
            m_RadLa = latitude * Math.PI / 180;
            Ec = Rj + (Rc - Rj) * (90 - m_Latitude) / 90;
            Ed = Ec * Math.Cos(m_RadLa);
        }
    }
    public interface IPlot
    {
        /// <summary>
        /// 是否为图元
        /// </summary>
        /// <returns></returns>
        bool IsPlot();

        /// <summary>
        /// 设置当前图元的点集数量
        /// </summary>
        /// <param name="value"></param>
        void SetPoints(PointLatLng[] value);

        /// <summary>
        /// 获取当前图元的点集
        /// </summary>
        /// <returns>图元的点集</returns>
        PointLatLng[] GetPoints();

        /// <summary>
        /// 获取当前图元的点集数量
        /// </summary>
        /// <returns>图元的点集的数量</returns>
        int GetPointCount();

        /// <summary>
        /// 更新某个索引的点
        /// </summary>
        /// <param name="point">点</param>
        /// <param name="index">位置</param>
        void UpdatePoint(PointLatLng point, int index);

        /// <summary>
        /// 更新最后一个点
        /// </summary>
        /// <param name="point">{ol.Coordinate}</param>
        void UpdateLastPoint(PointLatLng point);

        /// <summary>
        /// 图元绘制逻辑.各个图元用来覆盖
        /// </summary>
        void Generate();

        /// <summary>
        /// 图元结束绘制回调
        /// </summary>
        void FinishDrawing();
    }
    public class Constants
    {
        public static readonly double TWO_PI = Math.PI * 2;
        public static readonly double HALF_PI = Math.PI / 2;
        public static readonly int FITTING_COUNT = 100;
        public static readonly double ZERO_TOLERANCE = 0.0001;
    }
    public enum PlotTypes
    {
          [Description("点")]
        MARKER = 1,
         [Description("折线")]
        POLYLINE = 2,
           [Description("多边形")]
        POLYGON = 3,
          [Description("圆")]
        CIRCLE = 4,
           [Description("椭圆")]
        ELLIPSE = 5,
          [Description("矩形")]
        RECTANGLE = 6,
           [Description("弧线")]
        ARC = 7,
           [Description("进攻箭头")]
        ATTACK_ARROW = 8,
           [Description("曲线面")]
        CLOSED_CURVE = 9,
            [Description("曲线")]
        CURVE = 10,
           [Description("双箭头")]
        DOUBLE_ARROW = 11,
          [Description("细直箭头")]
        FINE_ARROW = 12,
            [Description("突击方向")]
        ASSAULT_DIRECTION = 13,
           [Description("自由线")]
        FREEHAND_LINE = 14,
           [Description("自由面")]
        FREEHAND_POLYGON = 15,
        //   [Description("聚集地")]
        GATHERING_PLACE = 16,
           [Description("弓形")]
        LUNE = 17,
           [Description("扇形")]
        SECTOR = 18,
           [Description("分队战斗行动")]
        SQUAD_COMBAT = 19,
          [Description("直箭头")]
        STRAIGHT_ARROW = 20,
           [Description("进攻箭头（尾）")]
        TAILED_ATTACK_ARROW = 21,
           [Description("分队战斗行动（尾）")]
        TAILED_SQUAD_COMBAT = 22,
        NONE = 23,

    }
    public class PlotUtil
    {
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        public static double Distance(PointLatLng pnt1, PointLatLng pnt2)
        {
            return Math.Sqrt(Math.Pow((pnt1.Lng - pnt2.Lng), 2) + Math.Pow((pnt1.Lat - pnt2.Lat), 2));
        }

        public static double WholeDistance(List<PointLatLng> points)
        {
            double _distance = 0.0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                _distance += Distance(points[i], points[i + 1]);
            }
            return _distance;
        }

        public static double GetBaseLength(List<PointLatLng> points)
        {
            return Math.Pow(WholeDistance(points), 0.99);
        }

        public static PointLatLng Mid(PointLatLng pnt1, PointLatLng pnt2)
        {
            return new PointLatLng((pnt1.Lat + pnt2.Lat) / 2, (pnt1.Lng + pnt2.Lng) / 2);
        }

        public static PointLatLng GetCircleCenterOfThreePoints(PointLatLng pnt1, PointLatLng pnt2, PointLatLng pnt3)
        {
            var pntA = new PointLatLng((pnt1.Lat + pnt2.Lat) / 2, (pnt1.Lng + pnt2.Lng) / 2);
            var pntB = new PointLatLng(pntA.Lat + pnt1.Lng - pnt2.Lng, pntA.Lng - pnt1.Lat + pnt2.Lat);
            var pntC = new PointLatLng((pnt1.Lat + pnt3.Lat) / 2, (pnt1.Lng + pnt3.Lng) / 2);
            var pntD = new PointLatLng(pntC.Lat + pnt1.Lng - pnt3.Lng, pntC.Lng - pnt1.Lat + pnt3.Lat);
            return GetIntersectPoint(pntA, pntB, pntC, pntD);
        }

        public static PointLatLng GetIntersectPoint(PointLatLng pntA, PointLatLng pntB, PointLatLng pntC, PointLatLng pntD)
        {
            if (pntA.Lat == pntB.Lat)
            {
                var f = (pntD.Lng - pntC.Lng) / (pntD.Lat - pntC.Lat);
                var x = f * (pntA.Lat - pntC.Lat) + pntC.Lng;
                var y = pntA.Lat;
                return new PointLatLng(y, x);
            }
            if (pntC.Lat == pntD.Lat)
            {
                var e = (pntB.Lng - pntA.Lng) / (pntB.Lat - pntA.Lat);
                var x = e * (pntC.Lat - pntA.Lat) + pntA.Lng;
                var y = pntC.Lat;
                return new PointLatLng(y, x);
            }
            else
            {
                var e = (pntB.Lng - pntA.Lng) / (pntB.Lat - pntA.Lat);
                var f = (pntD.Lng - pntC.Lng) / (pntD.Lat - pntC.Lat);
                var y = (e * pntA.Lat - pntA.Lng - f * pntC.Lat + pntC.Lng) / (e - f);
                var x = e * y - e * pntA.Lat + pntA.Lng;
                return new PointLatLng(y, x);
            }
        }

        /// <summary>
        /// 获取方位角
        /// </summary>
        /// <param name="startPnt">起点</param>
        /// <param name="endPnt">终点</param>
        public static double GetAzimuth(PointLatLng startPnt, PointLatLng endPnt)
        {
            double azimuth = 0.0;
            var angle = Math.Asin(Math.Abs(endPnt.Lat - startPnt.Lat) / Distance(startPnt, endPnt));
            if (endPnt.Lat >= startPnt.Lat && endPnt.Lng >= startPnt.Lng)
                azimuth = angle + Math.PI;
            else if (endPnt.Lat >= startPnt.Lat && endPnt.Lng < startPnt.Lng)
                azimuth = Constants.TWO_PI - angle;
            else if (endPnt.Lat < startPnt.Lat && endPnt.Lng < startPnt.Lng)
                azimuth = angle;
            else if (endPnt.Lat < startPnt.Lat && endPnt.Lng >= startPnt.Lng)
                azimuth = Math.PI - angle;
            return azimuth;
        }

        public static double GetAngleOfThreePoints(PointLatLng pntA, PointLatLng pntB, PointLatLng pntC)
        {
            var angle = GetAzimuth(pntB, pntA) - GetAzimuth(pntB, pntC);
            return (angle < 0 ? angle + Constants.TWO_PI : angle);
        }

        public static bool IsClockWise(PointLatLng pnt1, PointLatLng pnt2, PointLatLng pnt3)
        {
            return ((pnt3.Lat - pnt1.Lat) * (pnt2.Lng - pnt1.Lng) > (pnt2.Lat - pnt1.Lat) * (pnt3.Lng - pnt1.Lng));
        }

        public static PointLatLng GetPointOnLine(double t, PointLatLng startPnt, PointLatLng endPnt)
        {
            var x = startPnt.Lng + (t * (endPnt.Lng - startPnt.Lng));
            var y = startPnt.Lat + (t * (endPnt.Lat - startPnt.Lat));
            return new PointLatLng(y, x);
        }

        public static PointLatLng GetCubicValue(double t, PointLatLng startPnt, PointLatLng cPnt1, PointLatLng cPnt2, PointLatLng endPnt)
        {
            t = Math.Max(Math.Min(t, 1), 0);
            var tp = 1 - t;
            var t2 = t * t;
            var t3 = t2 * t;
            var tp2 = tp * tp;
            var tp3 = tp2 * tp;
            var x = (tp3 * startPnt.Lng) + (3 * tp2 * t * cPnt1.Lng) + (3 * tp * t2 * cPnt2.Lng) + (t3 * endPnt.Lng);
            var y = (tp3 * startPnt.Lat) + (3 * tp2 * t * cPnt1.Lat) + (3 * tp * t2 * cPnt2.Lat) + (t3 * endPnt.Lat);
            return new PointLatLng(y, x);
        }

        public static PointLatLng GetThirdPoint(PointLatLng startPnt, PointLatLng endPnt, double angle, double distance, bool clockWise)
        {
            var azimuth = GetAzimuth(startPnt, endPnt);
            var alpha = clockWise ? azimuth + angle : azimuth - angle;
            var dx = distance * Math.Cos(alpha);
            var dy = distance * Math.Sin(alpha);
            return new PointLatLng(endPnt.Lat + dy, endPnt.Lng + dx);
        }

        public static List<PointLatLng> GetArcPoints(PointLatLng center, double radius, double startAngle, double endAngle)
        {
            List<PointLatLng> pnts = new List<PointLatLng>();
            var angleDiff = endAngle - startAngle;
            angleDiff = angleDiff < 0 ? angleDiff + Constants.TWO_PI : angleDiff;
            for (var i = 0; i <= Constants.FITTING_COUNT; i++)
            {
                var angle = startAngle + angleDiff * i / Constants.FITTING_COUNT;
                double x = center.Lng + radius * Math.Cos(angle);
                double y = center.Lat + radius * Math.Sin(angle);
                pnts.Add(new PointLatLng(y, x));
            }
            return pnts;
        }

        public static PointLatLng[] GetBisectorNormals(double t, PointLatLng pnt1, PointLatLng pnt2, PointLatLng pnt3)
        {
            var normal = GetNormal(pnt1, pnt2, pnt3);
            var dist = Math.Sqrt(normal.Lng * normal.Lng + normal.Lat * normal.Lat);
            var uX = normal.Lng / dist;
            var uY = normal.Lat / dist;
            var d1 = Distance(pnt1, pnt2);
            var d2 = Distance(pnt2, pnt3);
            PointLatLng bisectorNormalRight;
            PointLatLng bisectorNormalLeft;

            if (dist > Constants.ZERO_TOLERANCE)
            {
                if (IsClockWise(pnt1, pnt2, pnt3))
                {
                    var dt = t * d1;
                    var x = pnt2.Lng - dt * uY;
                    var y = pnt2.Lat + dt * uX;
                    bisectorNormalRight = new PointLatLng(y, x);
                    dt = t * d2;
                    x = pnt2.Lng + dt * uY;
                    y = pnt2.Lat - dt * uX;
                    bisectorNormalLeft = new PointLatLng(y, x);
                }
                else
                {
                    var dt = t * d1;
                    var x = pnt2.Lng + dt * uY;
                    var y = pnt2.Lat - dt * uX;
                    bisectorNormalRight = new PointLatLng(y, x);
                    dt = t * d2;
                    x = pnt2.Lng - dt * uY;
                    y = pnt2.Lat + dt * uX;
                    bisectorNormalLeft = new PointLatLng(y, x);
                }
            }
            else
            {
                var x = pnt2.Lng + t * (pnt1.Lng - pnt2.Lng);
                var y = pnt2.Lat + t * (pnt1.Lat - pnt2.Lat);
                bisectorNormalRight = new PointLatLng(y, x);
                x = pnt2.Lng + t * (pnt3.Lng - pnt2.Lng);
                y = pnt2.Lat + t * (pnt3.Lat - pnt2.Lat);
                bisectorNormalLeft = new PointLatLng(y, x);
            }
            return new PointLatLng[] { bisectorNormalRight, bisectorNormalLeft };
        }

        public static PointLatLng GetNormal(PointLatLng pnt1, PointLatLng pnt2, PointLatLng pnt3)
        {
            var dX1 = pnt1.Lng - pnt2.Lng;
            var dY1 = pnt1.Lat - pnt2.Lat;
            var d1 = Math.Sqrt(dX1 * dX1 + dY1 * dY1);
            dX1 /= d1;
            dY1 /= d1;

            var dX2 = pnt3.Lng - pnt2.Lng;
            var dY2 = pnt3.Lat - pnt2.Lat;
            var d2 = Math.Sqrt(dX2 * dX2 + dY2 * dY2);
            dX2 /= d2;
            dY2 /= d2;

            var uX = dX1 + dX2;
            var uY = dY1 + dY2;
            return new PointLatLng(uY, uX);
        }

        public static List<PointLatLng> GetCurvePoints(double t, PointLatLng[] controlPoints)
        {
            var leftControl = GetLeftMostControlPoint(t, controlPoints);
            List<PointLatLng> normals = new List<PointLatLng> { leftControl };
            for (int i = 0; i < controlPoints.Length - 2; i++)
            {
                var pnt1 = controlPoints[i];
                var pnt2 = controlPoints[i + 1];
                var pnt3 = controlPoints[i + 2];
                var normalPoints = GetBisectorNormals(t, pnt1, pnt2, pnt3);
                normals = normals.Concat(normalPoints).ToList();
            }
            var rightControl = GetRightMostControlPoint(t, controlPoints);
            normals.Add(rightControl);
            List<PointLatLng> points = new List<PointLatLng>();
            for (int i = 0; i < controlPoints.Length - 1; i++)
            {
                var pnt1 = controlPoints[i];
                var pnt2 = controlPoints[i + 1];
                points.Add(pnt1);
                for (var k = 0.0; k < Constants.FITTING_COUNT; k++)
                {
                    PointLatLng pnt = GetCubicValue(k / Constants.FITTING_COUNT, pnt1, normals[i * 2], normals[i * 2 + 1], pnt2);
                    points.Add(pnt);
                }
                points.Add(pnt2);
            }
            return points;
        }

        public static PointLatLng GetLeftMostControlPoint(double t, PointLatLng[] controlPoints)
        {
            var pnt1 = controlPoints[0];
            var pnt2 = controlPoints[1];
            var pnt3 = controlPoints[2];
            var pnts = GetBisectorNormals(0, pnt1, pnt2, pnt3);
            var normalRight = pnts[0];
            var normal = GetNormal(pnt1, pnt2, pnt3);
            var dist = Math.Sqrt(normal.Lng * normal.Lng + normal.Lat * normal.Lat);
            double controlX = 0;
            double controlY = 0;
            if (dist > Constants.ZERO_TOLERANCE)
            {
                var arr_mid = Mid(pnt1, pnt2);
                var pX = pnt1.Lng - arr_mid.Lng;
                var pY = pnt1.Lat - arr_mid.Lat;

                var d1 = Distance(pnt1, pnt2);
                // normal at midpoint
                var n = 2.0 / d1;
                var nX = -n * pY;
                var nY = n * pX;

                // upper triangle of symmetric transform matrix
                var a11 = nX * nX - nY * nY;
                var a12 = 2 * nX * nY;
                var a22 = nY * nY - nX * nX;

                var dX = normalRight.Lng - arr_mid.Lng;
                var dY = normalRight.Lat - arr_mid.Lat;

                // coordinates of reflected vector
                controlX = arr_mid.Lng + a11 * dX + a12 * dY;
                controlY = arr_mid.Lat + a12 * dX + a22 * dY;
            }
            else
            {
                controlX = pnt1.Lng + t * (pnt2.Lng - pnt1.Lng);
                controlY = pnt1.Lat + t * (pnt2.Lat - pnt1.Lat);
            }
            return new PointLatLng(controlY, controlX);
        }

        public static PointLatLng GetRightMostControlPoint(double t, PointLatLng[] controlPoints)
        {
            var count = controlPoints.Length;
            var pnt1 = controlPoints[count - 3];
            var pnt2 = controlPoints[count - 2];
            var pnt3 = controlPoints[count - 1];
            var pnts = GetBisectorNormals(0, pnt1, pnt2, pnt3);
            var normalLeft = pnts[1];
            var normal = GetNormal(pnt1, pnt2, pnt3);
            var dist = Math.Sqrt(normal.Lng * normal.Lng + normal.Lat * normal.Lat);
            double controlX = 0;
            double controlY = 0;
            if (dist > Constants.ZERO_TOLERANCE)
            {
                var arr_mid = Mid(pnt2, pnt3);
                var pX = pnt3.Lng - arr_mid.Lng;
                var pY = pnt3.Lat - arr_mid.Lat;

                var d1 = Distance(pnt2, pnt3);
                // normal at midpoint
                var n = 2.0 / d1;
                var nX = -n * pY;
                var nY = n * pX;

                // upper triangle of symmetric transform matrix
                var a11 = nX * nX - nY * nY;
                var a12 = 2 * nX * nY;
                var a22 = nY * nY - nX * nX;

                var dX = normalLeft.Lng - arr_mid.Lng;
                var dY = normalLeft.Lat - arr_mid.Lat;

                // coordinates of reflected vector
                controlX = arr_mid.Lng + a11 * dX + a12 * dY;
                controlY = arr_mid.Lat + a12 * dX + a22 * dY;
            }
            else
            {
                controlX = pnt3.Lng + t * (pnt2.Lng - pnt3.Lng);
                controlY = pnt3.Lat + t * (pnt2.Lat - pnt3.Lat);
            }
            return new PointLatLng(controlY, controlX);
        }

        public static List<PointLatLng> GetBezierPoints(PointLatLng[] points)
        {
            if (points.Length <= 2)
                return points.ToList();

            List<PointLatLng> bezierPoints = new List<PointLatLng>();
            var n = points.Length - 1;
            for (var t = 0.0; t <= 1; t += 0.01)
            {
                double x = 0.0;
                double y = 0.0;
                for (var index = 0; index <= n; index++)
                {
                    var factor = GetBinomialFactor(n, index);
                    var a = Math.Pow(t, index);
                    var b = Math.Pow((1 - t), (n - index));
                    x += factor * a * b * points[index].Lng;
                    y += factor * a * b * points[index].Lat;
                }
                bezierPoints.Add(new PointLatLng(y, x));
            }
            bezierPoints.Add(points[n]);
            return bezierPoints;
        }

        public static double GetBinomialFactor(double n, double index)
        {
            return GetFactorial(n) / (GetFactorial(index) * GetFactorial(n - index));
        }

        public static double GetFactorial(double n)
        {
            if (n <= 1)
                return 1;
            if (n == 2)
                return 2;
            if (n == 3)
                return 6;
            if (n == 4)
                return 24;
            if (n == 5)
                return 120;
            var result = 1;
            for (var i = 1; i <= n; i++)
                result *= i;
            return result;
        }

        public static List<PointLatLng> GetQBSplinePoints(PointLatLng[] points)
        {
            if (points.Length <= 2)
                return points.ToList();

            int n = 2;

            List<PointLatLng> bSplinePoints = new List<PointLatLng>();
            int m = points.Length - n - 1;
            bSplinePoints.Add(points[0]);
            for (int i = 0; i <= m; i++)
            {
                for (var t = 0.0; t <= 1; t += 0.05)
                {
                    var x = 0.0;
                    var y = 0.0;
                    for (var k = 0; k <= n; k++)
                    {
                        var factor = GetQuadricBSplineFactor(k, t);
                        x += factor * points[i + k].Lng;
                        y += factor * points[i + k].Lat;
                    }
                    bSplinePoints.Add(new PointLatLng(y, x));
                }
            }
            bSplinePoints.Add(points[points.Length - 1]);
            return bSplinePoints;
        }

        public static double GetQuadricBSplineFactor(int k, double t)
        {
            if (k == 0)
                return Math.Pow(t - 1, 2) / 2;
            if (k == 1)
                return (-2 * Math.Pow(t, 2) + 2 * t + 1) / 2;
            if (k == 2)
                return Math.Pow(t, 2) / 2;
            return 0;
        }
    }
    //所有多边形的基类，主要是赋予GUID，便于存储、检索
    public class GmapPolgonExt:GMapPolygon
    {
        private Guid _guid;
     
        protected PointLatLng[] points;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public  Guid ID
        {
            get
            {
                return this._guid;
            }
            set
            {
                this._guid = value;
            }
        }
        /// <summary>
        /// 以下属性不显示
        /// </summary>
        [Browsable(false), Category("基本属性"), Description("")]
        public new GMapOverlay Overlay
        {
            get { return base.Overlay; }
            //set { base.Overlay = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new bool IsMouseOver
        {
            get { return base.IsMouseOver; }
            //set { base.Overlay = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new RouteStatusCode Status
        {
            get { return base.Status; }
            set { base.Status = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new string ErrorMessage
        {
            get { return base.ErrorMessage; }
            set { base.ErrorMessage = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new int ErrorCode
        {
            get { return base.ErrorCode; }
            set { base.ErrorCode = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new string WarningMessage
        {
            get { return base.WarningMessage; }
            set { base.WarningMessage = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new PointLatLng? From
        {
            get { return base.From; }
            //set { base.From = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new PointLatLng? To
        {
            get { return base.To; }
            // set { base.To = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new double Distance
        {
            get { return base.Distance; }
            // set { base.Distance= value; }
        }
        public GmapPolgonExt(PointLatLng[] points, Guid guid, string name = "Circle")
    : base(points.ToList(), name)
        {

            this._guid = guid;
            this.points = points;
            
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }


    }
        //所有多边形的基类，主要是赋予GUID，便于存储、检索
    public class GMapRouteExt : GMapRoute
    {
        private Guid _guid;
     
        protected PointLatLng[] points;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public  Guid ID
        {
            get
            {
                return this._guid;
            }
            set
            {
                this._guid = value;
            }
        }
        /// <summary>
        /// 以下属性不显示
        /// </summary>
        [Browsable(false), Category("基本属性"), Description("")]
        public new GMapOverlay Overlay
        {
            get { return base.Overlay; }
            //set { base.Overlay = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new bool IsMouseOver
        {
            get { return base.IsMouseOver; }
            //set { base.Overlay = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new RouteStatusCode Status
        {
            get { return base.Status; }
            set { base.Status = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new string ErrorMessage
        {
            get { return base.ErrorMessage; }
            set { base.ErrorMessage = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new int ErrorCode
        {
            get { return base.ErrorCode; }
            set { base.ErrorCode = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new string WarningMessage
        {
            get { return base.WarningMessage; }
            set { base.WarningMessage = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new PointLatLng? From
        {
            get { return base.From; }
            //set { base.From = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new PointLatLng? To
        {
            get { return base.To; }
            // set { base.To = value; }
        }
        [Browsable(false), Category("基本属性"), Description("")]
        public new double Distance
        {
            get { return base.Distance; }
            // set { base.Distance= value; }
        }
        public GMapRouteExt(PointLatLng[] points,  Guid guid, string name = "line") :
           base(points.ToList(), name)
        {
            this._guid = guid;
            this.points = points;

        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }


    }
    //点符号
    public class Marker : GMapMarkerExt.Markers.GMapMarkerLabelImage, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        public Marker(PointLatLng[] points, Image image,string name = "GatheringPlace")
            : base(points.Length > 0 ? points[0] : new PointLatLng(), image)
        {
            this.points = points;
            this.geo_type = "RootTest";

            this.type = PlotTypes.MARKER;
            this.fixPointCount = 1;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            var pnt = temps[0];
            this.Position = (pnt);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 圆弧
    /// </summary>
    public class Arc : GMapRouteExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;
       
        private PlotTypes type;
        private int fixPointCount = 0;
        private Guid guid;
        private DashStyle dashstyle;
        [Browsable(true), Category("基本属性"), Description("ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条类型")]
        public DashStyle LineDashStyle
        {
            get
            {
                return base.Stroke.DashStyle;
            }
            set
            {
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.DashStyle = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写"), ReadOnlyAttribute(false)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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


    public Arc(PointLatLng[] points, Guid guid, string name = "Arc")
            : base(points, guid,name)
        {
            this.points = points;
            this.geo_type = "RootTest";

            this.type = PlotTypes.ARC;
            this.fixPointCount = 3;
        this.guid = guid;
        this.SetPoints(points);

        }

        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            if (count == 2)
            {
                List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temps);
            }
            else
            {
                var pnt1 = this.points[0];
                var pnt2 = this.points[1];
                var pnt3 = this.points[2];
                var center = PlotUtil.GetCircleCenterOfThreePoints(pnt1, pnt2, pnt3);
                var radius = PlotUtil.Distance(pnt1, center);

                var angle1 = PlotUtil.GetAzimuth(pnt1, center);
                var angle2 = PlotUtil.GetAzimuth(pnt2, center);

                double startAngle = 0.0;
                double endAngle = 0.0;

                if (PlotUtil.IsClockWise(pnt1, pnt2, pnt3))
                {
                    startAngle = angle2;
                    endAngle = angle1;
                }
                else
                {
                    startAngle = angle1;
                    endAngle = angle2;
                }
                List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(PlotUtil.GetArcPoints(center, radius, startAngle, endAngle).ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temps);
            }
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 曲线
    /// </summary>
    public class Curve : GMapRouteExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private double t = 0.0;
      
        private LineTypes lineType;
        private Guid guid;
        private DashStyle dashstyle;
        [Browsable(true), Category("基本属性"), Description("ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条类型")]
        public DashStyle LineDashStyle
        {
            get
            {
                return base.Stroke.DashStyle;
            }
            set
            {
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.DashStyle = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写"), ReadOnlyAttribute(false)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public Curve(PointLatLng[] points,  Guid guid, string name = "Polyline") :
          base(points, guid,"Curve")
        {
            this.points = points;
            this.geo_type = "RootTest";
         
            this.type = PlotTypes.CURVE;
            this.t = 0.3; 
            this.guid = guid;
            this.SetPoints(points);
        }
        public LineTypes getLineType()
        {
            return this.lineType;
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            if (count == 2)
            {
                List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temps);
            }
            else
            {
                List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(PlotUtil.GetCurvePoints(this.t, this.points).ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temps);
            }
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 折线
    /// </summary>
    public class Polyline : GMapRouteExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;
       // private PlotTypes type;
        private PlotTypes type;
        private LineTypes lineType;
        private Guid guid;
        private DashStyle dashstyle;
        [Browsable(true), Category("基本属性"), Description("ID")]
        public  Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条类型")]
        public DashStyle LineDashStyle
        {
            get
            {
                return base.Stroke.DashStyle;
            }
            set
            {
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.DashStyle = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写"), ReadOnlyAttribute(false)]
                public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
     

        public Polyline(PointLatLng[] points,Guid guid, string name = "Polyline") :
            base(points, guid, name)
        {
            this.points = points;
            this.geo_type = "LineString";
           
            this.type = PlotTypes.POLYLINE;
            this.guid = guid;
            this.SetPoints(points);
        }
        public LineTypes getLineType()
        {
            return this.lineType;
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 自由线
    /// </summary>
    public class FreehandLine : GMapRouteExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private bool freehand = false;
        private Guid guid;
        private DashStyle dashstyle;
        [Browsable(true), Category("基本属性"), Description("ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条类型")]
        public DashStyle LineDashStyle
        {
            get
            {
                return base.Stroke.DashStyle;
            }
            set
            {
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.DashStyle = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写"), ReadOnlyAttribute(false)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public FreehandLine(PointLatLng[] points, Guid guid, string name = "FreehandLine") :
           base(points, guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.FREEHAND_LINE;
            this.freehand = true;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            List<PointLatLng> temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
   
    /// <summary>
    /// 圆
    /// </summary>
    public class Circle : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;
       
        private PlotTypes type;
        private int fixPointCount = 0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public new Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写"), ReadOnlyAttribute(false)]
        
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        

        public Circle(PointLatLng[] points,Guid guid,string name= "Circle")
            : base(points, guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.CIRCLE;
            this.fixPointCount = 2;
            this.SetPoints(points);
        }

        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var center = this.points[0];
            var radius = PlotUtil.Distance(center, this.points[1]);
            var circlePoints = this.GeneratePoints(center, radius);
            var temps = GeometryUtil.ConvertCoordinates(circlePoints.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        private PointLatLng[] GeneratePoints(PointLatLng center, double radius)
        {
            double x;
            double y;
            double angle;
            List<PointLatLng> points = new List<PointLatLng>();
            for (var i = 0; i <= Constants.FITTING_COUNT; i++)
            {
                angle = Math.PI * 2 * i / Constants.FITTING_COUNT;
                x = center.Lng + radius * Math.Cos(angle);
                y = center.Lat + radius * Math.Sin(angle);
                points.Add(new PointLatLng(y, x));
            }
            return points.ToArray();
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 椭圆
    /// </summary>
    public class Ellipse : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color=  value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
       

        public Ellipse(PointLatLng[] points,Guid guid,string name="Ellipse") :
            base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.ELLIPSE;
            this.fixPointCount = 2;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var pnt1 = this.points[0];
            var pnt2 = this.points[1];
            var center = PlotUtil.Mid(pnt1, pnt2);
            var majorRadius = Math.Abs((pnt1.Lng - pnt2.Lng) / 2);
            var minorRadius = Math.Abs((pnt1.Lat - pnt2.Lat) / 2);

            var elllipsePoints = this.GeneratePoints(center, majorRadius, minorRadius);
            var temps = GeometryUtil.ConvertCoordinates(elllipsePoints.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        private List<PointLatLng> GeneratePoints(PointLatLng center, double majorRadius, double minorRadius)
        {
            double x;
            double y;
            double angle;
            List<PointLatLng> points = new List<PointLatLng>();
            for (var i = 0; i <= Constants.FITTING_COUNT; i++)
            {
                angle = Math.PI * 2 * i / Constants.FITTING_COUNT;
                x = center.Lng + majorRadius * Math.Cos(angle);
                y = center.Lat + minorRadius * Math.Sin(angle);
                points.Add(new PointLatLng(y, x));
            }
            return points;
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 弓形
    /// </summary>
    public class Lune : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public Lune(PointLatLng[] points,Guid guid,string name="Lune")
            : base(points,guid,name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.LUNE;
            this.fixPointCount = 3;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            if (this.GetPointCount() < 2)
            {
                return;
            }
            var pnts = this.GetPoints().ToList();
            if (this.GetPointCount() == 2)
            {
                var mid = PlotUtil.Mid(pnts[0], pnts[1]);
                var d = PlotUtil.Distance(pnts[0], mid);
                var pnt = PlotUtil.GetThirdPoint(pnts[0], mid, Constants.HALF_PI, d, false);
                pnts.Add(pnt);
            }
            var pnt1 = pnts[0];
            var pnt2 = pnts[1];
            var pnt3 = pnts[2];
            var center = PlotUtil.GetCircleCenterOfThreePoints(pnt1, pnt2, pnt3);
            var radius = PlotUtil.Distance(pnt1, center);

            var angle1 = PlotUtil.GetAzimuth(pnt1, center);
            var angle2 = PlotUtil.GetAzimuth(pnt2, center);
            double startAngle = 0;
            double endAngle = 0;
            if (PlotUtil.IsClockWise(pnt1, pnt2, pnt3))
            {
                startAngle = angle2;
                endAngle = angle1;
            }
            else
            {
                startAngle = angle1;
                endAngle = angle2;
            }
            pnts = PlotUtil.GetArcPoints(center, radius, startAngle, endAngle);
            pnts.Add(pnts[0]);

            var temps = GeometryUtil.ConvertCoordinates(pnts.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 扇形
    /// </summary>
    public class Sector : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public Sector(PointLatLng[] points,Guid guid,string name= "Sector")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.SECTOR;
            this.fixPointCount = 3;
            this.SetPoints(points);
        }

        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            if (this.GetPointCount() < 2)
                return;
            if (this.GetPointCount() == 2)
            {
                var temp = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.AddRange(temp);
            }
            else
            {
                var pnts = this.GetPoints().ToList();
                var center = pnts[0];
                var pnt2 = pnts[1];
                var pnt3 = pnts[2];
                var radius = PlotUtil.Distance(pnt2, center);
                var startAngle = PlotUtil.GetAzimuth(pnt2, center);
                var endAngle = PlotUtil.GetAzimuth(pnt3, center);
                var pList = PlotUtil.GetArcPoints(center, radius, startAngle, endAngle);
                pList.AddRange(new PointLatLng[] { center, pList[0] });

                var temps = GeometryUtil.ConvertCoordinates(pList.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temps);
            }
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }

    /// <summary>
    /// 曲线面
    /// </summary>
    public class ClosedCurve : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private double t = 0.0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public ClosedCurve(PointLatLng[] points,Guid guid,string name="ClosedCurve")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.CLOSED_CURVE;
            this.t = 0.3;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            if (count == 2)
            {
                this.Points.Clear();
                var temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.AddRange(temps);
            }
            else
            {
                var pnts = this.GetPoints().ToList();
                pnts.AddRange(new PointLatLng[] { pnts[0], pnts[1] });
                var normals = new PointLatLng[] { };
                for (var i = 0; i < pnts.Count - 2; i++)
                {
                    var normalPoints = PlotUtil.GetBisectorNormals(this.t, pnts[i], pnts[i + 1], pnts[i + 2]);
                    normals = normals.Concat(normalPoints).ToArray();
                }
                var count1 = normals.Length;
                normals = new PointLatLng[] { normals[count1 - 1] }.Concat(normals).ToArray();

                var pList = new List<PointLatLng>();
                for (int i = 0; i < pnts.Count - 2; i++)
                {
                    var pnt1 = pnts[i];
                    var pnt2 = pnts[i + 1];
                    pList.Add(pnt1);
                    for (var k = 0.0; k <= Constants.FITTING_COUNT; k++)
                    {
                        var pnt = PlotUtil.GetCubicValue(k / Constants.FITTING_COUNT, pnt1, normals[i * 2], normals[i * 2 + 1], pnt2);
                        pList.Add(pnt);
                    }
                    pList.Add(pnt2);
                }
                var finnalTemps = GeometryUtil.ConvertCoordinates(pList, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(finnalTemps);
            }
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 多边形
    /// </summary>
    public class Polygon : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public Polygon(PointLatLng[] points,Guid guid,string name="Polygon")
            : base(points,guid, "Polygon")
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.POLYGON;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 矩形
    /// </summary>
    public class Rectangle : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public Rectangle(PointLatLng[] points,Guid guid,string name="Rectangle")
            : base(points,guid,name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.RECTANGLE;
            this.fixPointCount = 2;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            else
            {
                var pnt1 = this.points[0];
                var pnt2 = this.points[1];
                var xmin = Math.Min(pnt1.Lng, pnt2.Lng);
                var xmax = Math.Max(pnt1.Lng, pnt2.Lng);
                var ymin = Math.Min(pnt1.Lat, pnt2.Lat);
                var ymax = Math.Max(pnt1.Lat, pnt2.Lat);
                var tl = new PointLatLng(ymax, xmin);
                var tr = new PointLatLng(ymax, xmax);
                var br = new PointLatLng(ymin, xmax);
                var bl = new PointLatLng(ymin, xmin);

                var temps = GeometryUtil.ConvertCoordinates(new PointLatLng[] { tl, tr, br, bl }.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temps);
            }
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 自由面
    /// </summary>
    class FreehandPolygon : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private bool freehand;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public FreehandPolygon(PointLatLng[] points,Guid guid,string name = "FreehandPolygon")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.FREEHAND_POLYGON;
            this.freehand = true;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var temps = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 聚集地
    /// </summary>
    public class GatheringPlace : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        private double t = 0.0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public GatheringPlace(PointLatLng[] points,Guid guid, string name = "GatheringPlace")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.GATHERING_PLACE;
            this.t = 0.4;
            this.fixPointCount = 3;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var pnts = this.GetPoints();
            List<PointLatLng> pointList = pnts.ToList();
            if (pnts.Length < 2)
            {
                return;
            }
            if (this.GetPointCount() == 2)
            {
                PointLatLng mid1 = PlotUtil.Mid(pointList[0], pointList[1]);
                var d = PlotUtil.Distance(pointList[0], mid1) / 0.9;
                var pnt = PlotUtil.GetThirdPoint(pointList[0], mid1, Constants.HALF_PI, d, true);
                pointList = new PointLatLng[] { pointList[0], pnt, pointList[1] }.ToList();
            }
            var mid = PlotUtil.Mid(pointList[0], pointList[2]);
            pointList.AddRange(new PointLatLng[] { mid, pointList[0], pointList[1] });
            pnts = pointList.ToArray();
            var normals = new PointLatLng[] { };
            for (int i = 0; i < pnts.Length - 2; i++)
            {
                var pnt1 = pnts[i];
                var pnt2 = pnts[i + 1];
                var pnt3 = pnts[i + 2];
                var normalPoints = PlotUtil.GetBisectorNormals(this.t, pnt1, pnt2, pnt3);
                normals = normals.Concat(normalPoints).ToArray();
            }
            var count = normals.Length;
            normals = new PointLatLng[] { normals[count - 1] }.Concat(normals.Take(count - 1)).ToArray();
            List<PointLatLng> pList = new List<PointLatLng>();
            for (int i = 0; i < pnts.Length - 2; i++)
            {
                var pnt1 = pnts[i];
                var pnt2 = pnts[i + 1];
                pList.Add(pnt1);
                for (var k = 0.0; k <= Constants.FITTING_COUNT; k++)
                {
                    var pnt = PlotUtil.GetCubicValue(k / Constants.FITTING_COUNT, pnt1, normals[i * 2], normals[i * 2 + 1], pnt2);
                    pList.Add(pnt);
                }
                pList.Add(pnt2);
            }

            var temps = GeometryUtil.ConvertCoordinates(pList.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 钳击
    /// </summary>
    public class DoubleArrow : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private double headHeightFactor = 0.0;
        private double headWidthFactor = 0.0;
        private double neckHeightFactor = 0.15;
        private double neckWidthFactor = 0.15;
        private PointLatLng connPoint;
        private PointLatLng tempPoint4;
        private int fixPointCount = 0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public DoubleArrow(PointLatLng[] points,Guid guid, string name = "DoubleArrow")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.DOUBLE_ARROW;
            this.headHeightFactor = 0.25;
            this.headWidthFactor = 0.3;
            this.neckHeightFactor = 0.85;
            this.neckWidthFactor = 0.15;
            this.connPoint = PointLatLng.Empty;
            this.tempPoint4 = PointLatLng.Empty; ;
            this.fixPointCount = 4;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {
            List<PointLatLng> pointList = points.ToList();
            if (this.GetPointCount() == 3 && this.tempPoint4 != null)
                pointList.Add(this.tempPoint4);
            if (this.connPoint != null)
                pointList.Add(this.connPoint);
            this.points = pointList.ToArray();
        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            if (count == 2)
            {
                this.Points.Clear();
                var temp = GeometryUtil.ConvertCoordinates(points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.AddRange(temp);
                return;
            }
            var pnt1 = this.points[0];
            var pnt2 = this.points[1];
            var pnt3 = this.points[2];
            count = this.GetPointCount();
            if (count == 3)
                this.tempPoint4 = this.GetTempPoint4(pnt1, pnt2, pnt3);
            else
                this.tempPoint4 = this.points[3];
            if (count == 3 || count == 4)
                this.connPoint = PlotUtil.Mid(pnt1, pnt2);
            else
                this.connPoint = this.points[4];
            PointLatLng[] leftArrowPnts;
            PointLatLng[] rightArrowPnts;
            if (PlotUtil.IsClockWise(pnt1, pnt2, pnt3))
            {
                leftArrowPnts = this.GetArrowPoints(pnt1, this.connPoint, this.tempPoint4, false);
                rightArrowPnts = this.GetArrowPoints(this.connPoint, pnt2, pnt3, true);
            }
            else
            {
                leftArrowPnts = this.GetArrowPoints(pnt2, this.connPoint, pnt3, false);
                rightArrowPnts = this.GetArrowPoints(this.connPoint, pnt1, this.tempPoint4, true);
            }
            var m = leftArrowPnts.Length;
            var t = (m - 5) / 2;

            var llBodyPnts = leftArrowPnts.Take(t).ToArray();
            var lArrowPnts = leftArrowPnts.Skip(t).Take(5).ToArray();
            var lrBodyPnts = leftArrowPnts.Skip(t + 5).Take(m - (t + 5)).ToArray();

            var rlBodyPnts = rightArrowPnts.Take(t).ToArray();
            var rArrowPnts = rightArrowPnts.Skip(t).Take(5).ToArray();
            var rrBodyPnts = rightArrowPnts.Skip(t + 5).Take(m - (t + 5)).ToArray();

            rlBodyPnts = PlotUtil.GetBezierPoints(rlBodyPnts).ToArray();
            var bodyPnts = PlotUtil.GetBezierPoints(rrBodyPnts.Concat(llBodyPnts.Skip(1)).ToArray());
            lrBodyPnts = PlotUtil.GetBezierPoints(lrBodyPnts).ToArray();

            var pnts = rlBodyPnts.Concat(rArrowPnts.Concat(bodyPnts).Concat(lArrowPnts).Concat(lrBodyPnts).ToArray());
            var temps = GeometryUtil.ConvertCoordinates(pnts.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        private PointLatLng[] GetArrowPoints(PointLatLng pnt1, PointLatLng pnt2, PointLatLng pnt3, bool clockWise)
        {
            var midPnt = PlotUtil.Mid(pnt1, pnt2);
            var len = PlotUtil.Distance(midPnt, pnt3);
            var midPnt1 = PlotUtil.GetThirdPoint(pnt3, midPnt, 0, len * 0.3, true);
            var midPnt2 = PlotUtil.GetThirdPoint(pnt3, midPnt, 0, len * 0.5, true);
            //var midPnt3=PlotUtils.getThirdPoint(pnt3, midPnt, 0, len * 0.7, true);
            midPnt1 = PlotUtil.GetThirdPoint(midPnt, midPnt1, Constants.HALF_PI, len / 5, clockWise);
            midPnt2 = PlotUtil.GetThirdPoint(midPnt, midPnt2, Constants.HALF_PI, len / 4, clockWise);
            //midPnt3=PlotUtils.getThirdPoint(midPnt, midPnt3, Constants.HALF_PI, len / 5, clockWise);

            var points = new PointLatLng[] { midPnt, midPnt1, midPnt2, pnt3 };
            // 计算箭头部分
            var arrowPnts = this.GetArrowHeadPoints(points, this.headHeightFactor, this.headWidthFactor, this.neckHeightFactor, this.neckWidthFactor);
            var neckLeftPoint = arrowPnts[0];
            var neckRightPoint = arrowPnts[4];
            // 计算箭身部分
            var tailWidthFactor = PlotUtil.Distance(pnt1, pnt2) / PlotUtil.GetBaseLength(points.ToList()) / 2;
            var bodyPnts = this.GetArrowBodyPoints(points, neckLeftPoint, neckRightPoint, tailWidthFactor).ToArray();
            var n = bodyPnts.Length;
            var lPoints = bodyPnts.Take(n / 2).ToList();
            var rPoints = bodyPnts.Skip(n / 2).Take(n).ToList();
            lPoints.Add(neckLeftPoint);
            rPoints.Add(neckRightPoint);
            lPoints.Reverse();
            lPoints.Add(pnt2);
            rPoints.Reverse();
            rPoints.Add(pnt1);
            lPoints.Reverse();
            return lPoints.Concat(arrowPnts).Concat(rPoints).ToArray();
        }

        private PointLatLng[] GetArrowBodyPoints(PointLatLng[] points, PointLatLng neckLeft, PointLatLng neckRight, double tailWidthFactor)
        {
            var allLen = PlotUtil.WholeDistance(points.ToList());
            var len = PlotUtil.GetBaseLength(points.ToList());
            var tailWidth = len * tailWidthFactor;
            var neckWidth = PlotUtil.Distance(neckLeft, neckRight);
            var widthDif = (tailWidth - neckWidth) / 2;
            double tempLen = 0.0;
            List<PointLatLng> leftBodyPnts = new List<PointLatLng>();
            List<PointLatLng> rightBodyPnts = new List<PointLatLng>();
            for (var i = 1; i < points.Length - 1; i++)
            {
                var angle = PlotUtil.GetAngleOfThreePoints(points[i - 1], points[i], points[i + 1]) / 2;
                tempLen += PlotUtil.Distance(points[i - 1], points[i]);
                var w = (tailWidth / 2 - tempLen / allLen * widthDif) / Math.Sin(angle);
                var left = PlotUtil.GetThirdPoint(points[i - 1], points[i], Math.PI - angle, w, true);
                var right = PlotUtil.GetThirdPoint(points[i - 1], points[i], angle, w, false);
                leftBodyPnts.Add(left);
                rightBodyPnts.Add(right);
            }
            return leftBodyPnts.Concat(rightBodyPnts).ToArray();
        }

        private PointLatLng[] GetArrowHeadPoints(PointLatLng[] points, double headHeightFactor, double headWidthFactor, double neckHeightFactor, double neckWidthFactor)
        {
            var len = PlotUtil.GetBaseLength(points.ToList());
            var headHeight = len * headHeightFactor;
            var headPnt = points[points.Length - 1];
            //var tailWidth = PlotUtil.Distance(tailLeft, tailRight);
            var headWidth = headHeight * headWidthFactor;
            var neckWidth = headHeight * neckWidthFactor;
            var neckHeight = headHeight * neckHeightFactor;
            var headEndPnt = PlotUtil.GetThirdPoint(points[points.Length - 2], headPnt, 0, headHeight, true);
            var neckEndPnt = PlotUtil.GetThirdPoint(points[points.Length - 2], headPnt, 0, neckHeight, true);
            var headLeft = PlotUtil.GetThirdPoint(headPnt, headEndPnt, Constants.HALF_PI, headWidth, false);
            var headRight = PlotUtil.GetThirdPoint(headPnt, headEndPnt, Constants.HALF_PI, headWidth, true);
            var neckLeft = PlotUtil.GetThirdPoint(headPnt, neckEndPnt, Constants.HALF_PI, neckWidth, false);
            var neckRight = PlotUtil.GetThirdPoint(headPnt, neckEndPnt, Constants.HALF_PI, neckWidth, true);
            return new PointLatLng[] { neckLeft, headLeft, headPnt, headRight, neckRight };
        }

        private PointLatLng GetTempPoint4(PointLatLng linePnt1, PointLatLng linePnt2, PointLatLng point)
        {
            var midPnt = PlotUtil.Mid(linePnt1, linePnt2);
            var len = PlotUtil.Distance(midPnt, point);
            var angle = PlotUtil.GetAngleOfThreePoints(linePnt1, midPnt, point);
            PointLatLng symPnt;
            double distance1;
            double distance2;
            PointLatLng mid;
            if (angle < Constants.HALF_PI)
            {
                distance1 = len * Math.Sin(angle);
                distance2 = len * Math.Cos(angle);
                mid = PlotUtil.GetThirdPoint(linePnt1, midPnt, Constants.HALF_PI, distance1, false);
                symPnt = PlotUtil.GetThirdPoint(midPnt, mid, Constants.HALF_PI, distance2, true);
            }
            else if (angle >= Constants.HALF_PI && angle < Math.PI)
            {
                distance1 = len * Math.Sin(Math.PI - angle);
                distance2 = len * Math.Cos(Math.PI - angle);
                mid = PlotUtil.GetThirdPoint(linePnt1, midPnt, Constants.HALF_PI, distance1, false);
                symPnt = PlotUtil.GetThirdPoint(midPnt, mid, Constants.HALF_PI, distance2, false);
            }
            else if (angle >= Math.PI && angle < Math.PI * 1.5)
            {
                distance1 = len * Math.Sin(angle - Math.PI);
                distance2 = len * Math.Cos(angle - Math.PI);
                mid = PlotUtil.GetThirdPoint(linePnt1, midPnt, Constants.HALF_PI, distance1, true);
                symPnt = PlotUtil.GetThirdPoint(midPnt, mid, Constants.HALF_PI, distance2, true);
            }
            else
            {
                distance1 = len * Math.Sin(Math.PI * 2 - angle);
                distance2 = len * Math.Cos(Math.PI * 2 - angle);
                mid = PlotUtil.GetThirdPoint(linePnt1, midPnt, Constants.HALF_PI, distance1, true);
                symPnt = PlotUtil.GetThirdPoint(midPnt, mid, Constants.HALF_PI, distance2, false);
            }
            return symPnt;
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 直箭头
    /// </summary>
    public class StraightArrow : GMapRoute, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        private PlotTypes type;
        private int fixPointCount = 0;
        private double maxArrowLength = 0.0;
        private int arrowLengthScale = 5;


        public StraightArrow(PointLatLng[] points, string name = "StraightArrow")
            : base(points.ToList(), name)
        {
            this.points = points;
            this.geo_type = "RootTest";

            this.type = PlotTypes.STRAIGHT_ARROW;
            this.fixPointCount = 2;
            this.maxArrowLength = 3000000;
            this.arrowLengthScale = 5;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            if (this.GetPointCount() < 2)
            {
                return;
            }
            var pnts = this.GetPoints();
            var pnt1 = pnts[0];
            var pnt2 = pnts[1];
            var distance = PlotUtil.Distance(pnt1, pnt2);
            var len = distance / this.arrowLengthScale;
            len = len > this.maxArrowLength ? this.maxArrowLength : len;
            var leftPnt = PlotUtil.GetThirdPoint(pnt1, pnt2, Math.PI / 6, len, false);
            var rightPnt = PlotUtil.GetThirdPoint(pnt1, pnt2, Math.PI / 6, len, true);
            List<PointLatLng> tempPoints = new PointLatLng[] { pnt1, pnt2, leftPnt, pnt2, rightPnt }.ToList();
            var temps = GeometryUtil.ConvertCoordinates(tempPoints, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();

            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 细直箭头
    /// </summary>
    public class FineArrow : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        protected PlotTypes type = PlotTypes.FINE_ARROW;
        protected double tailWidthFactor = 0.15;
        protected double neckWidthFactor = 0.2;
        protected double headWidthFactor = 0.25;
        protected double headAngle = Math.PI / 8.5;
        protected double neckAngle = Math.PI / 13;
        private int fixPointCount = 2;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public FineArrow(PointLatLng[] points,Guid guid,string name="FineArrow") :
            base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.FINE_ARROW;
            this.tailWidthFactor = 0.15;
            this.neckWidthFactor = 0.2;
            this.headWidthFactor = 0.25;
            this.headAngle = Math.PI / 8.5;
            this.neckAngle = Math.PI / 13;
            this.fixPointCount = 2;
            this.SetPoints(points);
        }

        public void FinishDrawing()
        {

        }

        public void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var pnts = this.GetPoints();
            var pnt1 = pnts[0];
            var pnt2 = pnts[1];
            var len = PlotUtil.GetBaseLength(pnts.ToList());
            var tailWidth = len * this.tailWidthFactor;
            var neckWidth = len * this.neckWidthFactor;
            var headWidth = len * this.headWidthFactor;
            var tailLeft = PlotUtil.GetThirdPoint(pnt2, pnt1, Constants.HALF_PI, tailWidth, true);
            var tailRight = PlotUtil.GetThirdPoint(pnt2, pnt1, Constants.HALF_PI, tailWidth, false);
            var headLeft = PlotUtil.GetThirdPoint(pnt1, pnt2, this.headAngle, headWidth, false);
            var headRight = PlotUtil.GetThirdPoint(pnt1, pnt2, this.headAngle, headWidth, true);
            var neckLeft = PlotUtil.GetThirdPoint(pnt1, pnt2, this.neckAngle, neckWidth, false);
            var neckRight = PlotUtil.GetThirdPoint(pnt1, pnt2, this.neckAngle, neckWidth, true);
            var pList = new PointLatLng[] { tailLeft, neckLeft, headLeft, pnt2, headRight, neckRight, tailRight };

            var temps = GeometryUtil.ConvertCoordinates(pList.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        public int GetPointCount()
        {
            return this.points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 突击方向
    /// </summary>
    public class AssaultDirection : FineArrow
    {
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value;base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public AssaultDirection(PointLatLng[] points,Guid guid,string name="AssaultDirection")
            : base(points,guid,name)
        {
            this.type = PlotTypes.ASSAULT_DIRECTION;
            this.tailWidthFactor = 0.2;
            this.neckWidthFactor = 0.25;
            this.headWidthFactor = 0.3;
            this.headAngle = Math.PI / 4;
            this.neckAngle = Math.PI * 0.17741;
            this.guid = guid;
            this.SetPoints(points);
        }
    }
    /// <summary>
    /// 进攻方向
    /// </summary>
    public class AttackArrow : GmapPolgonExt, IPlot
    {
        protected string geo_type = string.Empty;
        protected PointLatLng[] points;

        protected PlotTypes type = PlotTypes.ATTACK_ARROW;
        protected double headHeightFactor = 0.18;
        protected double headWidthFactor = 0.3;
        protected double neckHeightFactor = 0.85;
        protected double neckWidthFactor = 0.15;
        protected double headTailFactor = 0.8;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public AttackArrow(PointLatLng[] points,Guid guid, string name = "AttackArrow")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.ATTACK_ARROW;
            this.headHeightFactor = 0.18;
            this.headWidthFactor = 0.3;
            this.neckHeightFactor = 0.85;
            this.neckWidthFactor = 0.15;
            this.headTailFactor = 0.8;
            this.SetPoints(points);
        }
        public void FinishDrawing()
        {

        }

        public virtual void Generate()
        {
            if (this.GetPointCount() < 2)
            {
                return;
            }
            if (this.GetPointCount() == 2)
            {
                var temp = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.Clear();
                this.Points.AddRange(temp);
                return;
            }
            var pnts = this.GetPoints();
            // 计算箭尾
            var tailLeft = pnts[0];
            var tailRight = pnts[1];
            if (PlotUtil.IsClockWise(pnts[0], pnts[1], pnts[2]))
            {
                tailLeft = pnts[1];
                tailRight = pnts[0];
            }
            var midTail = PlotUtil.Mid(tailLeft, tailRight);
            var bonePnts = new PointLatLng[] { midTail }.Concat(pnts.Skip(2)).ToArray();
            // 计算箭头
            var headPnts = this.GetArrowHeadPoints(bonePnts, tailLeft, tailRight);
            var neckLeft = headPnts[0];
            var neckRight = headPnts[4];
            var tailWidthFactor = PlotUtil.Distance(tailLeft, tailRight) / PlotUtil.GetBaseLength(bonePnts.ToList());
            // 计算箭身
            var bodyPnts = this.GetArrowBodyPoints(bonePnts, neckLeft, neckRight, tailWidthFactor);
            // 整合
            var count = bodyPnts.Length;
            var leftPnts = new PointLatLng[] { tailLeft }.Concat(bodyPnts.Take(count / 2)).ToList();
            leftPnts.RemoveAt(1);
            leftPnts.Add(neckLeft);
            var rightPnts = new PointLatLng[] { tailRight }.Concat(bodyPnts.Skip(count / 2).Take(bodyPnts.Length)).ToList();
            rightPnts.RemoveAt(1);
            rightPnts.Add(neckRight);

            leftPnts = PlotUtil.GetQBSplinePoints(leftPnts.ToArray());
            rightPnts = PlotUtil.GetQBSplinePoints(rightPnts.ToArray());

            //右侧曲线节点反转
            rightPnts.Reverse();

            List<PointLatLng> tempPoints = leftPnts.ToArray().Concat(headPnts).Concat(rightPnts).ToList();
            var temps = GeometryUtil.ConvertCoordinates(tempPoints, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        protected PointLatLng[] GetArrowHeadPoints(PointLatLng[] points, PointLatLng tailLeft, PointLatLng tailRight)
        {
            var len = PlotUtil.GetBaseLength(points.ToList());
            var headHeight = len * this.headHeightFactor;
            var headPnt = points[points.Length - 1];
            len = PlotUtil.Distance(headPnt, points[points.Length - 2]);
            var tailWidth = PlotUtil.Distance(tailLeft, tailRight);
            if (headHeight > tailWidth * this.headTailFactor)
            {
                headHeight = tailWidth * this.headTailFactor;
            }
            var headWidth = headHeight * this.headWidthFactor;
            var neckWidth = headHeight * this.neckWidthFactor;
            headHeight = headHeight > len ? len : headHeight;
            var neckHeight = headHeight * this.neckHeightFactor;
            var headEndPnt = PlotUtil.GetThirdPoint(points[points.Length - 2], headPnt, 0, headHeight, true);
            var neckEndPnt = PlotUtil.GetThirdPoint(points[points.Length - 2], headPnt, 0, neckHeight, true);
            var headLeft = PlotUtil.GetThirdPoint(headPnt, headEndPnt, Constants.HALF_PI, headWidth, false);
            var headRight = PlotUtil.GetThirdPoint(headPnt, headEndPnt, Constants.HALF_PI, headWidth, true);
            var neckLeft = PlotUtil.GetThirdPoint(headPnt, neckEndPnt, Constants.HALF_PI, neckWidth, false);
            var neckRight = PlotUtil.GetThirdPoint(headPnt, neckEndPnt, Constants.HALF_PI, neckWidth, true);
            return new PointLatLng[] { neckLeft, headLeft, headPnt, headRight, neckRight };
        }

        protected PointLatLng[] GetArrowBodyPoints(PointLatLng[] bonePnts, PointLatLng neckLeft, PointLatLng neckRight, double tailWidthFactor)
        {
            var allLen = PlotUtil.WholeDistance(points.ToList());
            var len = PlotUtil.GetBaseLength(points.ToList());
            var tailWidth = len * tailWidthFactor;
            var neckWidth = PlotUtil.Distance(neckLeft, neckRight);
            var widthDif = (tailWidth - neckWidth) / 2;
            var tempLen = 0.0;
            List<PointLatLng> leftBodyPnts = new List<PointLatLng>();
            List<PointLatLng> rightBodyPnts = new List<PointLatLng>();
            for (int i = 1; i < points.Length - 1; i++)
            {
                var angle = PlotUtil.GetAngleOfThreePoints(points[i - 1], points[i], points[i + 1]) / 2;
                tempLen += PlotUtil.Distance(points[i - 1], points[i]);
                var w = (tailWidth / 2 - tempLen / allLen * widthDif) / Math.Sin(angle);
                var left = PlotUtil.GetThirdPoint(points[i - 1], points[i], Math.PI - angle, w, true);
                var right = PlotUtil.GetThirdPoint(points[i - 1], points[i], angle, w, false);
                leftBodyPnts.Add(left);
                rightBodyPnts.Add(right);
            }
            return leftBodyPnts.Concat(rightBodyPnts).ToArray();
        }


        public int GetPointCount()
        {
            return points.Length;
        }

        public PointLatLng[] GetPoints()
        {
            return this.points;
        }

        public bool IsPlot()
        {
            return true;
        }

        public void SetPoints(PointLatLng[] value)
        {
            //将经纬度坐标转为投影坐标参与计算
            if (value != null)
                value = GeometryUtil.WGS84ToWebMercator2(value.ToList()).ToArray();
            this.points = (value != null) ? value : new PointLatLng[] { };
            if (this.points.Length >= 1)
                this.Generate();
        }

        public void UpdateLastPoint(PointLatLng point)
        {
            this.UpdatePoint(point, this.points.Length - 1);
        }

        public void UpdatePoint(PointLatLng point, int index)
        {
            if (index >= 0 && index < this.points.Length)
            {
                this.points[index] = point;
                this.Generate();
            }
        }
    }
    /// <summary>
    /// 进攻箭头（尾）
    /// </summary>
    public class TailedAttackArrow : AttackArrow
    {
        private double tailWidthFactor = 0.1;
        private double swallowTailFactor = 1;
        private PointLatLng swallowTailPnt;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public TailedAttackArrow(PointLatLng[] points, Guid guid,string name = "TailedAttackArrow")
            : base(points, guid,name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.TAILED_ATTACK_ARROW;
            this.headHeightFactor = 0.18;
            this.headWidthFactor = 0.3;
            this.neckHeightFactor = 0.85;
            this.neckWidthFactor = 0.15;
            this.tailWidthFactor = 0.1;
            this.headTailFactor = 0.8;
            this.swallowTailFactor = 1;
            this.swallowTailPnt = PointLatLng.Empty; ;
            this.SetPoints(points);
        }

        public override void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            if (this.GetPointCount() == 2)
            {
                this.Points.Clear();
                var temp = GeometryUtil.ConvertCoordinates(this.points.ToList(), ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);
                this.Points.AddRange(temp);
                return;
            }
            var pnts = this.GetPoints();
            var tailLeft = pnts[0];
            var tailRight = pnts[1];
            if (PlotUtil.IsClockWise(pnts[0], pnts[1], pnts[2]))
            {
                tailLeft = pnts[1];
                tailRight = pnts[0];
            }
            var midTail = PlotUtil.Mid(tailLeft, tailRight);
            var bonePnts = new PointLatLng[] { midTail }.Concat(pnts.Skip(2).Take(pnts.Length)).ToArray();
            var headPnts = this.GetArrowHeadPoints(bonePnts, tailLeft, tailRight);
            var neckLeft = headPnts[0];
            var neckRight = headPnts[4];
            var tailWidth = PlotUtil.Distance(tailLeft, tailRight);
            var allLen = PlotUtil.GetBaseLength(bonePnts.ToList());
            var len = allLen * this.tailWidthFactor * this.swallowTailFactor;
            this.swallowTailPnt = PlotUtil.GetThirdPoint(bonePnts[1], bonePnts[0], 0, len, true);
            var factor = tailWidth / allLen;
            var bodyPnts = this.GetArrowBodyPoints(bonePnts, neckLeft, neckRight, factor);
            count = bodyPnts.Length;
            var leftPnts = new PointLatLng[] { tailLeft }.Concat(bodyPnts.Take(count / 2)).ToList();
            leftPnts.RemoveAt(1);
            leftPnts.Add(neckLeft);
            var rightPnts = new PointLatLng[] { tailRight }.Concat(bodyPnts.Skip(count / 2)).ToList();
            rightPnts.RemoveAt(1);
            rightPnts.Add(neckRight);

            leftPnts = PlotUtil.GetQBSplinePoints(leftPnts.ToArray());
            rightPnts = PlotUtil.GetQBSplinePoints(rightPnts.ToArray());
            rightPnts.Reverse();


            List<PointLatLng> tempPoints = leftPnts.Concat(headPnts).Concat(rightPnts).Concat(new PointLatLng[] { swallowTailPnt, leftPnts[0] }).ToList();
            var temps = GeometryUtil.ConvertCoordinates(tempPoints, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);

            this.Points.Clear();
            this.Points.AddRange(temps);
        }
    }
    /// <summary>
    /// 分队战斗行动
    /// </summary>
    public class SquadCombat : AttackArrow
    {
        private double tailWidthFactor = 0.0;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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

        public SquadCombat(PointLatLng[] points,Guid guid,string name="SquadCombat")
            : base(points,guid,name)
        {
            this.type = PlotTypes.SQUAD_COMBAT;
            this.guid = guid;
            this.headHeightFactor = 0.18;
            this.headWidthFactor = 0.3;
            this.neckHeightFactor = 0.85;
            this.neckWidthFactor = 0.15;
            this.tailWidthFactor = 0.1;
            this.SetPoints(points);
        }
      
        public override void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var pnts = this.GetPoints();
            var tailPnts = this.GetTailPoints(pnts);
            var headPnts = this.GetArrowHeadPoints(pnts, tailPnts[0], tailPnts[1]);
            var neckLeft = headPnts[0];
            var neckRight = headPnts[4];
            var bodyPnts = this.GetArrowBodyPoints(pnts, neckLeft, neckRight, this.tailWidthFactor);
            count = bodyPnts.Length;
            var leftPnts = new PointLatLng[] { tailPnts[0] }.Concat(bodyPnts.Take(count / 2)).ToList();
            leftPnts.Add(neckLeft);
            var rightPnts = new PointLatLng[] { tailPnts[1] }.Concat(bodyPnts.Skip(count / 2).Take(count)).ToList();
            rightPnts.Add(neckRight);

            leftPnts = PlotUtil.GetQBSplinePoints(leftPnts.ToArray());
            rightPnts = PlotUtil.GetQBSplinePoints(rightPnts.ToArray());
            rightPnts.Reverse();

            List<PointLatLng> tempPoints = leftPnts.Concat(headPnts.Concat(rightPnts)).ToList();
            var temps = GeometryUtil.ConvertCoordinates(tempPoints, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);

            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        private PointLatLng[] GetTailPoints(PointLatLng[] pnts)
        {
            var allLen = PlotUtil.GetBaseLength(points.ToList());
            var tailWidth = allLen * this.tailWidthFactor;
            var tailLeft = PlotUtil.GetThirdPoint(points[1], points[0], Constants.HALF_PI, tailWidth, false);
            var tailRight = PlotUtil.GetThirdPoint(points[1], points[0], Constants.HALF_PI, tailWidth, true);
            return new PointLatLng[] { tailLeft, tailRight };
        }
    }
    /// <summary>
    /// 分队战斗行动（尾）
    /// </summary>
    public class TailedSquadCombat : AttackArrow
    {
        private double tailWidthFactor = 0.0;
        private double swallowTailFactor = 1;
        private PointLatLng swallowTailPnt;
        private Guid guid;
        [Browsable(true), Category("基本属性"), Description("标号ID")]
        public Guid ID
        {
            get { return this.guid; }
            set { this.guid = value; base.ID = value; }
        }
        [Browsable(true), Category("基本属性"), Description("名称")]
        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        [Browsable(true), Category("基本属性"), Description("线条颜色")]
        public Color LineColor
        {
            get
            {
                return base.Stroke.Color;
            }
            set
            {

                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条宽度")]
        public float LineWidth
        {
            get
            {
                return base.Stroke.Width;
            }
            set
            {
                if (value < 0)
                    value = 0;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Width = value;
            }

        }
        [Browsable(true), Category("基本属性"), Description("线条透明度")]
        public int LineOpaCity
        {
            get
            {
                return (int)Math.Ceiling((decimal)base.Stroke.Color.A * 100 / 255);
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Stroke = (Pen)base.Stroke.Clone();
                base.Stroke.Color = Color.FromArgb(value * 255 / 100, base.Stroke.Color.R, base.Stroke.Color.G, base.Stroke.Color.B);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充颜色")]
        public Color FillColor
        {
            get
            {
                return ((SolidBrush)base.Fill).Color;
            }
            set
            {
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(value);
            }
        }
        [Browsable(true), Category("基本属性"), Description("填充透明度")]
        public int FillOpaCity
        {
            get
            {

                return (((SolidBrush)base.Fill).Color.A) * 100 / 255;

            }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 100)
                    value = 100;
                base.Fill = (SolidBrush)base.Fill.Clone();
                base.Fill = new SolidBrush(Color.FromArgb(value * 255 / 100, ((SolidBrush)base.Fill).Color.R, ((SolidBrush)base.Fill).Color.G, ((SolidBrush)base.Fill).Color.B));
            }
        }
        [Browsable(true), Category("基本属性"), Description("点集合，不能改写")]
        //[ReadOnlyAttribute(true)]
        public List<PointLatLng> LinePointLatLng

        {
            get
            {

                return GeometryUtil.WebMercatorToWGS84(GetPoints().ToList());

            }
            set
            {
                // UIMessageBox.Show(value.ToString());

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
        public TailedSquadCombat(PointLatLng[] points,Guid guid, string name = "TailedSquadCombat")
            : base(points,guid, name)
        {
            this.points = points;
            this.geo_type = "RootTest";
            this.guid = guid;
            this.type = PlotTypes.TAILED_SQUAD_COMBAT;
            this.headHeightFactor = 0.18;
            this.headWidthFactor = 0.3;
            this.neckHeightFactor = 0.85;
            this.neckWidthFactor = 0.15;
            this.tailWidthFactor = 0.1;
            this.swallowTailFactor = 1;
            this.swallowTailPnt = PointLatLng.Empty;
            this.SetPoints(points);
        }

        public override void Generate()
        {
            var count = this.GetPointCount();
            if (count < 2)
            {
                return;
            }
            var pnts = this.GetPoints();
            var tailPnts = this.GetTailPoints(pnts);
            var headPnts = this.GetArrowHeadPoints(pnts, tailPnts[0], tailPnts[2]);
            var neckLeft = headPnts[0];
            var neckRight = headPnts[4];
            var bodyPnts = this.GetArrowBodyPoints(pnts, neckLeft, neckRight, this.tailWidthFactor);
            count = bodyPnts.Length;
            var leftPnts = new PointLatLng[] { tailPnts[0] }.Concat(bodyPnts.Take(count / 2)).ToList();
            leftPnts.Add(neckLeft);
            var rightPnts = new PointLatLng[] { tailPnts[2] }.Concat(bodyPnts.Skip(count / 2).Take(count)).ToList();
            rightPnts.Add(neckRight);

            leftPnts = PlotUtil.GetQBSplinePoints(leftPnts.ToArray());
            rightPnts = PlotUtil.GetQBSplinePoints(rightPnts.ToArray());
            rightPnts.Reverse();
            List<PointLatLng> tempPoints = leftPnts.Concat(headPnts).Concat(rightPnts).Concat(new PointLatLng[] { tailPnts[1], leftPnts[0] }).ToList();

            var temps = GeometryUtil.ConvertCoordinates(tempPoints, ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WebMercator, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);

            this.Points.Clear();
            this.Points.AddRange(temps);
        }

        private PointLatLng[] GetTailPoints(PointLatLng[] points)
        {
            var allLen = PlotUtil.GetBaseLength(points.ToList());
            var tailWidth = allLen * this.tailWidthFactor;
            var tailLeft = PlotUtil.GetThirdPoint(points[1], points[0], Constants.HALF_PI, tailWidth, false);
            var tailRight = PlotUtil.GetThirdPoint(points[1], points[0], Constants.HALF_PI, tailWidth, true);
            var len = tailWidth * this.swallowTailFactor;
            var swallowTailPnt = PlotUtil.GetThirdPoint(points[1], points[0], 0, len, true);
            return new PointLatLng[] { tailLeft, swallowTailPnt, tailRight };
        }
    }
    public class PointLatLngComparer : IEqualityComparer<PointLatLng>
    {
        public bool Equals(PointLatLng x, PointLatLng y)
        {
            return x.Lat == y.Lat && x.Lng == y.Lng;
        }
        public int GetHashCode(PointLatLng obj)
        {
            return string.Format("{0},{1}", obj.Lng, obj.Lat).GetHashCode();
        }
    }
}
