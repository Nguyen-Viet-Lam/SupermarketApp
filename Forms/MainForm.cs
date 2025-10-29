using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Data;

namespace SupermarketApp.Forms
{
    public partial class MainForm : UIForm
    {
        private UIPanel pnlHeader;
        private UIPanel pnlSidebar;
        private UIPanel pnlContent;
        private UILabel lblTitle;
        private UILabel lblUser;
        private UIButton btnDashboard;
        private UIButton btnProducts;
        private UIButton btnInventory;
        private UIButton btnCustomers;
        private UIButton btnInvoices;
        private UIButton btnInvoiceHistory;
        private UIButton btnPurchaseOrders;
        private UIButton btnReports;
        private UIButton btnSuppliers;
        private UIButton btnEmployees;
        private UIButton btnSettings;
        private UIButton btnLogout;
        private string currentUser;

        public MainForm(string username)
        {
            currentUser = username;
            InitializeComponent();
            this.Load += async (s, e) => await ShowDashboardAsync();
            
            // Cập nhật tên user sau khi khởi tạo
            this.lblUser.Text = $"👤 Xin chào, {currentUser}";
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new Sunny.UI.UIPanel();
            this.lblUser = new Sunny.UI.UILabel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.pnlSidebar = new Sunny.UI.UIPanel();
            this.btnLogout = new Sunny.UI.UIButton();
            this.btnSettings = new Sunny.UI.UIButton();
            this.btnSuppliers = new Sunny.UI.UIButton();
            this.btnEmployees = new Sunny.UI.UIButton();
            this.btnReports = new Sunny.UI.UIButton();
            this.btnPurchaseOrders = new Sunny.UI.UIButton();
            this.btnInvoiceHistory = new Sunny.UI.UIButton();
            this.btnInvoices = new Sunny.UI.UIButton();
            this.btnCustomers = new Sunny.UI.UIButton();
            this.btnInventory = new Sunny.UI.UIButton();
            this.btnProducts = new Sunny.UI.UIButton();
            this.btnDashboard = new Sunny.UI.UIButton();
            this.pnlContent = new Sunny.UI.UIPanel();
            this.pnlHeader.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.pnlHeader.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.pnlHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlHeader.Location = new System.Drawing.Point(0, 35);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlHeader.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlHeader.Size = new System.Drawing.Size(1200, 70);
            this.pnlHeader.TabIndex = 0;
            this.pnlHeader.Text = null;
            this.pnlHeader.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblUser
            // 
            this.lblUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUser.ForeColor = System.Drawing.Color.Black;
            this.lblUser.Location = new System.Drawing.Point(1005, 20);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(175, 30);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "👤 Xin chào, User";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(473, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "QUẢN LÝ BÁN HÀNG SIÊU THỊ";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.White;
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Controls.Add(this.btnSettings);
            this.pnlSidebar.Controls.Add(this.btnSuppliers);
            this.pnlSidebar.Controls.Add(this.btnEmployees);
            this.pnlSidebar.Controls.Add(this.btnReports);
            this.pnlSidebar.Controls.Add(this.btnPurchaseOrders);
            this.pnlSidebar.Controls.Add(this.btnInvoiceHistory);
            this.pnlSidebar.Controls.Add(this.btnInvoices);
            this.pnlSidebar.Controls.Add(this.btnCustomers);
            this.pnlSidebar.Controls.Add(this.btnInventory);
            this.pnlSidebar.Controls.Add(this.btnProducts);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.pnlSidebar.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.pnlSidebar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlSidebar.Location = new System.Drawing.Point(0, 105);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSidebar.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlSidebar.Size = new System.Drawing.Size(252, 659);
            this.pnlSidebar.TabIndex = 1;
            this.pnlSidebar.Text = null;
            this.pnlSidebar.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnLogout.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnLogout.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnLogout.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnLogout.Location = new System.Drawing.Point(15, 594);
            this.btnLogout.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnLogout.Size = new System.Drawing.Size(220, 50);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "🚪 Đăng xuất";
            this.btnLogout.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSettings.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSettings.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSettings.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnSettings.Location = new System.Drawing.Point(15, 534);
            this.btnSettings.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnSettings.Size = new System.Drawing.Size(220, 50);
            this.btnSettings.TabIndex = 10;
            this.btnSettings.Text = "⚙️ Cài đặt";
            this.btnSettings.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // btnSuppliers
            // 
            this.btnSuppliers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSuppliers.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSuppliers.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSuppliers.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSuppliers.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnSuppliers.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnSuppliers.Location = new System.Drawing.Point(15, 494);
            this.btnSuppliers.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnSuppliers.Name = "btnSuppliers";
            this.btnSuppliers.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnSuppliers.Size = new System.Drawing.Size(220, 50);
            this.btnSuppliers.TabIndex = 6;
            this.btnSuppliers.Text = "🏭 Nhà cung cấp";
            this.btnSuppliers.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnSuppliers.Click += new System.EventHandler(this.BtnSuppliers_Click);
            // 
            // btnEmployees
            // 
            this.btnEmployees.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmployees.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnEmployees.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnEmployees.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnEmployees.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnEmployees.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnEmployees.Location = new System.Drawing.Point(15, 438);
            this.btnEmployees.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnEmployees.Name = "btnEmployees";
            this.btnEmployees.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnEmployees.Size = new System.Drawing.Size(220, 50);
            this.btnEmployees.TabIndex = 5;
            this.btnEmployees.Text = "👥 Nhân viên";
            this.btnEmployees.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnEmployees.Click += new System.EventHandler(this.BtnEmployees_Click);
            // 
            // btnReports
            // 
            this.btnReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReports.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnReports.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnReports.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnReports.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnReports.Location = new System.Drawing.Point(15, 382);
            this.btnReports.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnReports.Name = "btnReports";
            this.btnReports.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnReports.Size = new System.Drawing.Size(220, 50);
            this.btnReports.TabIndex = 4;
            this.btnReports.Text = "📊 Thống kê";
            this.btnReports.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnReports.Click += new System.EventHandler(this.BtnReports_Click);
            // 
            // btnPurchaseOrders
            // 
            this.btnPurchaseOrders.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPurchaseOrders.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnPurchaseOrders.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnPurchaseOrders.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnPurchaseOrders.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnPurchaseOrders.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnPurchaseOrders.Location = new System.Drawing.Point(15, 276);
            this.btnPurchaseOrders.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnPurchaseOrders.Name = "btnPurchaseOrders";
            this.btnPurchaseOrders.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnPurchaseOrders.Size = new System.Drawing.Size(220, 50);
            this.btnPurchaseOrders.TabIndex = 3;
            this.btnPurchaseOrders.Text = "📦 Nhập hàng";
            this.btnPurchaseOrders.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnPurchaseOrders.Click += new System.EventHandler(this.BtnPurchaseOrders_Click);
            // 
            // btnInvoiceHistory
            // 
            this.btnInvoiceHistory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInvoiceHistory.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInvoiceHistory.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInvoiceHistory.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnInvoiceHistory.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnInvoiceHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnInvoiceHistory.Location = new System.Drawing.Point(15, 326);
            this.btnInvoiceHistory.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnInvoiceHistory.Name = "btnInvoiceHistory";
            this.btnInvoiceHistory.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnInvoiceHistory.Size = new System.Drawing.Size(220, 50);
            this.btnInvoiceHistory.TabIndex = 3;
            this.btnInvoiceHistory.Text = "📜 Lịch sử HĐ";
            this.btnInvoiceHistory.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnInvoiceHistory.Click += new System.EventHandler(this.BtnInvoiceHistory_Click);
            // 
            // btnInvoices
            // 
            this.btnInvoices.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInvoices.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInvoices.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInvoices.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnInvoices.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnInvoices.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnInvoices.Location = new System.Drawing.Point(15, 220);
            this.btnInvoices.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnInvoices.Name = "btnInvoices";
            this.btnInvoices.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnInvoices.Size = new System.Drawing.Size(220, 50);
            this.btnInvoices.TabIndex = 3;
            this.btnInvoices.Text = "🛒 Bán hàng";
            this.btnInvoices.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnInvoices.Click += new System.EventHandler(this.BtnInvoices_Click);
            // 
            // btnCustomers
            // 
            this.btnCustomers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustomers.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCustomers.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCustomers.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnCustomers.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnCustomers.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnCustomers.Location = new System.Drawing.Point(15, 120);
            this.btnCustomers.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnCustomers.Size = new System.Drawing.Size(220, 50);
            this.btnCustomers.TabIndex = 2;
            this.btnCustomers.Text = "👥 Khách hàng";
            this.btnCustomers.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCustomers.Click += new System.EventHandler(this.BtnCustomers_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInventory.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInventory.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInventory.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnInventory.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnInventory.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.btnInventory.Location = new System.Drawing.Point(15, 176);
            this.btnInventory.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnInventory.Size = new System.Drawing.Size(220, 50);
            this.btnInventory.TabIndex = 2;
            this.btnInventory.Text = "📦 Kho hàng";
            this.btnInventory.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnInventory.Click += new System.EventHandler(this.BtnInventory_Click);
            // 
            // btnProducts
            // 
            this.btnProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProducts.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnProducts.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnProducts.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnProducts.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProducts.Location = new System.Drawing.Point(15, 64);
            this.btnProducts.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnProducts.Name = "btnProducts";
            this.btnProducts.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnProducts.Size = new System.Drawing.Size(220, 50);
            this.btnProducts.TabIndex = 1;
            this.btnProducts.Text = "📦 Sản phẩm";
            this.btnProducts.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnProducts.Click += new System.EventHandler(this.BtnProducts_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnDashboard.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnDashboard.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnDashboard.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(78)))), ((int)(((byte)(216)))));
            this.btnDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.Location = new System.Drawing.Point(15, 8);
            this.btnDashboard.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.btnDashboard.Size = new System.Drawing.Size(220, 50);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "🏠 Trang chủ";
            this.btnDashboard.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnDashboard.Click += new System.EventHandler(this.BtnDashboard_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlContent.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.pnlContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlContent.Location = new System.Drawing.Point(252, 105);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlContent.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlContent.Size = new System.Drawing.Size(948, 659);
            this.pnlContent.TabIndex = 2;
            this.pnlContent.Text = null;
            this.pnlContent.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1200, 764);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlHeader);
            this.Name = "MainForm";
            this.Text = "Quản lý bán hàng - SupermarketApp";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 1200, 700);
            this.pnlHeader.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private async void BtnDashboard_Click(object sender, EventArgs e)
        {
            await ShowDashboardAsync();
        }

        private void BtnProducts_Click(object sender, EventArgs e)
        {
            OpenProduct();
        }

        private void BtnInventory_Click(object sender, EventArgs e)
        {
            OpenInventory();
        }

        private void BtnCustomers_Click(object sender, EventArgs e)
        {
            OpenCustomer();
        }

        private void BtnInvoices_Click(object sender, EventArgs e)
        {
            OpenInvoice();
        }

        private void BtnInvoiceHistory_Click(object sender, EventArgs e)
        {
            OpenInvoiceHistory();
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            OpenReport();
        }

        private void BtnPurchaseOrders_Click(object sender, EventArgs e)
        {
            OpenPurchaseOrder();
        }

        private void BtnEmployees_Click(object sender, EventArgs e)
        {
            OpenEmployee();
        }

        private void BtnSuppliers_Click(object sender, EventArgs e)
        {
            OpenSupplier();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            OpenSettings();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageHelper.ShowAsk("Bạn có chắc muốn đăng xuất?"))
            {
                this.Close();
                Application.Exit();
            }
        }

        private async Task ShowDashboardAsync()
        {
            pnlContent.Controls.Clear();
            
            var dashPanel = new UIPanel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(243, 244, 246),
                FillColor2 = Color.FromArgb(243, 244, 246),
                RectSides = ToolStripStatusLabelBorderSides.None,
                AutoScroll = true,
                AutoScrollMinSize = new Size(950, 650)
            };

            // Welcome Header
            var pnlWelcome = new UIPanel
            {
                Location = new Point(20, 20),
                Size = new Size(900, 80),
                FillColor = Color.White,
                FillColor2 = Color.White,
                Radius = 10,
                RectColor = Color.FromArgb(59, 130, 246),
                RectSize = 2
            };

            var lblWelcome = new UILabel
            {
                Font = new Font("Microsoft Sans Serif", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246),
                Location = new Point(20, 15),
                Size = new Size(860, 50),
                Text = $"👋 Xin chào, {currentUser}! Chào mừng đến với hệ thống quản lý",
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlWelcome.Controls.Add(lblWelcome);
            dashPanel.Controls.Add(pnlWelcome);

            // Stats Cards Section
            var lblStats = new UILabel
            {
                Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(20, 120),
                Size = new Size(300, 30),
                Text = "📊 Thống kê tổng quan",
                TextAlign = ContentAlignment.MiddleLeft
            };
            dashPanel.Controls.Add(lblStats);

            try
            {
                using (var db = new SupermarketContext())
                {
                    var totalProducts = await Task.Run(() => db.SanPham.Count());
                    var totalCustomers = await Task.Run(() => db.KhachHang.Count());
                    var totalInvoices = await Task.Run(() => db.HoaDon.Count());
                    var totalRevenue = await Task.Run(() => db.HoaDon.Sum(x => (decimal?)x.TongTien) ?? 0);
                    
                    var today = DateTime.Today.ToUniversalTime();
                    var tomorrow = today.AddDays(1);
                    var todayInvoices = await Task.Run(() => db.HoaDon
                        .Where(x => x.NgayLap >= today && x.NgayLap < tomorrow)
                        .Count());
                    var todayRevenue = await Task.Run(() => db.HoaDon
                        .Where(x => x.NgayLap >= today && x.NgayLap < tomorrow)
                        .Sum(x => (decimal?)x.TongTien) ?? 0);
                    
                    var lowStockProducts = await Task.Run(() => db.SanPham
                        .Where(x => x.SoLuong < 10)
                        .Count());

                    // Row 1: General Stats
                    CreateStatCard(dashPanel, "📦 Tổng sản phẩm", totalProducts.ToString(), 
                        Color.FromArgb(59, 130, 246), 20, 165);
                    CreateStatCard(dashPanel, "👥 Tổng khách hàng", totalCustomers.ToString(), 
                        Color.FromArgb(16, 185, 129), 260, 165);
                    CreateStatCard(dashPanel, "🧾 Tổng hóa đơn", totalInvoices.ToString(), 
                        Color.FromArgb(139, 92, 246), 500, 165);
                    CreateStatCard(dashPanel, "💰 Tổng doanh thu", $"{totalRevenue:N0}", 
                        Color.FromArgb(239, 68, 68), 740, 165);

                    // Row 2: Today Stats
                    var lblToday = new UILabel
                    {
                        Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(31, 41, 55),
                        Location = new Point(20, 315),
                        Size = new Size(300, 30),
                        Text = "📅 Thống kê hôm nay",
                        TextAlign = ContentAlignment.MiddleLeft
                    };
                    dashPanel.Controls.Add(lblToday);

                    CreateStatCard(dashPanel, "🛒 Hóa đơn hôm nay", todayInvoices.ToString(), 
                        Color.FromArgb(245, 158, 11), 20, 360);
                    CreateStatCard(dashPanel, "💵 Doanh thu hôm nay", $"{todayRevenue:N0}", 
                        Color.FromArgb(16, 185, 129), 260, 360);
                    CreateStatCard(dashPanel, "⚠️ Sản phẩm sắp hết", lowStockProducts.ToString(), 
                        Color.FromArgb(239, 68, 68), 500, 360);
                    
                    var avgRevenue = totalInvoices > 0 ? totalRevenue / totalInvoices : 0;
                    CreateStatCard(dashPanel, "📈 TB/Hóa đơn", $"{avgRevenue:N0}", 
                        Color.FromArgb(59, 130, 246), 740, 360);

                    // Quick Actions Panel
                    CreateQuickActionsPanel(dashPanel, 20, 510);
                }
            }
            catch (Exception ex)
            {
                MessageHelper.ShowError("Lỗi tải dữ liệu: " + ex.Message);
            }

            pnlContent.Controls.Add(dashPanel);
        }

        private void CreateQuickActionsPanel(UIPanel parent, int x, int y)
        {
            var pnlActions = new UIPanel
            {
                Location = new Point(x, y),
                Size = new Size(960, 100),
                FillColor = Color.White,
                FillColor2 = Color.White,
                Radius = 10,
                RectColor = Color.FromArgb(203, 213, 225),
                RectSize = 1
            };

            var lblTitle = new UILabel
            {
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(20, 15),
                Size = new Size(200, 25),
                Text = "⚡ Thao tác nhanh",
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlActions.Controls.Add(lblTitle);

            var btnQuickSell = new UIButton
            {
                Text = "🛒 Bán hàng",
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold),
                Location = new Point(20, 50),
                Size = new Size(140, 38),
                Radius = 5,
                FillColor = Color.FromArgb(59, 130, 246),
                FillColor2 = Color.FromArgb(37, 99, 235),
                FillHoverColor = Color.FromArgb(37, 99, 235),
                Cursor = Cursors.Hand
            };
            btnQuickSell.Click += (s, e) => OpenInvoice();
            pnlActions.Controls.Add(btnQuickSell);

            var btnQuickProduct = new UIButton
            {
                Text = "📦 Sản phẩm",
                Font = new Font("Microsoft Sans Serif", 11F),
                Location = new Point(175, 50),
                Size = new Size(140, 38),
                Radius = 5,
                FillColor = Color.FromArgb(16, 185, 129),
                FillColor2 = Color.FromArgb(5, 150, 105),
                FillHoverColor = Color.FromArgb(5, 150, 105),
                Cursor = Cursors.Hand
            };
            btnQuickProduct.Click += (s, e) => OpenProduct();
            pnlActions.Controls.Add(btnQuickProduct);

            var btnQuickCustomer = new UIButton
            {
                Text = "👥 Khách hàng",
                Font = new Font("Microsoft Sans Serif", 11F),
                Location = new Point(330, 50),
                Size = new Size(140, 38),
                Radius = 5,
                FillColor = Color.FromArgb(139, 92, 246),
                FillColor2 = Color.FromArgb(124, 58, 237),
                FillHoverColor = Color.FromArgb(124, 58, 237),
                Cursor = Cursors.Hand
            };
            btnQuickCustomer.Click += (s, e) => OpenCustomer();
            pnlActions.Controls.Add(btnQuickCustomer);

            var btnQuickReport = new UIButton
            {
                Text = "📊 Báo cáo",
                Font = new Font("Microsoft Sans Serif", 11F),
                Location = new Point(485, 50),
                Size = new Size(140, 38),
                Radius = 5,
                FillColor = Color.FromArgb(245, 158, 11),
                FillColor2 = Color.FromArgb(217, 119, 6),
                FillHoverColor = Color.FromArgb(217, 119, 6),
                Cursor = Cursors.Hand
            };
            btnQuickReport.Click += (s, e) => OpenReport();
            pnlActions.Controls.Add(btnQuickReport);

            parent.Controls.Add(pnlActions);
        }

        private void CreateStatCard(UIPanel parent, string title, string value, Color color, int x, int y)
        {
            var card = new UIPanel
            {
                FillColor = Color.White,
                FillColor2 = Color.White,
                Location = new Point(x, y),
                Size = new Size(230, 130),
                Radius = 10,
                RectColor = color,
                RectSize = 2
            };

            var lblTitle = new UILabel
            {
                Font = new Font("Microsoft Sans Serif", 11F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(15, 15),
                Size = new Size(200, 25),
                Text = title,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblValue = new UILabel
            {
                Font = new Font("Microsoft Sans Serif", 20F, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(15, 50),
                Size = new Size(200, 60),
                Text = value,
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            parent.Controls.Add(card);
        }

        private void OpenProduct()
        {
            pnlContent.Controls.Clear();
            var f = new ProductForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenCustomer()
        {
            pnlContent.Controls.Clear();
            var f = new CustomerForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenInvoice()
        {
            pnlContent.Controls.Clear();
            var f = new InvoiceForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenReport()
        {
            pnlContent.Controls.Clear();
            var f = new ReportForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenEmployee()
        {
            pnlContent.Controls.Clear();
            var f = new EmployeeForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenSupplier()
        {
            pnlContent.Controls.Clear();
            var f = new SupplierForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenPurchaseOrder()
        {
            pnlContent.Controls.Clear();
            var f = new PurchaseOrderForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenInvoiceHistory()
        {
            pnlContent.Controls.Clear();
            var f = new InvoiceHistoryForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenInventory()
        {
            pnlContent.Controls.Clear();
            var f = new InventoryForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }

        private void OpenSettings()
        {
            pnlContent.Controls.Clear();
            var f = new SettingsForm(currentUser) { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            pnlContent.Controls.Add(f);
            f.Show();
        }
    }
}
