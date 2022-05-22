using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.Projections;
using GMap.NET.ObjectModel;
using GMap.NET.Internals;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Svg;
namespace GmapTest
{
    public partial class MainForm : UIForm
    {
        //记录标绘结果GMapOverlay       
        private GMapOverlay resultLay = new GMapOverlay("markerLay");
        //临时标绘GMapOverlay  
        private GMapOverlay tmpLay = new GMapOverlay("tmpLayLay");
        //标绘类型
        DrawTypes drawType=DrawTypes.NONE;
        //移动标志
        bool moveFlag = false;
        //编辑ployline的标志
        bool editPolylineFlag = false;
        bool editPolygonFlag = false;
        //标绘maker标志
        bool drawMakerFlag = false;
        //标绘line标志
        bool drawLineFlag = false;
        //标绘面标志
        bool drawpolygonFlag = false;
        //编辑标志
        bool drawEditFlag = false;
        //开始标绘标志
        bool drawStartFlag = false;
        //开始删除编制
        bool drawDelFlag= false;
    
        //记录当前标绘的点集
        List<PointLatLng> linePoints = new List<PointLatLng>();
        //当前选中的图标图像
        Bitmap currentImage;
        string currentImagePath;
        //记录当前选中的marker
        GMapMarkerExt.Markers.GMapMarkerImage currentMarker;
        //记录当前的折线
        GMapRouteExt gmapRouteExt;
        GmapPolgonExt Polygon;
        //记录当前的面
        GmapPolgonExt currentPolygon;
        //记录已标绘图元
        DataTable drawDataTabel = new DataTable();
        LineTypes lineType;
        PlotTypes polygonType;
        //线条粗细
        float lineStrokes = 5;
        //线条颜色
        Color lineColor = Color.Red;
        public MainForm()
        {
            InitializeComponent();
        }
       
        private void MainForm_Load(object sender, EventArgs e)
        {
           //初始化地图
            this.gMapControl1.MapProvider = AMapProvider.Instance;
            gMapControl1.DragButton = MouseButtons.Left;
            GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            this.gMapControl1.Position = new PointLatLng(42.0, 127.0);
            gMapControl1.Zoom = 9;
            gMapControl1.Overlays.Add(resultLay);
            gMapControl1.Overlays.Add(tmpLay);
            //初始化点maker
           // iniMarkerList();
            IniMarkerPanel();
            //初始化图元记录表格
            drawDataTabel.Columns.Add("序号");
            drawDataTabel.Columns.Add("ID");
            drawDataTabel.Columns.Add("名称");
            drawDataTabel.Columns.Add("类型");
            this.layDataGridView.DataSource = drawDataTabel;


        }

        private void menuNavBar_MenuItemClick(string itemText, int menuIndex, int pageIndex)
        {
            if(itemText=="标绘")
            {
                drawStartFlag = true;
                drawEditFlag = false;
                drawDelFlag = false;
                this.statusLabel.Text = "标绘模式";
               
            }
            if (itemText == "编辑")
            {
                drawStartFlag = false;
                drawEditFlag = true ;
                drawDelFlag = false;
                this.statusLabel.Text = "编辑模式";
            }
            if (itemText == "删除")
            {
                drawStartFlag = false;
                drawEditFlag = false;
                drawDelFlag = true;
                this.statusLabel.Text = "删除模式";
                if (this.propertyGrid1.SelectedObject == null)
                {
                    UIMessageBox.ShowError("请选择要删除的标号！");
                    return;

                }
                string objectID=null;
                object selcetObject = this.propertyGrid1.SelectedObject;
                switch (this.propertyGrid1.SelectedObject.ToString())
                {
                    case "GMapMarkerExt.Markers.GMapMarkerImage":
                        if (resultLay.Markers.Contains((GMapMarker)selcetObject))
                        {
                            objectID = ((GMapMarkerExt.Markers.GMapMarkerImage)selcetObject).ID.ToString();
                            resultLay.Markers.Remove((GMapMarker)selcetObject);
                        }
                        break;
                    case "GmapTest.Polyline":
                        if (resultLay.Routes.Contains((GMapRoute)selcetObject))
                        {
                            objectID = ((Polyline)selcetObject).ID.ToString();
                            resultLay.Routes.Remove((GMapRoute)selcetObject);
                        }
                        break;
                    default:
                        if (resultLay.Polygons.Contains((GMapPolygon)selcetObject))
                        {
                            objectID = ((GmapPolgonExt)selcetObject).ID.ToString();
                            resultLay.Polygons.Remove((GMapPolygon)selcetObject);
                        }
                        break;
                }
                if(objectID!=null)
                {
                    DataTable dt = drawDataTabel.Clone();
                    int k = 0;
                    DataRow dr;
                    for(int i=0;i< drawDataTabel.Rows.Count;i++)
                    {
                        if (drawDataTabel.Rows[i]["ID"].ToString() == objectID)
                        {
                           // drawDataTabel.Rows[i].Delete();
                            continue;
                        }
                        dr = drawDataTabel.Rows[i];
                        dr["序号"]=(k+1).ToString();
                        dt.Rows.Add(dr.ItemArray);
                        k++;
                        
                        
                    }
                    drawDataTabel.Clear();
                    drawDataTabel = dt;
                    this.layDataGridView.DataSource = drawDataTabel;
                }
                this.propertyGrid1.SelectedObject = null;


            }
            if (itemText == "清除")
            {
                drawStartFlag = false;
                drawEditFlag = false;
                drawDelFlag = false;
                resultLay.Clear();
                tmpLay.Clear();
                this.statusLabel.Text = "查看模式";
                drawDataTabel.Rows.Clear();
                linePoints.Clear();
            }
            //处理切换地图
            switch (itemText)
            {
                case "高德线划图":
                    this.gMapControl1.MapProvider = AMapProvider.Instance;
                    break;
               
                default:
                  
                    break;
             }
            this.mapProviderLabel.Text = this.gMapControl1.MapProvider.Name;
        }

       
        //}
        /// <summary>
        /// 设置线条的颜色及粗细
        /// </summary>
        /// <param name="linetype"></param>
        /// <returns></returns>
        //private Pen getLineColor(LineTypes linetype )
        //{
        //    Pen pen=new Pen(Color.Gray, lineStrokes); ;
        //    switch (linetype)
        //    {
        //        case LineTypes.CXXLL:
        //            pen = new Pen(Color.Brown, lineStrokes);
        //            break;
        //        case LineTypes.RXXLL:
        //            pen = new Pen(Color.Brown, lineStrokes);
        //            break;
        //        case LineTypes.TSW:
        //            pen = new Pen(Color.Red, lineStrokes);
        //            break;
        //        case LineTypes.TZL:
        //            pen = new Pen(Color.Red, lineStrokes);
        //            break;
        //        case LineTypes.BJXL:
        //            pen = new Pen(Color.Black, lineStrokes);
        //            break;
        //        case LineTypes.CSXL:
        //            pen = new Pen(Color.GreenYellow, lineStrokes);
        //            break;
        //        case LineTypes.GLD:
        //            pen = new Pen(Color.Green, lineStrokes);
        //            break;
        //        case LineTypes.NONE:
        //            pen = new Pen(Color.Red, lineStrokes);
        //            break;
        //    }
        //   // pen = new Pen(lineColor, lineStrokes);
        //    return pen;
        //}
        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           
            //处理标绘时
            if (drawStartFlag)
            {   Guid guid = Guid.NewGuid();
                switch (drawType)
                {
                    case DrawTypes.DOT:
                        PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                        GMapMarkerExt.Markers.GMapMarkerImage marker = new GMapMarkerExt.Markers.GMapMarkerImage(point, guid,currentImagePath);
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.ToolTipText = string.Format("{0},{1},{2}", point.Lat, point.Lng, marker.Name);
                        resultLay.Markers.Add(marker);
                        tmpLay.Clear();
                        drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(),guid.ToString(), marker.Name, "点");
                        drawType = DrawTypes.NONE;
                         this.propertyGrid1.SelectedObject = marker;

                        break;
                    case DrawTypes.POLYLINE:
                        switch (lineType)
                        {
                            case LineTypes.Polyline:
                                Polyline pl = new Polyline(linePoints.ToArray(), guid, "");
                                pl.IsHitTestVisible = true;
                                // pl.SetPoints(linePoints.ToArray());
                                // gmapRouteExt.Stroke = getLineColor(lineType);
                                resultLay.Routes.Add(pl);
                                this.propertyGrid1.SelectedObject = pl;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), pl.ID, pl.Name, "线");
                                break;
                            case LineTypes.Curve:
                                Curve curve = new Curve(linePoints.ToArray(), guid, "");
                                curve.IsHitTestVisible = true;
                                // pl.SetPoints(linePoints.ToArray());
                                // gmapRouteExt.Stroke = getLineColor(lineType);
                                resultLay.Routes.Add(curve);
                                this.propertyGrid1.SelectedObject = curve;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), curve.ID, curve.Name, "线");
                                break;
                            case LineTypes.Arc:
                                Arc arc = new Arc(linePoints.ToArray(), guid, "");
                                arc.IsHitTestVisible = true;
                                arc.IsVisible = true;
                                // pl.SetPoints(linePoints.ToArray());
                                // gmapRouteExt.Stroke = getLineColor(lineType);
                                resultLay.Routes.Add(arc);
                                this.propertyGrid1.SelectedObject = arc;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), arc.ID, arc.Name, "线");
                                break;
                            case LineTypes.FreehandLine:
                                FreehandLine freehandline = new FreehandLine(linePoints.ToArray(), guid, "");
                                freehandline.IsHitTestVisible = true;
                                // pl.SetPoints(linePoints.ToArray());
                                // gmapRouteExt.Stroke = getLineColor(lineType);
                                resultLay.Routes.Add(freehandline);
                                this.propertyGrid1.SelectedObject = freehandline;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), freehandline.ID, freehandline.Name, "线");
                                break;
                        }

                       
                        tmpLay.Clear();
                        linePoints.Clear();
                      
                        drawType = DrawTypes.NONE;
                        drawLineFlag = false;
                        break;
                    case DrawTypes.POLYGON:
                      
                        switch (polygonType)
                        {
                            case PlotTypes.CIRCLE:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                Circle cc = new Circle(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                cc.IsHitTestVisible = true;
                               // cc.Stroke = new Pen(Color.Red, 5);
                                resultLay.Polygons.Add(cc);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = cc;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), cc.Name, "面");
                                break;
                            case PlotTypes.ELLIPSE:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                Ellipse ep = new Ellipse(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                ep.IsHitTestVisible = true;
                                resultLay.Polygons.Add(ep);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = ep;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), ep.Name, "面");
                                break;
                            case PlotTypes.RECTANGLE:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                Rectangle rt = new Rectangle(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                rt.IsHitTestVisible = true;
                                resultLay.Polygons.Add(rt);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = rt;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), rt.Name, "面");
                                break;
                            case PlotTypes.ARC:

                                break;
                            case PlotTypes.ATTACK_ARROW:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);
                              
                                AttackArrow aa = new AttackArrow(linePoints.ToArray(),guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                aa.IsHitTestVisible = true;
                                resultLay.Polygons.Add(aa);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = aa;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), aa.Name, "面");
                                break;
                            case PlotTypes.CLOSED_CURVE:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                ClosedCurve ccc = new ClosedCurve(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                ccc.IsHitTestVisible = true;
                                resultLay.Polygons.Add(ccc);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = ccc;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), ccc.Name, "面");
                                break;
                         
                            case PlotTypes.DOUBLE_ARROW:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);
                              
                                DoubleArrow da = new DoubleArrow(linePoints.ToArray(),guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                da.IsHitTestVisible = true;
                                resultLay.Polygons.Add(da);
                               
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = da;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), da.Name, "面");
                                break;
                            case PlotTypes.FINE_ARROW:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                FineArrow fa = new FineArrow(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                fa.IsHitTestVisible = true;
                                resultLay.Polygons.Add(fa);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = fa;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), fa.Name, "面");
                                break;
                            case PlotTypes.ASSAULT_DIRECTION:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                AssaultDirection ad = new AssaultDirection(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                ad.IsHitTestVisible = true;
                                resultLay.Polygons.Add(ad);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = ad;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), ad.Name, "面");
                                break;
                            case PlotTypes.FREEHAND_LINE:

                                break;
                            case PlotTypes.FREEHAND_POLYGON:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                FreehandPolygon fp = new FreehandPolygon(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                fp.IsHitTestVisible = true;
                                resultLay.Polygons.Add(fp);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = fp;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), fp.Name, "面");
                                break;
                            
                            case PlotTypes.LUNE:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);
                               
                                Lune lu = new Lune(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                lu.IsHitTestVisible = true;
                                resultLay.Polygons.Add(lu);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = lu;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), lu.Name, "面");
                                break;
                           case PlotTypes.GATHERING_PLACE:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);
                                GatheringPlace gp = new GatheringPlace(linePoints.ToArray(), guid);


                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                gp.IsHitTestVisible = true;
                                resultLay.Polygons.Add(gp);
                                drawType = DrawTypes.NONE;
                                tmpLay.Clear();
                                this.propertyGrid1.SelectedObject = gp;
                                  drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), gp.Name, "面");
                                break;
                            case PlotTypes.SECTOR:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);
                                Sector sc = new Sector(linePoints.ToArray(), guid);
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                sc.IsHitTestVisible = true;
                                resultLay.Polygons.Add(sc);
                                drawType = DrawTypes.NONE;
                                tmpLay.Clear();
                                this.propertyGrid1.SelectedObject = sc;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), sc.Name, "面");
                                break;
                            case PlotTypes.SQUAD_COMBAT:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                SquadCombat scb = new SquadCombat(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                scb.IsHitTestVisible = true;
                                resultLay.Polygons.Add(scb);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = scb;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), scb.Name, "面");
                                break;
                            case PlotTypes.STRAIGHT_ARROW:

                                break;
                            case PlotTypes.TAILED_ATTACK_ARROW:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                TailedAttackArrow taa = new TailedAttackArrow(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                taa.IsHitTestVisible = true;
                                resultLay.Polygons.Add(taa);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = taa;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), taa.Name, "面");

                                break;
                            case PlotTypes.TAILED_SQUAD_COMBAT:
                                //去掉鼠标最后两次双击点
                                linePoints.RemoveAt(linePoints.Count - 1);

                                TailedSquadCombat tsc = new TailedSquadCombat(linePoints.ToArray(), guid);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                tsc.IsHitTestVisible = true;
                                resultLay.Polygons.Add(tsc);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = tsc;
                                drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), tsc.Name, "面");
                                break;
                            case PlotTypes.NONE:

                                break;
                        }
                        break;
                }
            }
            //处理编辑时
            if (drawEditFlag)
            {
                switch (drawType)
                {  
                    //移动点
                    case DrawTypes.DOT:
                      
                        if (moveFlag && currentMarker != null)
                        {
                            moveFlag = false;
                           
                            currentMarker = null;
                            drawType = DrawTypes.NONE;
                        }
                        break;
                    case DrawTypes.POLYLINE:
                        tmpLay.Clear();
                        switch (lineType)
                        {
                            case LineTypes.Polyline:
                                Polyline pl = new Polyline(linePoints.ToArray(), gmapRouteExt.ID, gmapRouteExt.Name);
                                pl.IsHitTestVisible = true;
                              
                                resultLay.Routes.Add(pl);
                                break;
                            case LineTypes.Curve:
                                Curve curve = new Curve(linePoints.ToArray(), gmapRouteExt.ID, gmapRouteExt.Name);
                                curve.IsHitTestVisible = true;

                                resultLay.Routes.Add(curve);
                                break;
                            case LineTypes.Arc:
                                Arc arc = new Arc(linePoints.ToArray(), gmapRouteExt.ID, gmapRouteExt.Name);
                                arc.IsHitTestVisible = true;

                                resultLay.Routes.Add(arc);
                                break;
                            case LineTypes.FreehandLine:
                                FreehandLine freehandline = new FreehandLine(linePoints.ToArray(), gmapRouteExt.ID, gmapRouteExt.Name);
                                freehandline.IsHitTestVisible = true;
                                resultLay.Routes.Add(freehandline);
                                break;
                        }
                        
                        editPolylineFlag = false;
                        currentMarker = null;
                        moveFlag = false;
                        editPolylineFlag = false;
                        linePoints.Clear();
                        drawType = DrawTypes.NONE;
                        break;
                    case DrawTypes.POLYGON:
                        switch (polygonType)
                        {
                            case PlotTypes.CIRCLE:
                               
                                Circle cc = new Circle(linePoints.ToArray(), currentPolygon.ID,currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                cc.IsHitTestVisible = true;
                                // cc.Stroke = new Pen(Color.Red, 5);
                                resultLay.Polygons.Add(cc);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = cc;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), cc.Name, "面");
                                break;
                            case PlotTypes.ELLIPSE:
                               

                                Ellipse ep = new Ellipse(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                ep.IsHitTestVisible = true;
                                resultLay.Polygons.Add(ep);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = ep;
                              //  drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), ep.Name, "面");
                                break;
                            case PlotTypes.RECTANGLE:
                              

                                Rectangle rt = new Rectangle(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                rt.IsHitTestVisible = true;
                                resultLay.Polygons.Add(rt);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = rt;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), rt.Name, "面");
                                break;
                            case PlotTypes.ARC:

                                break;
                            case PlotTypes.ATTACK_ARROW:
                              

                                AttackArrow aa = new AttackArrow(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                aa.IsHitTestVisible = true;
                                resultLay.Polygons.Add(aa);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = aa;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), aa.Name, "面");
                                break;
                            case PlotTypes.CLOSED_CURVE:
                               

                                ClosedCurve ccc = new ClosedCurve(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                ccc.IsHitTestVisible = true;
                                resultLay.Polygons.Add(ccc);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = ccc;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), ccc.Name, "面");
                                break;

                            case PlotTypes.DOUBLE_ARROW:
                               

                                DoubleArrow da = new DoubleArrow(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                da.IsHitTestVisible = true;
                                resultLay.Polygons.Add(da);

                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = da;
                             //   drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), da.Name, "面");
                                break;
                            case PlotTypes.FINE_ARROW:
                            

                                FineArrow fa = new FineArrow(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                fa.IsHitTestVisible = true;
                                resultLay.Polygons.Add(fa);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = fa;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), fa.Name, "面");
                                break;
                            case PlotTypes.ASSAULT_DIRECTION:
                               

                                AssaultDirection ad = new AssaultDirection(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                ad.IsHitTestVisible = true;
                                resultLay.Polygons.Add(ad);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = ad;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), ad.Name, "面");
                                break;
                            case PlotTypes.FREEHAND_LINE:

                                break;
                            case PlotTypes.FREEHAND_POLYGON:
                               

                                FreehandPolygon fp = new FreehandPolygon(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                fp.IsHitTestVisible = true;
                                resultLay.Polygons.Add(fp);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = fp;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), fp.Name, "面");
                                break;

                            case PlotTypes.LUNE:
                               
                                Lune lu = new Lune(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                lu.IsHitTestVisible = true;
                                resultLay.Polygons.Add(lu);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = lu;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), lu.Name, "面");
                                break;
                            case PlotTypes.GATHERING_PLACE:
                               
                                GatheringPlace gp = new GatheringPlace(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);


                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                gp.IsHitTestVisible = true;
                                resultLay.Polygons.Add(gp);
                                drawType = DrawTypes.NONE;
                                tmpLay.Clear();
                                this.propertyGrid1.SelectedObject = gp;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), gp.Name, "面");
                                break;
                            case PlotTypes.SECTOR:
                               
                                Sector sc = new Sector(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                sc.IsHitTestVisible = true;
                                resultLay.Polygons.Add(sc);
                                drawType = DrawTypes.NONE;
                                tmpLay.Clear();
                                this.propertyGrid1.SelectedObject = sc;
                             //   drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), sc.Name, "面");
                                break;
                            case PlotTypes.SQUAD_COMBAT:
                              

                                SquadCombat scb = new SquadCombat(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                scb.IsHitTestVisible = true;
                                resultLay.Polygons.Add(scb);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = scb;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), scb.Name, "面");
                                break;
                            case PlotTypes.STRAIGHT_ARROW:

                                break;
                            case PlotTypes.TAILED_ATTACK_ARROW:
                                

                                TailedAttackArrow taa = new TailedAttackArrow(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                taa.IsHitTestVisible = true;
                                resultLay.Polygons.Add(taa);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = taa;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), taa.Name, "面");

                                break;
                            case PlotTypes.TAILED_SQUAD_COMBAT:
                        

                                TailedSquadCombat tsc = new TailedSquadCombat(linePoints.ToArray(), currentPolygon.ID, currentPolygon.Name);
                                // aa.IsHitTestVisible = true;
                                linePoints.Clear();
                                tmpLay.Clear();
                                // drawArrowFlag = false;
                                tsc.IsHitTestVisible = true;
                                resultLay.Polygons.Add(tsc);
                                drawType = DrawTypes.NONE;
                                this.propertyGrid1.SelectedObject = tsc;
                               // drawDataTabel.Rows.Add((this.drawDataTabel.Rows.Count + 1).ToString(), guid.ToString(), tsc.Name, "面");
                                break;
                            case PlotTypes.NONE:

                                break;
                        }
                       
                        currentMarker = null;
                        moveFlag = false;
                        editPolygonFlag = false;
                        linePoints.Clear();
                        drawType = DrawTypes.NONE;

                        break;
                }
               
    

            }


        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            Guid guid = Guid.NewGuid();
            //处理标绘时
            if (drawStartFlag)
            {
                switch (drawType)
                {
                    case DrawTypes.DOT:
                        PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                        //第一次进来
                        if (!moveFlag)
                        {
                            currentMarker = new GMapMarkerExt.Markers.GMapMarkerImage(point,guid, currentImagePath, "标记");
                            currentMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                            currentMarker.ToolTipText = string.Format("{0},{1},{2}", point.Lat, point.Lng, currentMarker.Name);
                            tmpLay.Markers.Add(currentMarker);
                            moveFlag = true;
                        }
                        else
                        {
                            currentMarker.Position = point;

                        }
                       

                        break;
                    case DrawTypes.POLYLINE:
                        if (lineType!= LineTypes.FreehandLine)
                        {
                            if (linePoints.Count == 0)
                                return;
                            if (linePoints.Count > 2)
                            {
                                linePoints.RemoveAt(linePoints.Count - 1);
                            }

                        }
                        linePoints.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));
                        tmpLay.Routes.Clear();
                        switch (lineType)
                        {
                            case LineTypes.Polyline:
                               Polyline pl = new Polyline(linePoints.ToArray(),guid,"");
                                tmpLay.Routes.Add(pl);
                                break;
                            case LineTypes.Curve:
                                Curve cy = new Curve(linePoints.ToArray(), guid, "");
                                tmpLay.Routes.Add(cy);
                                break;
                            case LineTypes.Arc:
                                Arc arc = new Arc(linePoints.ToArray(), guid, "");
                                tmpLay.Routes.Add(arc);
                                break;
                            case LineTypes.FreehandLine:
                                FreehandLine freehandline = new FreehandLine(linePoints.ToArray(), guid, "");
                                tmpLay.Routes.Add(freehandline);
                                break;
                        }

                        break;
                    case DrawTypes.POLYGON:
                        if (linePoints.Count > 1)
                        {
                            linePoints.RemoveAt(linePoints.Count - 1);

                        }
                        if (linePoints.Count == 0)
                        {
                            return;

                        }
                     
                        linePoints.Add(gMapControl1.FromLocalToLatLng(e.X, e.Y));
                        switch (polygonType)
                        {
                            case PlotTypes.MARKER:

                                break;
                            case PlotTypes.POLYLINE:

                                break;
                            case PlotTypes.POLYGON:

                                break;
                            case PlotTypes.CIRCLE:
                                Circle ccc = new Circle(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(ccc);
                                break;
                            case PlotTypes.ELLIPSE:
                                Ellipse ep = new Ellipse(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(ep);
                                break;
                            case PlotTypes.RECTANGLE:
                                Rectangle rt = new Rectangle(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(rt);
                                break;
                            case PlotTypes.ARC:

                                break;
                            case PlotTypes.ATTACK_ARROW:

                                AttackArrow aa = new AttackArrow(linePoints.ToArray(), guid);
                               
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(aa);
                                break;
                            case PlotTypes.CLOSED_CURVE:
                                ClosedCurve cc = new ClosedCurve(linePoints.ToArray(), guid);

                                tmpLay.Clear();
                                tmpLay.Polygons.Add(cc);
                                break;
                            case PlotTypes.CURVE:
                                
                                
                                break;
                            case PlotTypes.DOUBLE_ARROW:
                                DoubleArrow da = new DoubleArrow(linePoints.ToArray(), guid);

                                tmpLay.Clear();
                                tmpLay.Polygons.Add(da);
                                break;
                            case PlotTypes.FINE_ARROW:
                                FineArrow fa = new FineArrow(linePoints.ToArray(), guid);

                                tmpLay.Clear();
                                tmpLay.Polygons.Add(fa);
                                break;
                            case PlotTypes.ASSAULT_DIRECTION:
                                AssaultDirection ad = new AssaultDirection(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(ad);
                                break;
                            case PlotTypes.FREEHAND_LINE:

                                break;
                            case PlotTypes.FREEHAND_POLYGON:
                                FreehandPolygon fp = new FreehandPolygon(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(fp);
                                break;
                            case PlotTypes.GATHERING_PLACE:
                                GatheringPlace gp= new GatheringPlace(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(gp);
                                break;
                            case PlotTypes.LUNE:
                                Lune lune = new Lune(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(lune);
                                break;
                            case PlotTypes.SECTOR:
                                Sector sector = new Sector(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(sector);
                                break;
                            case PlotTypes.SQUAD_COMBAT:
                                SquadCombat sc = new SquadCombat(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(sc);
                                break;
                            case PlotTypes.STRAIGHT_ARROW:
                               
                                break;
                            case PlotTypes.TAILED_ATTACK_ARROW:
                                TailedAttackArrow taa = new TailedAttackArrow(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(taa);
                                break;
                            case PlotTypes.TAILED_SQUAD_COMBAT:
                                TailedSquadCombat tsc = new TailedSquadCombat(linePoints.ToArray(), guid);
                                tmpLay.Clear();
                                tmpLay.Polygons.Add(tsc);
                                break;
                            case PlotTypes.NONE:

                                break;
                        }
                        break;
                }
            }
            //处理编辑时
            if (drawEditFlag)
            {
                switch (drawType)
                {
                    //点状目标移动
                    case DrawTypes.DOT:

                        if (moveFlag && currentMarker != null)
                        {
                            currentMarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                        }
                        break;
                   //线状目标移动
                    case DrawTypes.POLYLINE:
                        if (moveFlag && currentMarker != null)
                        {

                           // if(currentMarker)
                           
                            int i = linePoints.IndexOf(currentMarker.Position);
                            if (i < 0)
                                return;
                            currentMarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                            linePoints[i] = (currentMarker.Position);
                            tmpLay.Routes.Clear();
                            switch (lineType)
                            {
                                case LineTypes.Polyline:
                                    Polyline pl = new Polyline(linePoints.ToArray(), guid, "");
                                    tmpLay.Routes.Add(pl);
                                    break;
                                case LineTypes.Curve:
                                    Curve cy = new Curve(linePoints.ToArray(), guid, "");
                                    tmpLay.Routes.Add(cy);
                                    break;
                                case LineTypes.Arc:
                                    Arc arc = new Arc(linePoints.ToArray(), guid, "");
                                    tmpLay.Routes.Add(arc);
                                    break;
                                case LineTypes.FreehandLine:
                                    FreehandLine freehandline = new FreehandLine(linePoints.ToArray(), guid, "");
                                    tmpLay.Routes.Add(freehandline);
                                    break;
                            }

                        }
                        break;
                    case DrawTypes.POLYGON:
                        if (moveFlag && currentMarker != null)
                        {


                            int i = linePoints.IndexOf(currentMarker.Position);
                            if (i < 0)
                                return;
                            currentMarker.Position = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                            linePoints[i] = (currentMarker.Position);
                          //  MessageBox.Show(polygonType.ToString());
                            switch (polygonType)
                            {
                                
                                case PlotTypes.CIRCLE:
                                    Circle ccc = new Circle(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(ccc);
                                    break;
                                case PlotTypes.ELLIPSE:
                                    Ellipse ep = new Ellipse(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(ep);
                                    break;
                                  
                                case PlotTypes.RECTANGLE:
                                    Rectangle rt = new Rectangle(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(rt);
                                    break;
                                case PlotTypes.ARC:

                                    break;
                                case PlotTypes.ATTACK_ARROW:

                                    AttackArrow aa = new AttackArrow(linePoints.ToArray(), guid);

                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(aa);
                                    break;
                                case PlotTypes.CLOSED_CURVE:
                                    ClosedCurve cc = new ClosedCurve(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(cc);
                                    break;
                                case PlotTypes.CURVE:


                                    break;
                                case PlotTypes.DOUBLE_ARROW:
                                    DoubleArrow da = new DoubleArrow(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(da);
                                    break;
                                case PlotTypes.FINE_ARROW:
                                    FineArrow fa = new FineArrow(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(fa);
                                    break;
                                case PlotTypes.ASSAULT_DIRECTION:
                                    AssaultDirection ad = new AssaultDirection(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(ad);
                                    break;
                                case PlotTypes.FREEHAND_LINE:

                                    break;
                                case PlotTypes.FREEHAND_POLYGON:
                                    FreehandPolygon fp = new FreehandPolygon(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(fp);
                                    break;
                                case PlotTypes.GATHERING_PLACE:
                                    GatheringPlace gp = new GatheringPlace(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(gp);
                                    break;
                                case PlotTypes.LUNE:
                                    Lune lune = new Lune(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(lune);
                                    break;
                                case PlotTypes.SECTOR:
                                    Sector sector = new Sector(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(sector);
                                    break;
                                case PlotTypes.SQUAD_COMBAT:
                                    SquadCombat sc = new SquadCombat(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(sc);
                                    break;
                                case PlotTypes.STRAIGHT_ARROW:
                                    break;
                                case PlotTypes.TAILED_ATTACK_ARROW:
                                    TailedAttackArrow taa = new TailedAttackArrow(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(taa);
                                    break;
                                case PlotTypes.TAILED_SQUAD_COMBAT:
                                    TailedSquadCombat tsc = new TailedSquadCombat(linePoints.ToArray(), guid);
                                    tmpLay.Polygons.Clear();
                                    tmpLay.Polygons.Add(tsc);
                                    break;
                                case PlotTypes.NONE:
                                    break;
                            }
                           
                        }
                        break;
                }
               
            }


        }

        private void gMapControl1_OnMarkerDoubleClick(GMapMarker item, MouseEventArgs e)
        {

        }

        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            Guid guid = Guid.NewGuid();
            this.propertyGrid1.SelectedObject = item;
            //移动点标记
            if (e.Button == System.Windows.Forms.MouseButtons.Left &&drawEditFlag && drawType==DrawTypes.NONE)
            {
                currentMarker = (GMapMarkerExt.Markers.GMapMarkerImage)item;
                this.propertyGrid1.SelectedObject = currentMarker;
                moveFlag = true;
                drawType = DrawTypes.DOT;
            }
            //处理编辑线时的点
            if(e.Button == System.Windows.Forms.MouseButtons.Left && drawEditFlag && drawType == DrawTypes.POLYLINE)
            {
               // MessageBox.Show("11");
                currentMarker = (GMapMarkerExt.Markers.GMapMarkerImage)item;
                moveFlag = true;
            }
           
            //编辑线时添加点
            if (e.Button == System.Windows.Forms.MouseButtons.Right && drawEditFlag && drawType == DrawTypes.POLYLINE)
            {
                int i = linePoints.IndexOf(item.Position);
                List<PointLatLng> tmplinePoints = new List<PointLatLng>();
                for (int t = 0; t < linePoints.Count; t++)
                {
                    if (t == i)
                    {
                        tmplinePoints.Add(gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y));
                        tmplinePoints.Add(linePoints[t]);
                        //Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                        //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                        currentImagePath = Application.StartupPath + "\\image\\dot.svg";
                        //Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                        //GMapMarker marker = new GMarkerGoogle(point, bitmap);

                        GMapMarkerExt.Markers.GMapMarkerImage marker = new GMapMarkerExt.Markers.GMapMarkerImage(gMapControl1.FromLocalToLatLng(e.Location.X, e.Location.Y),guid, currentImagePath);
                        marker.Size = new Size(24, 24);
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

                //gmapRouteExt = new Polyline(linePoints.ToArray(),  gmapRouteExt.ID);
                //// gmapRouteExt.Stroke = getLineColor(lineType);
                //// markerLay.Routes.Add(route3);
                //tmpLay.Routes.Clear();
                //tmpLay.Routes.Add(gmapRouteExt);

            }
            //编辑线时删除点
            if (e.Button == System.Windows.Forms.MouseButtons.Middle && drawEditFlag && drawType == DrawTypes.POLYLINE)
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
                switch (lineType)
                {
                    case LineTypes.Polyline:
                        Polyline pl = new Polyline(linePoints.ToArray(), gmapRouteExt.ID);
                        pl.IsHitTestVisible = true;
                        resultLay.Routes.Add(pl);
                        break;
                    case LineTypes.Curve:
                        Curve curve = new Curve(linePoints.ToArray(), gmapRouteExt.ID);
                        curve.IsHitTestVisible = true;
                        resultLay.Routes.Add(curve);
                        break;
                    case LineTypes.Arc:
                        Arc arc = new Arc(linePoints.ToArray(), gmapRouteExt.ID);
                        arc.IsHitTestVisible = true;
                        resultLay.Routes.Add(arc);
                        break;
                    case LineTypes.FreehandLine:
                        FreehandLine freehandline = new FreehandLine(linePoints.ToArray(), gmapRouteExt.ID);
                        freehandline.IsHitTestVisible = true;
                        resultLay.Routes.Add(freehandline);
                        break;
                }
                //gmapRouteExt = new Polyline(linePoints.ToArray(), gmapRouteExt.ID);
                // gmapRouteExt.Stroke = getLineColor(lineType);
               
                tmpLay.Clear();
                currentMarker = null;
               
                editPolylineFlag = false;
                drawType = DrawTypes.NONE;

            }
            //处理编辑面时的点
            if (e.Button == System.Windows.Forms.MouseButtons.Left && drawEditFlag && drawType == DrawTypes.POLYGON)
            {
               
                currentMarker = (GMapMarkerExt.Markers.GMapMarkerImage)item;
                moveFlag = true;
            }
        }

   

        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {

            if (drawStartFlag && drawType==DrawTypes.POLYLINE)
            {
                //不是双击结束,则增加一个点
                if (e.Clicks == 1 && e.Button == MouseButtons.Left)
                {
                    if(lineType !=LineTypes.FreehandLine)
                    {
                        PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                        linePoints.Add(point);
                    }
                }
                else
                {
                    tmpLay.Clear();

                }


            }
            if (drawStartFlag && drawType == DrawTypes.POLYGON)
            {

                
                //不是双击结束,则增加一个点
                if (e.Clicks == 1&&e.Button==MouseButtons.Left)
                {

                    PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                    linePoints.Add(point);
                   
                }
                else
                {
                    tmpLay.Clear();

                }
            }
            this.propertyGrid1.SelectedObject = null;
        }

        private void gMapControl1_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {
            this.propertyGrid1.SelectedObject = item;
            if (e.Button == System.Windows.Forms.MouseButtons.Left && drawEditFlag == true)
            {
                if (!editPolylineFlag)
                {
                   // gmapRouteExt = (GMapRouteExt)item;
                    switch ((item.GetType().ToString().Split(".")[1]).ToUpper())
                    {
                        case "POLYLINE":
                            lineType = LineTypes.Polyline;
                            gmapRouteExt = (Polyline)item;
                            break;
                        case "CURVE":
                            lineType = LineTypes.Curve;
                            gmapRouteExt = (Curve)item;
                            break;
                        case "ARC":
                            gmapRouteExt = (Arc)item;
                            lineType = LineTypes.Arc;
                            break;
                           
                        case "FREEHANDLINE":
                            //gmapRouteExt = (FreehandLine)item;
                            //lineType = LineTypes.FreehandLine;
                            UIMessageBox.Show("自由曲线不支持编辑！");
                            return;
                           
                    }


                    this.propertyGrid1.SelectedObject = gmapRouteExt;
                    moveFlag = true;
                    drawType = DrawTypes.POLYLINE;
                    linePoints = gmapRouteExt.GetPoints().ToList() ;
                    //lineType = gmapRouteExt.getLineType();
                    resultLay.Routes.Remove(gmapRouteExt);
                    tmpLay.Routes.Add(gmapRouteExt);

                    foreach (PointLatLng i in linePoints)
                    {
                        Guid guid = Guid.NewGuid();
                        currentImagePath = Application.StartupPath + "\\image\\dot.svg";
                        //Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                        //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                        GMapMarkerExt.Markers.GMapMarkerImage marker = new GMapMarkerExt.Markers.GMapMarkerImage(i, guid, currentImagePath);
                        marker.Size = new Size(24, 24);
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.ToolTipText = string.Format("{0},{1},{2}", i.Lat, i.Lng, "单击左键移动点，点击右键添加点,中键删除点！");
                        // MessageBox.Show(marker.ToolTipText);
                        tmpLay.Markers.Add(marker);
                    }
                    editPolylineFlag = true;


                }
               
            }
        }

        private void uiDoubleUpDown1_ValueChanged(object sender, double value)
        {
            lineStrokes = (float)value;
        }

        private void uiColorPicker1_Click(object sender, EventArgs e)
        {
           
          //  MessageBox.Show(lineColor.ToString()) ;
        }

        private void uiColorPicker1_ValueChanged(object sender, Color value)
        {
            lineColor = uiColorPicker1.Value;
        }

     

        private void gMapControl1_OnPolygonClick(GMapPolygon item, MouseEventArgs e)
        {
            //  currentPolygon = (Polygon)item。;
            // item.t
           // MessageBox.Show(item.GetType().ToString());
            this.propertyGrid1.SelectedObject = item;
            if (e.Button == System.Windows.Forms.MouseButtons.Left && drawEditFlag == true)
            {
                if (!editPolygonFlag)
                {

                    currentPolygon = (GmapPolgonExt)item;
                  //  UIMessageBox.Show((item.GetType().ToString().Split(".")[1]).ToUpper());
                    switch ((item.GetType().ToString().Split(".")[1]).ToUpper())
                    {
                        case "SQUADCOMBAT":
                            polygonType = PlotTypes.SQUAD_COMBAT;
                            currentPolygon = (SquadCombat)item;
                            break;
                        case "TAILEDSQUADCOMBAT":
                            polygonType = PlotTypes.TAILED_SQUAD_COMBAT;
                            currentPolygon = (TailedSquadCombat)item;
                            break;
                        case "LUNE":
                            currentPolygon = (Lune)item;
                            polygonType = PlotTypes.LUNE;
                            break;
                        case "CLOSEDCURVE":
                            currentPolygon = (ClosedCurve)item;
                            polygonType = PlotTypes.CLOSED_CURVE;
                            break;
                        case "ATTACKARROW":
                            currentPolygon = (AttackArrow)item;
                            polygonType = PlotTypes.ATTACK_ARROW;
                            break;
                        case "TAILEDATTACK_ARROW":
                            currentPolygon = (TailedAttackArrow)item;
                            polygonType = PlotTypes.TAILED_ATTACK_ARROW;
                            break;
                        case "GATHERING_PLACE":
                            currentPolygon = (GatheringPlace)item;
                            polygonType = PlotTypes.GATHERING_PLACE;
                            break;
                        case "SECTOR":
                            currentPolygon = (Sector)item;
                            polygonType = PlotTypes.SECTOR;
                            break;
                        case "ASSAULTDIRECTION":
                            currentPolygon = (AssaultDirection)item;
                            polygonType = PlotTypes.ASSAULT_DIRECTION;
                            break;
                        case "FINEARROW":
                            currentPolygon = (FineArrow)item;
                            polygonType = PlotTypes.FINE_ARROW;
                            break;
                        case "FREEHANDPOLYGON":
                            currentPolygon = (FreehandPolygon)item;
                            polygonType = PlotTypes.FREEHAND_POLYGON;
                            break;
                        case "RECTANGLE":
                            currentPolygon = (Rectangle)item;
                            polygonType = PlotTypes.RECTANGLE;
                            break;
                        case "DOUBLEARROW":
                            currentPolygon = (DoubleArrow)item;
                            polygonType = PlotTypes.DOUBLE_ARROW;
                            break;
                        case "CIRCLE":
                            currentPolygon = (Circle)item;
                            polygonType = PlotTypes.CIRCLE;
                            break;
                        case "ELLIPSE":
                            currentPolygon = (Ellipse)item;
                            polygonType = PlotTypes.ELLIPSE;
                            break;
                        default:
                            polygonType = PlotTypes.NONE;
                            break;
                    }
                    //if (polygonType == PlotTypes.NONE)
                    //    return;
                   // UIMessageBox.Show(polygonType.ToString()) ;
                       moveFlag = true;
                    drawType = DrawTypes.POLYGON;
                   

                    linePoints = (currentPolygon.GetPoints().ToList());

                   // UIMessageBox.Show(linePoints.Count.ToString());
                    resultLay.Polygons.Remove(currentPolygon);
                    tmpLay.Polygons.Add(item);

                    foreach (PointLatLng i in linePoints)
                    {
                        
                        Guid guid = Guid.NewGuid();
                        currentImagePath = Application.StartupPath + "\\image\\dot.svg";
                        //Bitmap bitmap = Bitmap.FromFile(Application.StartupPath + "\\image\\dot.png") as Bitmap;
                        //GMapMarker marker = new GMarkerGoogle(point, bitmap);
                        GMapMarkerExt.Markers.GMapMarkerImage marker = new GMapMarkerExt.Markers.GMapMarkerImage(i, guid, currentImagePath);
                        marker.Size = new Size(24, 24);
                        marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        marker.ToolTipText = string.Format("{0},{1},{2}", i.Lat, i.Lng, "单击左键移动点，点击右键添加点,中键删除点！");
                        // MessageBox.Show(marker.ToolTipText);
                        tmpLay.Markers.Add(marker);
                    }
                    editPolygonFlag = true;


                }

            }
        }


        private void layDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void layDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (layDataGridView.SelectedRows != null)
            {
                string selectObjectID = layDataGridView.SelectedRows[0].Cells["ID"].Value.ToString();
                
                foreach (GMapMarkerExt.Markers.GMapMarkerImage i in resultLay.Markers)
                {
                    if (i.ID.ToString() == selectObjectID)
                    {
                        this.gMapControl1.Position = i.Position;
                        this.propertyGrid1.SelectedObject = i;
                    }

                }
                foreach (GMapRouteExt i in resultLay.Routes)
                {
                    //
                    if (i.ID.ToString() == selectObjectID)
                    {
                        //  MessageBox.Show(i.ID.ToString());
                        
                       // int centerPoint = i.LinePointLatLng.Count / 2 + 1;
                        this.gMapControl1.Position = PlotUtil.Mid(i.Points[0], i.Points[i.Points.Count - 1]);
                        this.propertyGrid1.SelectedObject = i;
                    }

                }
                foreach (GmapPolgonExt i in resultLay.Polygons)
                {
                    if (i.ID.ToString() == selectObjectID)
                    {
                       
                        this.gMapControl1.Position = GeometryUtil.GetCenterPoint((GMapPolygon)i); 
                        this.propertyGrid1.SelectedObject = i;
                    }
                }

            }
        }
      
        private void Btn_Click(object sender, EventArgs e)
        {
            UIImageButton btn = (UIImageButton)sender;
            switch (btn.Tag)
            {
                case "DOT":
                    currentImagePath = Application.StartupPath + "\\image\\dot\\" + btn.TagString + ".svg";
                    //  MessageBox.Show(currentImagePath);
                    //     markerListBox.SelectedItem.ImagePath;
                    //  drawMakerFlag = true;
                    moveFlag = false;
                    drawType = DrawTypes.DOT;
                    break;
                case "LINE":
                    moveFlag = false;
                    drawType = DrawTypes.POLYLINE;
                    drawLineFlag = true;
                    linePoints.Clear();
                    //string type = FileEx.FileInfo(Application.StartupPath + "\\image\\line\\" + lineListBox.SelectedItem.Description + ".svg").NameWithoutExt();
                    switch (btn.TagString)
                    {
                        case "折线":
                            lineType = LineTypes.Polyline;
                            break;
                        case "曲线":
                            lineType = LineTypes.Curve;
                            break;
                        case "圆弧":
                            lineType = LineTypes.Arc;
                            break;
                        case "自由线":
                            lineType = LineTypes.FreehandLine;
                            break;
                    }
                    break;
                case "POLYGON":
                    moveFlag = false;
                    drawType = DrawTypes.POLYGON;
                   
                    linePoints.Clear();
                    switch (btn.TagString)
                    {

                        case "战斗行动":
                            polygonType = PlotTypes.SQUAD_COMBAT;
                            break;
                        case "战斗行动-尾":
                            polygonType = PlotTypes.TAILED_SQUAD_COMBAT;
                            break;
                        case "弓形":
                            polygonType = PlotTypes.LUNE;
                            break;
                        case "曲线面":
                            polygonType = PlotTypes.CLOSED_CURVE;
                            break;
                        case "进攻":
                            polygonType = PlotTypes.ATTACK_ARROW;
                            break;
                        case "进攻-尾":
                            polygonType = PlotTypes.TAILED_ATTACK_ARROW;
                            break;
                        case "聚集地":
                            polygonType = PlotTypes.GATHERING_PLACE;
                            break;
                        case "扇形":
                            polygonType = PlotTypes.SECTOR;
                            break;
                        case "突击方向":
                            polygonType = PlotTypes.ASSAULT_DIRECTION;
                            break;
                        case "细直箭头":
                            polygonType = PlotTypes.FINE_ARROW;
                            break;
                        case "自由面":
                            polygonType = PlotTypes.FREEHAND_POLYGON;
                            break;
                        case "矩形":
                            polygonType = PlotTypes.RECTANGLE;
                            break;
                        case "钳击":
                            polygonType = PlotTypes.DOUBLE_ARROW;
                            break;
                        case "圆形":
                            polygonType = PlotTypes.CIRCLE;
                            break;
                        case "椭圆":
                            polygonType = PlotTypes.ELLIPSE;
                            break;
                        default:
                            polygonType = PlotTypes.NONE;
                            break;

                    }
                    break;
            }
          
         }
        private void IniMarkerPanel()
        {
            UIImageButton btn;
            string[] dotFiles = DirEx.GetFiles(Application.StartupPath + "\\image\\dot");
            string makerName;
            if (dotFiles.Length > 0)
            {
                foreach (string i in dotFiles)
                {
                    makerName = FileEx.FileInfo(i).NameWithoutExt();
                    SvgDocument svgDocument = SvgDocument.Open(i);
                    SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                    if (svgElement == null)
                        continue;
                    btn = new UIImageButton();
                    var bitmap1 = svgDocument.Draw(48, 48);
                    
                    svgElement.Stroke = new SvgColourServer(Color.DarkRed);
                    var bitmap2 = svgDocument.Draw(48, 48);
                    if (bitmap1 != null)
                    {
                        btn.Image = bitmap1;
                        btn.ImageHover = bitmap2;
                        btn.Size = new Size(48, 48);
                         btn.SetDPIScale();
                        btn.Tag = "DOT";
                         btn.TagString=makerName;
                        btn.BorderStyle = BorderStyle.FixedSingle;
                        uiToolTip1.SetToolTip(btn, makerName);
                        btn.Click += Btn_Click;
                        this.dotMarkerPanel.Add(btn);
                    }
                }
            }
            dotFiles = DirEx.GetFiles(Application.StartupPath + "\\image\\line");

            if (dotFiles.Length > 0)
            {
                foreach (string i in dotFiles)
                {
                    makerName = FileEx.FileInfo(i).NameWithoutExt();
                    SvgDocument svgDocument = SvgDocument.Open(i);
                    SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                    if (svgElement == null)
                        continue;
                    btn = new UIImageButton();
                    var bitmap1 = svgDocument.Draw(48, 48);
                   
                    svgElement.Stroke = new SvgColourServer(Color.DarkRed);
                    var bitmap2 = svgDocument.Draw(48, 48);
                    if (bitmap1 != null)
                    {
                        btn.Image = bitmap1;
                        btn.ImageHover = bitmap2;
                        btn.Size = new Size(48, 48);
                         btn.SetDPIScale();
                        btn.Tag = "LINE";
                        btn.TagString = makerName;
                        btn.BorderStyle = BorderStyle.FixedSingle;
                        uiToolTip1.SetToolTip(btn, makerName);
                        btn.Click += Btn_Click;
                        this.lineMarkerPanel.Add(btn);
                    }
                }
            }
            dotFiles = DirEx.GetFiles(Application.StartupPath + "\\image\\polygon");

            if (dotFiles.Length > 0)
            {

                foreach (string i in dotFiles)
                {
                    makerName = FileEx.FileInfo(i).NameWithoutExt();
                    SvgDocument svgDocument = SvgDocument.Open(i);
                    SvgElement svgElement = svgDocument.GetElementById("svgpropert");
                    if (svgElement == null)
                        continue;
                    btn = new UIImageButton();
                    var bitmap1 = svgDocument.Draw(45, 45);
                   
                    svgElement.Stroke = new SvgColourServer(Color.DarkRed);
                    var bitmap2 = svgDocument.Draw(45, 45);
                    if (bitmap1 != null)
                    {
                        btn.Image = bitmap1;
                        btn.ImageHover = bitmap2;
                        btn.Size = new Size(48, 48);
                       // btn.SizeMode = PictureBoxSizeMode.CenterImage;
                         btn.SetDPIScale();
                        btn.Tag = "POLYGON";
                        btn.TagString = makerName;
                        btn.BorderStyle = BorderStyle.FixedSingle;
                        uiToolTip1.SetToolTip(btn, makerName);
                        btn.Click += Btn_Click;
                        this.polygonMarkerPanel.Add(btn);
                    }
                }
            }

        }

        private void gMapControl1_OnMapZoomChanged()
        {
            zoomLabel.Text = "ZOOM:" + this.gMapControl1.Zoom.ToString();
            //int R = (int)((Radius) / Overlay.Control.MapProvider.Projection.GetGroundResolution((int)Overlay.Control.Zoom, Position.Lat)) * 2;

            int r = (int)((48 * this.gMapControl1.Zoom) / 9);
            foreach (GMapMarker marker in this.resultLay.Markers)
            {
                marker.Size = new Size(r, r);
            }
          
        }
    }
}
