namespace PMQuanLyKhachSan
{
    partial class frmLeTan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLeTan));
            btnNhanVien = new Button();
            lblChucVu = new Label();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            btnDangXuat = new Button();
            btnLichSu = new Button();
            btnHoaDon = new Button();
            btnDatDV = new Button();
            btnDatPhong = new Button();
            btnThoat = new Button();
            pnlContent = new Panel();
            btnQLDatPhong = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // btnNhanVien
            // 
            btnNhanVien.BackColor = Color.FromArgb(4, 17, 33);
            btnNhanVien.FlatStyle = FlatStyle.Flat;
            btnNhanVien.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnNhanVien.ForeColor = Color.FromArgb(246, 224, 175);
            btnNhanVien.Image = (Image)resources.GetObject("btnNhanVien.Image");
            btnNhanVien.ImageAlign = ContentAlignment.TopCenter;
            btnNhanVien.Location = new Point(15, 20);
            btnNhanVien.Name = "btnNhanVien";
            btnNhanVien.Size = new Size(288, 74);
            btnNhanVien.TabIndex = 12;
            btnNhanVien.Text = "Tên nhân viên";
            btnNhanVien.TextAlign = ContentAlignment.TopCenter;
            btnNhanVien.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnNhanVien.UseVisualStyleBackColor = false;
            btnNhanVien.Click += btnNhanVien_Click;
            // 
            // lblChucVu
            // 
            lblChucVu.AutoSize = true;
            lblChucVu.BackColor = Color.FromArgb(4, 17, 33);
            lblChucVu.FlatStyle = FlatStyle.Flat;
            lblChucVu.ForeColor = Color.FromArgb(246, 224, 175);
            lblChucVu.ImageAlign = ContentAlignment.MiddleLeft;
            lblChucVu.Location = new Point(107, 59);
            lblChucVu.Name = "lblChucVu";
            lblChucVu.Size = new Size(61, 20);
            lblChucVu.TabIndex = 18;
            lblChucVu.Text = "Chức vụ";
            lblChucVu.Click += lblChucVu_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(47, 116);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(212, 280);
            pictureBox1.TabIndex = 36;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Tan;
            panel1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point);
            panel1.Location = new Point(318, -251);
            panel1.Name = "panel1";
            panel1.Size = new Size(4, 1434);
            panel1.TabIndex = 37;
            // 
            // btnDangXuat
            // 
            btnDangXuat.BackColor = Color.FromArgb(4, 17, 33);
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnDangXuat.ForeColor = Color.FromArgb(246, 224, 175);
            btnDangXuat.Image = (Image)resources.GetObject("btnDangXuat.Image");
            btnDangXuat.Location = new Point(22, 905);
            btnDangXuat.Name = "btnDangXuat";
            btnDangXuat.Size = new Size(277, 50);
            btnDangXuat.TabIndex = 44;
            btnDangXuat.Text = "Đăng xuất";
            btnDangXuat.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDangXuat.UseVisualStyleBackColor = false;
            btnDangXuat.Click += btnDangXuat_Click;
            // 
            // btnLichSu
            // 
            btnLichSu.BackColor = Color.FromArgb(4, 17, 33);
            btnLichSu.FlatAppearance.BorderSize = 0;
            btnLichSu.FlatStyle = FlatStyle.Flat;
            btnLichSu.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnLichSu.ForeColor = Color.FromArgb(246, 224, 175);
            btnLichSu.Image = (Image)resources.GetObject("btnLichSu.Image");
            btnLichSu.Location = new Point(27, 695);
            btnLichSu.Name = "btnLichSu";
            btnLichSu.Size = new Size(253, 58);
            btnLichSu.TabIndex = 42;
            btnLichSu.Text = "Tra cứu đặt phòng";
            btnLichSu.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnLichSu.UseVisualStyleBackColor = false;
            btnLichSu.Click += btnLichSu_Click;
            // 
            // btnHoaDon
            // 
            btnHoaDon.BackColor = Color.FromArgb(4, 17, 33);
            btnHoaDon.FlatAppearance.BorderSize = 0;
            btnHoaDon.FlatStyle = FlatStyle.Flat;
            btnHoaDon.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnHoaDon.ForeColor = Color.FromArgb(246, 224, 175);
            btnHoaDon.Image = (Image)resources.GetObject("btnHoaDon.Image");
            btnHoaDon.Location = new Point(6, 620);
            btnHoaDon.Name = "btnHoaDon";
            btnHoaDon.Size = new Size(279, 61);
            btnHoaDon.TabIndex = 41;
            btnHoaDon.Text = "Xuất hóa đơn";
            btnHoaDon.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnHoaDon.UseVisualStyleBackColor = false;
            btnHoaDon.Click += btnHoaDon_Click;
            // 
            // btnDatDV
            // 
            btnDatDV.BackColor = Color.FromArgb(4, 17, 33);
            btnDatDV.FlatAppearance.BorderSize = 0;
            btnDatDV.FlatStyle = FlatStyle.Flat;
            btnDatDV.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnDatDV.ForeColor = Color.FromArgb(246, 224, 175);
            btnDatDV.Image = (Image)resources.GetObject("btnDatDV.Image");
            btnDatDV.Location = new Point(15, 546);
            btnDatDV.Name = "btnDatDV";
            btnDatDV.Size = new Size(274, 61);
            btnDatDV.TabIndex = 40;
            btnDatDV.Text = "Đặt dịch vụ";
            btnDatDV.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDatDV.UseVisualStyleBackColor = false;
            btnDatDV.Click += btnDatDV_Click;
            // 
            // btnDatPhong
            // 
            btnDatPhong.BackColor = Color.FromArgb(4, 17, 33);
            btnDatPhong.FlatAppearance.BorderSize = 0;
            btnDatPhong.FlatStyle = FlatStyle.Flat;
            btnDatPhong.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnDatPhong.ForeColor = Color.FromArgb(246, 224, 175);
            btnDatPhong.Image = (Image)resources.GetObject("btnDatPhong.Image");
            btnDatPhong.Location = new Point(22, 473);
            btnDatPhong.Name = "btnDatPhong";
            btnDatPhong.Size = new Size(274, 61);
            btnDatPhong.TabIndex = 39;
            btnDatPhong.Text = "Đặt phòng";
            btnDatPhong.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDatPhong.UseVisualStyleBackColor = false;
            btnDatPhong.Click += btnDatPhong_Click;
            // 
            // btnThoat
            // 
            btnThoat.BackColor = Color.FromArgb(4, 17, 33);
            btnThoat.FlatAppearance.BorderSize = 0;
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnThoat.ForeColor = Color.FromArgb(246, 224, 175);
            btnThoat.Image = (Image)resources.GetObject("btnThoat.Image");
            btnThoat.ImageAlign = ContentAlignment.MiddleRight;
            btnThoat.Location = new Point(8, 971);
            btnThoat.Name = "btnThoat";
            btnThoat.Size = new Size(291, 50);
            btnThoat.TabIndex = 38;
            btnThoat.Text = "Thoát chương trình";
            btnThoat.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnThoat.UseVisualStyleBackColor = false;
            btnThoat.Click += btnThoat_Click;
            // 
            // pnlContent
            // 
            pnlContent.Location = new Point(348, -6);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(1522, 1107);
            pnlContent.TabIndex = 45;
            // 
            // btnQLDatPhong
            // 
            btnQLDatPhong.BackColor = Color.FromArgb(4, 17, 33);
            btnQLDatPhong.FlatAppearance.BorderSize = 0;
            btnQLDatPhong.FlatStyle = FlatStyle.Flat;
            btnQLDatPhong.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnQLDatPhong.ForeColor = Color.FromArgb(246, 224, 175);
            btnQLDatPhong.Image = (Image)resources.GetObject("btnQLDatPhong.Image");
            btnQLDatPhong.Location = new Point(32, 768);
            btnQLDatPhong.Name = "btnQLDatPhong";
            btnQLDatPhong.Size = new Size(253, 58);
            btnQLDatPhong.TabIndex = 46;
            btnQLDatPhong.Text = "Quản lý đặt phòng";
            btnQLDatPhong.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnQLDatPhong.UseVisualStyleBackColor = false;
            btnQLDatPhong.Click += button1_Click;
            // 
            // frmLeTan
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(1, 12, 25);
            ClientSize = new Size(1900, 1098);
            Controls.Add(btnQLDatPhong);
            Controls.Add(pnlContent);
            Controls.Add(btnDangXuat);
            Controls.Add(btnLichSu);
            Controls.Add(btnHoaDon);
            Controls.Add(btnDatDV);
            Controls.Add(btnDatPhong);
            Controls.Add(btnThoat);
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(lblChucVu);
            Controls.Add(btnNhanVien);
            Name = "frmLeTan";
            Text = "frmLeTan";
            Load += frmLichLamViec_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnNhanVien;
        private Label lblChucVu;
        private PictureBox pictureBox1;
        private Panel panel1;
        private Button btnDangXuat;
        private Button btnLichSu;
        private Button btnHoaDon;
        private Button btnDatDV;
        private Button btnDatPhong;
        private Button btnThoat;
        private Panel pnlContent;
        private Button btnQLDatPhong;
    }
}