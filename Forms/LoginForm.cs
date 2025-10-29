using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Utils;
using SupermarketApp.Services;

namespace SupermarketApp.Forms
{
    public partial class LoginForm : UIForm
    {
        private UILabel lblTitle;
        private UILabel lblUsername;
        private UILabel lblPassword;
        private UITextBox txtUsername;
        private UITextBox txtPassword;
        private UIButton btnLogin;
        private UIButton btnRegister;
        private UIPanel pnlCenter;
        private readonly AuthService auth = new AuthService();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pnlCenter = new Sunny.UI.UIPanel();
            this.btnRegister = new Sunny.UI.UIButton();
            this.btnLogin = new Sunny.UI.UIButton();
            this.txtPassword = new Sunny.UI.UITextBox();
            this.lblPassword = new Sunny.UI.UILabel();
            this.txtUsername = new Sunny.UI.UITextBox();
            this.lblUsername = new Sunny.UI.UILabel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.pnlCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.White;
            this.pnlCenter.Controls.Add(this.btnRegister);
            this.pnlCenter.Controls.Add(this.btnLogin);
            this.pnlCenter.Controls.Add(this.txtPassword);
            this.pnlCenter.Controls.Add(this.lblPassword);
            this.pnlCenter.Controls.Add(this.txtUsername);
            this.pnlCenter.Controls.Add(this.lblUsername);
            this.pnlCenter.Controls.Add(this.lblTitle);
            this.pnlCenter.FillColor = System.Drawing.Color.White;
            this.pnlCenter.FillColor2 = System.Drawing.Color.White;
            this.pnlCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.pnlCenter.Location = new System.Drawing.Point(180, 80);
            this.pnlCenter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCenter.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Padding = new System.Windows.Forms.Padding(30);
            this.pnlCenter.Radius = 15;
            this.pnlCenter.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.pnlCenter.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlCenter.Size = new System.Drawing.Size(480, 420);
            this.pnlCenter.TabIndex = 0;
            this.pnlCenter.Text = null;
            this.pnlCenter.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRegister
            // 
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnRegister.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnRegister.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnRegister.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnRegister.Location = new System.Drawing.Point(245, 310);
            this.btnRegister.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(200, 45);
            this.btnRegister.TabIndex = 6;
            this.btnRegister.Text = "ĐĂNG KÝ";
            this.btnRegister.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnLogin.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnLogin.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnLogin.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(24)))), ((int)(((byte)(93)))));
            this.btnLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Location = new System.Drawing.Point(35, 310);
            this.btnLogin.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(200, 45);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "ĐĂNG NHẬP";
            this.btnLogin.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtPassword.Location = new System.Drawing.Point(35, 235);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.ShowText = false;
            this.txtPassword.Size = new System.Drawing.Size(410, 40);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPassword.Watermark = "Nhập mật khẩu...";
            // 
            // lblPassword
            // 
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPassword.Location = new System.Drawing.Point(35, 200);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(150, 25);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Mật khẩu";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            this.txtUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtUsername.Location = new System.Drawing.Point(35, 145);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUsername.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Padding = new System.Windows.Forms.Padding(5);
            this.txtUsername.ShowText = false;
            this.txtUsername.Size = new System.Drawing.Size(410, 40);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtUsername.Watermark = "Nhập tên đăng nhập...";
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblUsername.Location = new System.Drawing.Point(35, 110);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(150, 25);
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Text = "Tên đăng nhập";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTitle.Location = new System.Drawing.Point(13, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(467, 60);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ĐĂNG NHẬP HỆ THỐNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(850, 600);
            this.Controls.Add(this.pnlCenter);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.Text = "Quản lý bán hàng - SupermarketApp";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.ZoomScaleRect = new System.Drawing.Rectangle(19, 19, 850, 600);
            this.pnlCenter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageHelper.ShowError("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang xử lý...";
            
            var ok = await auth.ValidateAsync(txtUsername.Text, txtPassword.Text);
            
            btnLogin.Enabled = true;
            btnLogin.Text = "ĐĂNG NHẬP";

            if (!ok)
            {
                MessageHelper.ShowError("Tên đăng nhập hoặc mật khẩu không đúng!\nVui lòng kiểm tra lại.");
                txtPassword.Text = "";
                txtPassword.Focus();
                return;
            }

            MessageHelper.ShowSuccess($"Chào mừng {txtUsername.Text}!");
            new MainForm(txtUsername.Text).Show();
            this.Hide();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            new RegisterForm().ShowDialog();
        }
    }
}
