using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;
using SupermarketApp.Services;
using SupermarketApp.Utils;

namespace SupermarketApp.Forms
{
    public partial class RegisterForm : UIForm
    {
        private UILabel lblTitle;
        private UILabel lblFullName;
        private UILabel lblUsername;
        private UILabel lblPassword;
        private UILabel lblConfirmPassword;
        private UITextBox txtFullName;
        private UITextBox txtUsername;
        private UITextBox txtPassword;
        private UITextBox txtConfirmPassword;
        private UIButton btnRegister;
        private UIButton btnCancel;
        private UIPanel pnlCenter;
        private readonly AuthService auth = new AuthService();

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pnlCenter = new Sunny.UI.UIPanel();
            this.lblTitle = new Sunny.UI.UILabel();
            this.lblFullName = new Sunny.UI.UILabel();
            this.txtFullName = new Sunny.UI.UITextBox();
            this.lblUsername = new Sunny.UI.UILabel();
            this.txtUsername = new Sunny.UI.UITextBox();
            this.lblPassword = new Sunny.UI.UILabel();
            this.txtPassword = new Sunny.UI.UITextBox();
            this.lblConfirmPassword = new Sunny.UI.UILabel();
            this.txtConfirmPassword = new Sunny.UI.UITextBox();
            this.btnRegister = new Sunny.UI.UIButton();
            this.btnCancel = new Sunny.UI.UIButton();
            this.pnlCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.White;
            this.pnlCenter.Controls.Add(this.btnCancel);
            this.pnlCenter.Controls.Add(this.btnRegister);
            this.pnlCenter.Controls.Add(this.txtConfirmPassword);
            this.pnlCenter.Controls.Add(this.lblConfirmPassword);
            this.pnlCenter.Controls.Add(this.txtPassword);
            this.pnlCenter.Controls.Add(this.lblPassword);
            this.pnlCenter.Controls.Add(this.txtUsername);
            this.pnlCenter.Controls.Add(this.lblUsername);
            this.pnlCenter.Controls.Add(this.txtFullName);
            this.pnlCenter.Controls.Add(this.lblFullName);
            this.pnlCenter.Controls.Add(this.lblTitle);
            this.pnlCenter.FillColor = System.Drawing.Color.White;
            this.pnlCenter.FillColor2 = System.Drawing.Color.White;
            this.pnlCenter.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.pnlCenter.Location = new System.Drawing.Point(150, 60);
            this.pnlCenter.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlCenter.MinimumSize = new System.Drawing.Size(1, 1);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Padding = new System.Windows.Forms.Padding(30);
            this.pnlCenter.Radius = 15;
            this.pnlCenter.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.pnlCenter.RectSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.None;
            this.pnlCenter.Size = new System.Drawing.Size(520, 570);
            this.pnlCenter.TabIndex = 0;
            this.pnlCenter.Text = null;
            this.pnlCenter.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.lblTitle.Location = new System.Drawing.Point(35, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(450, 50);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ĐĂNG KÝ TÀI KHOẢN";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFullName
            // 
            this.lblFullName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFullName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblFullName.Location = new System.Drawing.Point(35, 100);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(150, 25);
            this.lblFullName.TabIndex = 1;
            this.lblFullName.Text = "Họ và tên";
            this.lblFullName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtFullName
            // 
            this.txtFullName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFullName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFullName.Location = new System.Drawing.Point(35, 130);
            this.txtFullName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtFullName.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Padding = new System.Windows.Forms.Padding(5);
            this.txtFullName.Radius = 5;
            this.txtFullName.ShowText = false;
            this.txtFullName.Size = new System.Drawing.Size(450, 38);
            this.txtFullName.TabIndex = 2;
            this.txtFullName.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtFullName.Watermark = "Nhập họ và tên...";
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblUsername.Location = new System.Drawing.Point(35, 180);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(150, 25);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Tên đăng nhập";
            this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUsername
            // 
            this.txtUsername.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsername.Location = new System.Drawing.Point(35, 210);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUsername.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Padding = new System.Windows.Forms.Padding(5);
            this.txtUsername.Radius = 5;
            this.txtUsername.ShowText = false;
            this.txtUsername.Size = new System.Drawing.Size(450, 38);
            this.txtUsername.TabIndex = 4;
            this.txtUsername.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtUsername.Watermark = "Nhập tên đăng nhập...";
            // 
            // lblPassword
            // 
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblPassword.Location = new System.Drawing.Point(35, 260);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(150, 25);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Mật khẩu";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPassword
            // 
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPassword.Location = new System.Drawing.Point(35, 290);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Radius = 5;
            this.txtPassword.ShowText = false;
            this.txtPassword.Size = new System.Drawing.Size(450, 38);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtPassword.Watermark = "Nhập mật khẩu...";
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.lblConfirmPassword.Location = new System.Drawing.Point(35, 340);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(200, 25);
            this.lblConfirmPassword.TabIndex = 7;
            this.lblConfirmPassword.Text = "Xác nhận mật khẩu";
            this.lblConfirmPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(35, 370);
            this.txtConfirmPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConfirmPassword.MinimumSize = new System.Drawing.Size(1, 16);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Padding = new System.Windows.Forms.Padding(5);
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.Radius = 5;
            this.txtConfirmPassword.ShowText = false;
            this.txtConfirmPassword.Size = new System.Drawing.Size(450, 38);
            this.txtConfirmPassword.TabIndex = 8;
            this.txtConfirmPassword.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.txtConfirmPassword.Watermark = "Nhập lại mật khẩu...";
            // 
            // btnRegister
            // 
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.btnRegister.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnRegister.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(39)))), ((int)(((byte)(119)))));
            this.btnRegister.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(24)))), ((int)(((byte)(93)))));
            this.btnRegister.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnRegister.Location = new System.Drawing.Point(35, 450);
            this.btnRegister.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Radius = 5;
            this.btnRegister.Size = new System.Drawing.Size(215, 45);
            this.btnRegister.TabIndex = 9;
            this.btnRegister.Text = "ĐĂNG KÝ";
            this.btnRegister.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.btnCancel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnCancel.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnCancel.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(270, 450);
            this.btnCancel.MinimumSize = new System.Drawing.Size(1, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 5;
            this.btnCancel.Size = new System.Drawing.Size(215, 45);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "HỦY";
            this.btnCancel.TipsFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // RegisterForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.ClientSize = new System.Drawing.Size(820, 680);
            this.Controls.Add(this.pnlCenter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng ký tài khoản - SupermarketApp";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(72)))), ((int)(((byte)(153)))));
            this.pnlCenter.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            // Kiểm tra validation
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập họ và tên!");
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập tên đăng nhập!");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageHelper.ShowWarning("Vui lòng nhập mật khẩu!");
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageHelper.ShowWarning("Mật khẩu phải có ít nhất 6 ký tự!");
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageHelper.ShowError("Mật khẩu xác nhận không khớp!\nVui lòng kiểm tra lại.");
                txtConfirmPassword.Text = "";
                txtConfirmPassword.Focus();
                return;
            }

            btnRegister.Enabled = false;
            btnRegister.Text = "Đang xử lý...";

            bool ok = await auth.RegisterAsync(txtFullName.Text, txtUsername.Text, txtPassword.Text);

            btnRegister.Enabled = true;
            btnRegister.Text = "ĐĂNG KÝ";

            if (!ok)
            {
                MessageHelper.ShowError("Tên đăng nhập đã tồn tại!\nVui lòng chọn tên đăng nhập khác.");
                txtUsername.Focus();
                return;
            }

            MessageHelper.ShowSuccess("Đăng ký tài khoản thành công!\nBạn có thể đăng nhập ngay bây giờ.");
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
