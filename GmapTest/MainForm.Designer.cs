namespace GmapTest
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("标绘");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("编辑");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("删除");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("清除");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("高德线划图");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("底图", new System.Windows.Forms.TreeNode[] {
            treeNode5});
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuNavBar = new Sunny.UI.UINavBar();
            this.mianSplitContainer1 = new Sunny.UI.UISplitContainer();
            this.uiSplitContainer1 = new Sunny.UI.UISplitContainer();
            this.uiTabControl2 = new Sunny.UI.UITabControl();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.dotMarkerPanel = new Sunny.UI.UIFlowLayoutPanel();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.lineMarkerPanel = new Sunny.UI.UIFlowLayoutPanel();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.polygonMarkerPanel = new Sunny.UI.UIFlowLayoutPanel();
            this.zoomLabel = new Sunny.UI.UILabel();
            this.mapProviderLabel = new Sunny.UI.UILabel();
            this.statusLabel = new Sunny.UI.UILabel();
            this.uiNavBar1 = new Sunny.UI.UINavBar();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.uiTabControl1 = new Sunny.UI.UITabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.linePanel = new Sunny.UI.UIPanel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.uiColorPicker1 = new Sunny.UI.UIColorPicker();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiDoubleUpDown1 = new Sunny.UI.UIDoubleUpDown();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.layDataGridView = new Sunny.UI.UIDataGridView();
            this.uiToolTip1 = new Sunny.UI.UIToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.mianSplitContainer1)).BeginInit();
            this.mianSplitContainer1.Panel1.SuspendLayout();
            this.mianSplitContainer1.Panel2.SuspendLayout();
            this.mianSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).BeginInit();
            this.uiSplitContainer1.Panel1.SuspendLayout();
            this.uiSplitContainer1.Panel2.SuspendLayout();
            this.uiSplitContainer1.SuspendLayout();
            this.uiTabControl2.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.uiTabControl1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.linePanel.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuNavBar
            // 
            this.menuNavBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuNavBar.DropMenuFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.menuNavBar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.menuNavBar.Location = new System.Drawing.Point(0, 35);
            this.menuNavBar.Name = "menuNavBar";
            treeNode1.Name = "节点0";
            treeNode1.Text = "标绘";
            treeNode2.Name = "节点1";
            treeNode2.Text = "编辑";
            treeNode3.Name = "节点2";
            treeNode3.Text = "删除";
            treeNode4.Name = "节点0";
            treeNode4.Text = "清除";
            treeNode5.Name = "节点1";
            treeNode5.Text = "高德线划图";
            treeNode6.Name = "节点3";
            treeNode6.Text = "底图";
            this.menuNavBar.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode6});
            this.menuNavBar.Size = new System.Drawing.Size(994, 49);
            this.menuNavBar.TabIndex = 0;
            this.menuNavBar.Text = "uiNavBar1";
            this.menuNavBar.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.menuNavBar.MenuItemClick += new Sunny.UI.UINavBar.OnMenuItemClick(this.menuNavBar_MenuItemClick);
            // 
            // mianSplitContainer1
            // 
            this.mianSplitContainer1.CollapsePanel = Sunny.UI.UISplitContainer.UICollapsePanel.Panel2;
            this.mianSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.mianSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mianSplitContainer1.Location = new System.Drawing.Point(0, 84);
            this.mianSplitContainer1.MinimumSize = new System.Drawing.Size(20, 20);
            this.mianSplitContainer1.Name = "mianSplitContainer1";
            // 
            // mianSplitContainer1.Panel1
            // 
            this.mianSplitContainer1.Panel1.Controls.Add(this.uiSplitContainer1);
            // 
            // mianSplitContainer1.Panel2
            // 
            this.mianSplitContainer1.Panel2.Controls.Add(this.uiTabControl1);
            this.mianSplitContainer1.Size = new System.Drawing.Size(994, 530);
            this.mianSplitContainer1.SplitterDistance = 803;
            this.mianSplitContainer1.SplitterWidth = 11;
            this.mianSplitContainer1.TabIndex = 1;
            this.mianSplitContainer1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiSplitContainer1
            // 
            this.uiSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.uiSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.uiSplitContainer1.MinimumSize = new System.Drawing.Size(20, 20);
            this.uiSplitContainer1.Name = "uiSplitContainer1";
            // 
            // uiSplitContainer1.Panel1
            // 
            this.uiSplitContainer1.Panel1.Controls.Add(this.uiTabControl2);
            // 
            // uiSplitContainer1.Panel2
            // 
            this.uiSplitContainer1.Panel2.Controls.Add(this.zoomLabel);
            this.uiSplitContainer1.Panel2.Controls.Add(this.mapProviderLabel);
            this.uiSplitContainer1.Panel2.Controls.Add(this.statusLabel);
            this.uiSplitContainer1.Panel2.Controls.Add(this.uiNavBar1);
            this.uiSplitContainer1.Panel2.Controls.Add(this.gMapControl1);
            this.uiSplitContainer1.Size = new System.Drawing.Size(803, 530);
            this.uiSplitContainer1.SplitterDistance = 170;
            this.uiSplitContainer1.SplitterWidth = 11;
            this.uiSplitContainer1.TabIndex = 0;
            this.uiSplitContainer1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiTabControl2
            // 
            this.uiTabControl2.Controls.Add(this.tabPage7);
            this.uiTabControl2.Controls.Add(this.tabPage8);
            this.uiTabControl2.Controls.Add(this.tabPage9);
            this.uiTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl2.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl2.ItemSize = new System.Drawing.Size(55, 40);
            this.uiTabControl2.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl2.MainPage = "";
            this.uiTabControl2.Name = "uiTabControl2";
            this.uiTabControl2.SelectedIndex = 0;
            this.uiTabControl2.Size = new System.Drawing.Size(170, 530);
            this.uiTabControl2.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl2.TabIndex = 5;
            this.uiTabControl2.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.dotMarkerPanel);
            this.tabPage7.Location = new System.Drawing.Point(0, 40);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(170, 490);
            this.tabPage7.TabIndex = 0;
            this.tabPage7.Text = "点";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // dotMarkerPanel
            // 
            this.dotMarkerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dotMarkerPanel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dotMarkerPanel.Location = new System.Drawing.Point(0, 0);
            this.dotMarkerPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dotMarkerPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.dotMarkerPanel.Name = "dotMarkerPanel";
            this.dotMarkerPanel.Padding = new System.Windows.Forms.Padding(2);
            this.dotMarkerPanel.ShowText = false;
            this.dotMarkerPanel.Size = new System.Drawing.Size(170, 490);
            this.dotMarkerPanel.TabIndex = 1;
            this.dotMarkerPanel.Text = "uiFlowLayoutPanel2";
            this.dotMarkerPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.dotMarkerPanel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.lineMarkerPanel);
            this.tabPage8.Location = new System.Drawing.Point(0, 40);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Size = new System.Drawing.Size(200, 60);
            this.tabPage8.TabIndex = 1;
            this.tabPage8.Text = "线";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // lineMarkerPanel
            // 
            this.lineMarkerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lineMarkerPanel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lineMarkerPanel.Location = new System.Drawing.Point(0, 0);
            this.lineMarkerPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lineMarkerPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.lineMarkerPanel.Name = "lineMarkerPanel";
            this.lineMarkerPanel.Padding = new System.Windows.Forms.Padding(2);
            this.lineMarkerPanel.ShowText = false;
            this.lineMarkerPanel.Size = new System.Drawing.Size(200, 60);
            this.lineMarkerPanel.TabIndex = 2;
            this.lineMarkerPanel.Text = "uiFlowLayoutPanel2";
            this.lineMarkerPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.lineMarkerPanel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.polygonMarkerPanel);
            this.tabPage9.Location = new System.Drawing.Point(0, 40);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Size = new System.Drawing.Size(200, 60);
            this.tabPage9.TabIndex = 2;
            this.tabPage9.Text = "面";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // polygonMarkerPanel
            // 
            this.polygonMarkerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.polygonMarkerPanel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.polygonMarkerPanel.Location = new System.Drawing.Point(0, 0);
            this.polygonMarkerPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.polygonMarkerPanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.polygonMarkerPanel.Name = "polygonMarkerPanel";
            this.polygonMarkerPanel.Padding = new System.Windows.Forms.Padding(2);
            this.polygonMarkerPanel.ShowText = false;
            this.polygonMarkerPanel.Size = new System.Drawing.Size(200, 60);
            this.polygonMarkerPanel.TabIndex = 2;
            this.polygonMarkerPanel.Text = "uiFlowLayoutPanel2";
            this.polygonMarkerPanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.polygonMarkerPanel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // zoomLabel
            // 
            this.zoomLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.zoomLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.zoomLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.zoomLabel.ForeColor = System.Drawing.Color.White;
            this.zoomLabel.Location = new System.Drawing.Point(255, 507);
            this.zoomLabel.Name = "zoomLabel";
            this.zoomLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.zoomLabel.Size = new System.Drawing.Size(137, 23);
            this.zoomLabel.Style = Sunny.UI.UIStyle.Custom;
            this.zoomLabel.TabIndex = 5;
            this.zoomLabel.Text = "ZOOM:";
            this.zoomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.zoomLabel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // mapProviderLabel
            // 
            this.mapProviderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mapProviderLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.mapProviderLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mapProviderLabel.ForeColor = System.Drawing.Color.White;
            this.mapProviderLabel.Location = new System.Drawing.Point(-3, 507);
            this.mapProviderLabel.Name = "mapProviderLabel";
            this.mapProviderLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mapProviderLabel.Size = new System.Drawing.Size(252, 23);
            this.mapProviderLabel.Style = Sunny.UI.UIStyle.Custom;
            this.mapProviderLabel.TabIndex = 4;
            this.mapProviderLabel.Text = "高德线划图";
            this.mapProviderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.mapProviderLabel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.statusLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(505, 505);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(107, 23);
            this.statusLabel.Style = Sunny.UI.UIStyle.Custom;
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "查看模式";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusLabel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiNavBar1
            // 
            this.uiNavBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uiNavBar1.DropMenuFont = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiNavBar1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiNavBar1.Location = new System.Drawing.Point(0, 504);
            this.uiNavBar1.Name = "uiNavBar1";
            this.uiNavBar1.Size = new System.Drawing.Size(612, 26);
            this.uiNavBar1.TabIndex = 2;
            this.uiNavBar1.Text = "uiNavBar1";
            this.uiNavBar1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = true;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(0, 0);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 21;
            this.gMapControl1.MinZoom = 1;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(612, 502);
            this.gMapControl1.TabIndex = 1;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl1_OnMarkerClick);
            this.gMapControl1.OnMarkerDoubleClick += new GMap.NET.WindowsForms.MarkerDoubleClick(this.gMapControl1_OnMarkerDoubleClick);
            this.gMapControl1.OnPolygonClick += new GMap.NET.WindowsForms.PolygonClick(this.gMapControl1_OnPolygonClick);
            this.gMapControl1.OnRouteClick += new GMap.NET.WindowsForms.RouteClick(this.gMapControl1_OnRouteClick);
            this.gMapControl1.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gMapControl1_OnMapZoomChanged);
            this.gMapControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDoubleClick);
            this.gMapControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseDown);
            this.gMapControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gMapControl1_MouseMove);
            // 
            // uiTabControl1
            // 
            this.uiTabControl1.Controls.Add(this.tabPage4);
            this.uiTabControl1.Controls.Add(this.tabPage5);
            this.uiTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.ItemSize = new System.Drawing.Size(60, 40);
            this.uiTabControl1.Location = new System.Drawing.Point(0, 0);
            this.uiTabControl1.MainPage = "";
            this.uiTabControl1.Name = "uiTabControl1";
            this.uiTabControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.uiTabControl1.SelectedIndex = 0;
            this.uiTabControl1.Size = new System.Drawing.Size(180, 530);
            this.uiTabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl1.TabIndex = 0;
            this.uiTabControl1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.linePanel);
            this.tabPage4.Location = new System.Drawing.Point(0, 40);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(180, 490);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "属性";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // linePanel
            // 
            this.linePanel.Controls.Add(this.propertyGrid1);
            this.linePanel.Controls.Add(this.uiButton1);
            this.linePanel.Controls.Add(this.uiColorPicker1);
            this.linePanel.Controls.Add(this.uiLabel2);
            this.linePanel.Controls.Add(this.uiDoubleUpDown1);
            this.linePanel.Controls.Add(this.uiLabel1);
            this.linePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linePanel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linePanel.Location = new System.Drawing.Point(0, 0);
            this.linePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.linePanel.MinimumSize = new System.Drawing.Size(1, 1);
            this.linePanel.Name = "linePanel";
            this.linePanel.Size = new System.Drawing.Size(180, 490);
            this.linePanel.TabIndex = 1;
            this.linePanel.Text = null;
            this.linePanel.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.linePanel.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid1.Size = new System.Drawing.Size(180, 490);
            this.propertyGrid1.TabIndex = 4;
            // 
            // uiButton1
            // 
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Location = new System.Drawing.Point(19, 149);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(150, 29);
            this.uiButton1.TabIndex = 4;
            this.uiButton1.Text = "应用";
            this.uiButton1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiColorPicker1
            // 
            this.uiColorPicker1.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.uiColorPicker1.FillColor = System.Drawing.Color.White;
            this.uiColorPicker1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiColorPicker1.Location = new System.Drawing.Point(19, 112);
            this.uiColorPicker1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiColorPicker1.MinimumSize = new System.Drawing.Size(63, 0);
            this.uiColorPicker1.Name = "uiColorPicker1";
            this.uiColorPicker1.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.uiColorPicker1.Size = new System.Drawing.Size(150, 29);
            this.uiColorPicker1.TabIndex = 3;
            this.uiColorPicker1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiColorPicker1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiColorPicker1.ValueChanged += new Sunny.UI.UIColorPicker.OnColorChanged(this.uiColorPicker1_ValueChanged);
            this.uiColorPicker1.Click += new System.EventHandler(this.uiColorPicker1_Click);
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(23, 84);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(100, 23);
            this.uiLabel2.TabIndex = 2;
            this.uiLabel2.Text = "颜色";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiDoubleUpDown1
            // 
            this.uiDoubleUpDown1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDoubleUpDown1.Location = new System.Drawing.Point(19, 42);
            this.uiDoubleUpDown1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiDoubleUpDown1.Minimum = 1D;
            this.uiDoubleUpDown1.MinimumSize = new System.Drawing.Size(100, 0);
            this.uiDoubleUpDown1.Name = "uiDoubleUpDown1";
            this.uiDoubleUpDown1.ShowText = false;
            this.uiDoubleUpDown1.Size = new System.Drawing.Size(150, 37);
            this.uiDoubleUpDown1.Step = 0.5D;
            this.uiDoubleUpDown1.TabIndex = 1;
            this.uiDoubleUpDown1.Text = "uiDoubleUpDown1";
            this.uiDoubleUpDown1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiDoubleUpDown1.Value = 5D;
            this.uiDoubleUpDown1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiDoubleUpDown1.ValueChanged += new Sunny.UI.UIDoubleUpDown.OnValueChanged(this.uiDoubleUpDown1_ValueChanged);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(14, 14);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(100, 23);
            this.uiLabel1.TabIndex = 0;
            this.uiLabel1.Text = "线条粗细";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.layDataGridView);
            this.tabPage5.Location = new System.Drawing.Point(0, 40);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(200, 60);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "图层";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // layDataGridView
            // 
            this.layDataGridView.AllowUserToAddRows = false;
            this.layDataGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.layDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.layDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.layDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.layDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.layDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.layDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.layDataGridView.ColumnHeadersHeight = 30;
            this.layDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.layDataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.layDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layDataGridView.EnableHeadersVisualStyles = false;
            this.layDataGridView.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.layDataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            this.layDataGridView.Location = new System.Drawing.Point(0, 0);
            this.layDataGridView.MultiSelect = false;
            this.layDataGridView.Name = "layDataGridView";
            this.layDataGridView.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.layDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.layDataGridView.RowHeadersWidth = 10;
            this.layDataGridView.RowHeight = 27;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.layDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.layDataGridView.RowTemplate.Height = 27;
            this.layDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.layDataGridView.SelectedIndex = -1;
            this.layDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.layDataGridView.ShowRect = false;
            this.layDataGridView.Size = new System.Drawing.Size(200, 60);
            this.layDataGridView.StripeOddColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.layDataGridView.TabIndex = 0;
            this.layDataGridView.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.layDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.layDataGridView_CellClick);
            // 
            // uiToolTip1
            // 
            this.uiToolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.uiToolTip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.uiToolTip1.OwnerDraw = true;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(994, 614);
            this.Controls.Add(this.mianSplitContainer1);
            this.Controls.Add(this.menuNavBar);
            this.Name = "MainForm";
            this.Text = "GmapTest";
            this.TextAlignment = System.Drawing.StringAlignment.Center;
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 800, 450);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mianSplitContainer1.Panel1.ResumeLayout(false);
            this.mianSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mianSplitContainer1)).EndInit();
            this.mianSplitContainer1.ResumeLayout(false);
            this.uiSplitContainer1.Panel1.ResumeLayout(false);
            this.uiSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitContainer1)).EndInit();
            this.uiSplitContainer1.ResumeLayout(false);
            this.uiTabControl2.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage8.ResumeLayout(false);
            this.tabPage9.ResumeLayout(false);
            this.uiTabControl1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.linePanel.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UINavBar menuNavBar;
        private Sunny.UI.UISplitContainer mianSplitContainer1;
        private Sunny.UI.UISplitContainer uiSplitContainer1;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private Sunny.UI.UITabControl uiTabControl1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private Sunny.UI.UIDataGridView layDataGridView;
        private Sunny.UI.UINavBar uiNavBar1;
        private Sunny.UI.UILabel statusLabel;
        private Sunny.UI.UIPanel linePanel;
        private Sunny.UI.UIButton uiButton1;
        private Sunny.UI.UIColorPicker uiColorPicker1;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIDoubleUpDown uiDoubleUpDown1;
        private Sunny.UI.UILabel uiLabel1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private Sunny.UI.UILabel mapProviderLabel;
        private Sunny.UI.UIToolTip uiToolTip1;
        private Sunny.UI.UITabControl uiTabControl2;
        private System.Windows.Forms.TabPage tabPage7;
        private Sunny.UI.UIFlowLayoutPanel dotMarkerPanel;
        private System.Windows.Forms.TabPage tabPage8;
        private Sunny.UI.UIFlowLayoutPanel lineMarkerPanel;
        private System.Windows.Forms.TabPage tabPage9;
        private Sunny.UI.UIFlowLayoutPanel polygonMarkerPanel;
        private Sunny.UI.UILabel zoomLabel;
    }
}