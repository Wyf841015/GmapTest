using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.Projections;
using GMap.NET.ObjectModel;
using GMap.NET.Internals;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Sunny.UI;
namespace GmapTest
{
    public partial class Form1 : Form
    {
        private GMapOverlay markerLay = new GMapOverlay("markerLay"); //放置marker的图层
        private GMapOverlay tmpLay = new GMapOverlay("tmpLayLay"); //放置marker的图层
        private GMapMarkerExt.Markers.GMapMarkerImage currentMarker;
        
        private GMapRoute currentLine;
        bool moveFlag = false;
        bool drawMakerFlag = false;
        bool drawLineFlag = false;
        bool drawArrowFlag = false;
        bool editLineFlag = false;
        bool drawLineStartFlag = false;
        GMapRoute route1 = new GMapRoute("route1");
        Bitmap currentImage;
        //设置route的颜色和粗细
    
        List<PointLatLng> linePoints = new List<PointLatLng>();
        public event EventHandler add;
        private void adda(object sender, EventArgs e)
        {
            MessageBox.Show("！！！");
        }
        
        public Form1()
        {
            InitializeComponent();
            markerLay.Markers.CollectionChanged += Markers_CollectionChanged;
        }

        private void Markers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.gMapControl1.MapProvider = AMapProvider.Instance;
            gMapControl1.DragButton = MouseButtons.Left;
           GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            //  gMapControl1.Position = new PointLatLng(LATITUDE, LONGITUDE);
            this.gMapControl1.Position = new PointLatLng(42.0, 127.0);
            gMapControl1.Zoom = 9;
           //  Bitmap bitmap = Bitmap.FromFile("F:\\Projects\\GMapDemo\\GMapDemo\\Image\\A.png") as Bitmap;
            gMapControl1.Overlays.Add(markerLay);
            gMapControl1.Overlays.Add(tmpLay);
            //初始化点目标列表
            string[] dotFiles=DirEx.GetFiles(Application.StartupPath+"\\image\\dot");
            if (dotFiles.Length>0)
            {
                foreach (string i in dotFiles)
                {
                    uiImageListBox1.AddImage(i, "XX");
                }
            }
            Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
            //  GeometryUtil.DrawEllipse(gMapControl1, markerLay, new PointLatLng(42.0, 127.0), 50, "");
            //PointLatLng[] p = new [] { new PointLatLng(42.0, 127.0), new PointLatLng(42.5, 127.5), new PointLatLng(43.0, 127.5) };
            //// Marker md = new Marker(p, bitmap);
            //// 
            //Arc a = new Arc(p);
            //markerLay.Routes.Add(a);
            //SquadCombat aa = new SquadCombat(p);
            //GatheringPlace aa1 = new GatheringPlace(p, "1234");
            //aa.Stroke = new Pen(Color.Red, 5);
            //aa.Fill = new SolidBrush(Color.FromArgb(200, Color.Green));
            //aa.IsHitTestVisible = true;
            //// aa.IsMouseOver = true;
    
            //markerLay.Polygons.Add(aa);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.gMapControl1.Dispose();
        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
           
          

        }
        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveFlag == true && currentMarker != null)
            {
                //获取坐标
                // Point pt = e.GetPosition(MainMap);
                //转换成地理坐标
                // PointLatLng point = MainMap.FromLocalToLatLng((int)pt.X, (int)pt.Y);
                currentMarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
            }
           
            if (drawLineFlag)
            {
                if (route1.Points.Count > 2)
                {
                    route1.Points.RemoveAt(route1.Points.Count - 1);
                }
                if (drawLineStartFlag)
                {
                    route1.Points.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));
                }
                tmpLay.Routes.Add(route1);
            }
            if (drawArrowFlag)
            {
                if (linePoints.Count > 1)
                {
                    linePoints.RemoveAt(linePoints.Count - 1);

                }
                if (linePoints.Count == 0)
                {
                    return;

                }
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                linePoints.Add(point);
                // linePoints = GeometryUtil.WebMercatorToWGS84(linePoints);

                PointLatLng[] points = linePoints.ToArray();
                //  MessageBox.Show(points.Length.ToString());
                AttackArrow aa = new AttackArrow(points, "");
                // aa.IsHitTestVisible = true;
                tmpLay.Clear();
                tmpLay.Polygons.Add(aa);
            }
            this.uiLabel1.Text = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat.ToString() + gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng.ToString(); ;
        }
        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if ( drawMakerFlag == true)
            {
                //objects.Markers.Clear();
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                //GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green);
                // Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\camera.png") as Bitmap;
                //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                GMapMarkerExt.Markers.GMapMarkerImage marker = new GMapMarkerExt.Markers.GMapMarkerImage(point, currentImage,"标记");
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                marker.ToolTipText = string.Format("{0},{1},{2}", point.Lat, point.Lng,marker.Name);
               
                markerLay.Markers.Add(marker);
                drawMakerFlag = false;
            }
            if (drawLineFlag)
            {

                route1.Points.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));
                linePoints.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));
                drawLineStartFlag = true;


            }
            if (drawArrowFlag)
            {

               

                //不是双击结束
                if (e.Clicks == 1)
                {
                   
                    PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                    linePoints.Add(point);
                   // this.uiLabel2.Text = linePoints.Count.ToString();
                  //  PointLatLng[] points = linePoints.ToArray();
                    //  MessageBox.Show(points.Length.ToString());

                  //  AttackArrow aa = new AttackArrow(points, "");
                    // aa.IsHitTestVisible = true;
                    //lineLay.Clear();
                   // lineLay.Polygons.Add(aa);
                }
                else
                {
                    tmpLay.Clear();

                }

            }

        }
        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            
            if (e.Button == System.Windows.Forms.MouseButtons.Left&& moveFlag == true)
            {
                currentMarker =(GMapMarkerExt.Markers.GMapMarkerImage) item;
                //moveFlag = true;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left && editLineFlag == true)
            {
                currentMarker = (GMapMarkerExt.Markers.GMapMarkerImage)item;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right && editLineFlag == true)
            {
                int i = linePoints.IndexOf(item.Position);
                List<PointLatLng> tmplinePoints = new List<PointLatLng>();
                for (int t=0;t<linePoints.Count;t++)
                {
                    if(t==i)
                    {
                        tmplinePoints.Add(gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y));
                        tmplinePoints.Add(linePoints[t]);
                         Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                            //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                            GMapMarker marker = new GMapMarkerExt.Markers.GMapMarkerImage(gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y), bitmap);
                            marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            marker.ToolTipText = string.Format("{0},{1},{2}", gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y).Lat, gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y).Lng, "\\n单击左键移动点，点击右键添加点,中键删除点！");
                        // MessageBox.Show(marker.ToolTipText);
                        tmpLay.Markers.Add(marker);
                       
                    }
                    else
                    {
                        tmplinePoints.Add(linePoints[t]);
                    }
                }
                linePoints.Clear();
                linePoints = tmplinePoints;
                GMapRoute route3 = new GMapRoute(linePoints, "");
                route3.Stroke = new Pen(Color.Red, 5);
                // markerLay.Routes.Add(route3);
                tmpLay.Routes.Clear();

                tmpLay.Routes.Add(route3);

            }
            if (e.Button == System.Windows.Forms.MouseButtons.Middle && editLineFlag == true)
            {
                int i = linePoints.IndexOf(item.Position);
                List<PointLatLng> tmplinePoints = new List<PointLatLng>();
                for (int t = 0; t < linePoints.Count; t++)
                {
                    if (t == i)
                    {
                        //tmplinePoints.Add(gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y));
                      //  tmplinePoints.Add(linePoints[t]);
                    }
                    else
                    {
                        tmplinePoints.Add(linePoints[t]);
                    }
                }
                linePoints.Clear();
                linePoints = tmplinePoints;
                GMapRoute route3 = new GMapRoute(linePoints, "");
                route3.Stroke = new Pen(Color.Red, 5);
                // markerLay.Routes.Add(route3);
                tmpLay.Clear();
                currentMarker = null;
                tmpLay.Routes.Add(route3);
              //  linePoints = markerLay.Routes[0].Points;
                // MessageBox.Show(markerLay.Routes.Count.ToString());
             

                foreach (PointLatLng k in linePoints)
                {
                    Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                    //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                    GMapMarker marker = new GMapMarkerExt.Markers.GMapMarkerImage(k, bitmap);
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    marker.ToolTipText = string.Format("{0},{1},{2}", k.Lat, k.Lng, "单击左键移动点，点击右键添加点, 中键删除点！");
                    // MessageBox.Show(marker.ToolTipText);
                    tmpLay.Markers.Add(marker);
                }

            }


        }

        private void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            
        }


       
        private void gMapControl1_DoubleClick(object sender, EventArgs e)
        {
            if (moveFlag == true && currentMarker != null)
            {
                //获取坐标
                // Point pt = e.GetPosition(MainMap);
                //转换成地理坐标
                // PointLatLng point = MainMap.FromLocalToLatLng((int)pt.X, (int)pt.Y);
                // currentMarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                moveFlag = false;
                currentMarker = null;
                //drawMakerFlag = true;
            }
            if (drawLineFlag&& route1.Points!=null)
            {
                drawLineFlag = false;
                GMapRoute route2 = new GMapRoute(linePoints,"111");
                route2.Stroke = new Pen(Color.Red, 5);
                route2.IsHitTestVisible = true;
                markerLay.Routes.Add(route2);
                
                // this.gMapControl1.UpdateRouteLocalPosition(route1);
                route1.Points.Clear();
                linePoints.Clear();
                drawLineStartFlag = false;

            }
            if(editLineFlag)
            {
                tmpLay.Routes.Clear();
                GMapRoute route3 = new GMapRoute(linePoints, "");
                route3.Stroke = new Pen(Color.Red, 5);
                // markerLay.Routes.Add(route3);
                tmpLay.Routes.Add(route3);
                linePoints = tmpLay.Routes[0].Points;
                editLineFlag = false;
               
                currentMarker = null;
                tmpLay.Markers.Clear();
              //  markerLay.Routes.Add(route3);
             
                // MessageBox.Show(markerLay.Routes.Count.ToString());
                editLineFlag = true;

                foreach (PointLatLng i in linePoints)
                {
                    Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                    //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                    GMapMarker marker = new GMapMarkerExt.Markers.GMapMarkerImage(i, bitmap);
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    marker.ToolTipText = string.Format("{0},{1},{2}", i.Lat, i.Lng, "单击左键移动点，点击右键添加点, 中键删除点！");
                    // MessageBox.Show(marker.ToolTipText);
                    tmpLay.Markers.Add(marker);
                }
            }
            if (drawArrowFlag)
            {
                //去掉鼠标最后两次双击点
                linePoints.RemoveAt(linePoints.Count - 1);
               // linePoints.RemoveAt(linePoints.Count - 2);
                PointLatLng[] points = linePoints.ToArray();
              //    MessageBox.Show(points.Length.ToString());
                AttackArrow aa = new AttackArrow(points, "进攻箭头1");
                // aa.IsHitTestVisible = true;
                linePoints.Clear();
                tmpLay.Clear();
                drawArrowFlag = false;
                aa.IsHitTestVisible = true;
                markerLay.Polygons.Add(aa);
              
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            drawMakerFlag = true;
            drawLineFlag = false;
            moveFlag = false;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            drawMakerFlag = false;
            moveFlag = true;
            drawLineFlag = false;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            drawLineFlag = true;

            route1.Stroke = new Pen(Color.Red, 2); ;
            drawMakerFlag = false;
            moveFlag = false;
        }

        

        private void gMapControl1_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {

            MessageBox.Show(item.Name);
        }

        private void gMapControl1_OnRouteEnter(GMapRoute item)
        {
            
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (markerLay.Routes.Count > 0)
            {
                linePoints = markerLay.Routes[0].Points;
                // MessageBox.Show(markerLay.Routes.Count.ToString());
                editLineFlag = true;
                GMapRoute route3 = new GMapRoute(linePoints, "");
                route3.Stroke = new Pen(Color.Red, 5);
                tmpLay.Routes.Add(route3);
                markerLay.Routes.Clear();
                foreach (PointLatLng i in linePoints)
                {
                    Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                    //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                    GMapMarker marker = new GMapMarkerExt.Markers.GMapMarkerImage(i, bitmap);
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    marker.ToolTipText = string.Format("{0},{1},{2}", i.Lat, i.Lng,"单击左键移动点，点击右键添加点,中键删除点！");
                    // MessageBox.Show(marker.ToolTipText);
                    tmpLay.Markers.Add(marker);
                }
            }
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            editLineFlag = false;
            tmpLay.Markers.Clear();
            GMapRoute route3 = new GMapRoute(linePoints, "");
            route3.Stroke = new Pen(Color.Red, 5);
            markerLay.Routes.Add(route3);
            editLineFlag = false;
            tmpLay.Routes.Clear();
            currentMarker = null;
            tmpLay.Markers.Clear();
            //  markerLay.Routes.Add(route3);
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        private void uiImageListBox1_ItemClick(object sender, EventArgs e)
        {
           currentImage = Bitmap.FromFile(uiImageListBox1.SelectedItem.ImagePath) as Bitmap;
          
        }

        private void gMapControl1_OnPolygonEnter(GMapPolygon item)
        {

            //AttackArrow aa = (AttackArrow)item;
            MessageBox.Show(item.GetType().ToString());

            //foreach (PointLatLng k in aa.GetPoints())
            //{
            //    //MessageBox.Show(GeometryUtil.WebMercatorToWGS84(k));
            //    linePoints.Add(k);
            //}
            //linePoints = GeometryUtil.WebMercatorToWGS84(linePoints);
            ////  linePoints = List < PointLatLng >( aa.GetPoints().ToArray()) ;

            //// MessageBox.Show(markerLay.Routes.Count.ToString());
            ////editLineFlag = true;
            ////// GMapRoute route3 = new GMapRoute(linePoints, "");
            //// route3.Stroke = new Pen(Color.Red, 5);
            //// lineLay.Routes.Add(route3);
            ////   markerLay.Routes.Clear();
            //foreach (PointLatLng i in linePoints)
            //{
            //    Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
            //    //GMapMarker marker = new GMarkerGoogle(point, bitmap);
            //    GMapMarker marker = new GMapMarkerExt.Markers.GMapMarkerImage(i, bitmap);
            //    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            //    marker.ToolTipText = string.Format("{0},{1},{2}", i.Lat, i.Lng, "单击左键移动点，点击右键添加点,中键删除点！");
            //    // MessageBox.Show(marker.ToolTipText);
            //    tmpLay.Markers.Add(marker);
            //}
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            if (markerLay.Polygons.Count > 0)
            {
                GatheringPlace aa =(GatheringPlace) markerLay.Polygons[0];
                foreach(PointLatLng k in aa.GetPoints())
                {
                    //MessageBox.Show(GeometryUtil.WebMercatorToWGS84(k));
                    linePoints.Add(k);
                }
                linePoints = GeometryUtil.WebMercatorToWGS84(linePoints);
              //  linePoints = List < PointLatLng >( aa.GetPoints().ToArray()) ;

                // MessageBox.Show(markerLay.Routes.Count.ToString());
                //editLineFlag = true;
                //// GMapRoute route3 = new GMapRoute(linePoints, "");
                // route3.Stroke = new Pen(Color.Red, 5);
                // lineLay.Routes.Add(route3);
                //   markerLay.Routes.Clear();
                foreach (PointLatLng i in linePoints)
                {
                    Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                    //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                    GMapMarker marker = new GMapMarkerExt.Markers.GMapMarkerImage(i, bitmap);
                    marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    marker.ToolTipText = string.Format("{0},{1},{2}", i.Lat, i.Lng, "单击左键移动点，点击右键添加点,中键删除点！");
                    // MessageBox.Show(marker.ToolTipText);
                    tmpLay.Markers.Add(marker);
                }
            }
        }

        private void gMapControl1_OnPolygonClick(GMapPolygon item, MouseEventArgs e)
        {
            MessageBox.Show(item.Name);
        }

        private void gMapControl1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void uiButton7_Click(object sender, EventArgs e)
        {
            drawArrowFlag = true;
            linePoints.Clear();
        }

        private void uiButton8_Click(object sender, EventArgs e)
        {
            markerLay.Clear();
            tmpLay.Clear();
            linePoints.Clear();
        }

        private void uiButton9_Click(object sender, EventArgs e)
        {
           // currentImage.Size=
        }

        private void gMapControl1_OnMarkerDoubleClick(GMapMarker item, MouseEventArgs e)
        {
            GMapMarkerExt.Markers.GMapMarkerImage maker = (GMapMarkerExt.Markers.GMapMarkerImage)item;
            MessageBox.Show(maker.Name);
            MessageBox.Show(maker.Size.ToString());
          //  MessageBox.Show(maker.)
        }
    }
}
