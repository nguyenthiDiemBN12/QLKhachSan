namespace PMQuanLyKhachSan
{
    partial class frmDatPhong
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDatPhong));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            txtTenKH = new TextBox();
            txtCCCD = new TextBox();
            txtSDT = new TextBox();
            dtpNgayTra = new DateTimePicker();
            dtpNgayNhan = new DateTimePicker();
            label1 = new Label();
            lblChucVu = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            cbLoai = new ComboBox();
            btnDatPhong = new Button();
            dgvPhong = new DataGridView();
            pictureBox1 = new PictureBox();
            btnTim = new Button();
            label9 = new Label();
            dtpNgayBD = new DateTimePicker();
            dtpNgayKT = new DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)dgvPhong).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // txtTenKH
            // 
            txtTenKH.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            txtTenKH.Location = new Point(256, 216);
            txtTenKH.Name = "txtTenKH";
            txtTenKH.Size = new Size(309, 34);
            txtTenKH.TabIndex = 3;
            // 
            // txtCCCD
            // 
            txtCCCD.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            txtCCCD.Location = new Point(255, 276);
            txtCCCD.Name = "txtCCCD";
            txtCCCD.Size = new Size(310, 34);
            txtCCCD.TabIndex = 4;
            // 
            // txtSDT
            // 
            txtSDT.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            txtSDT.Location = new Point(255, 343);
            txtSDT.Name = "txtSDT";
            txtSDT.Size = new Size(316, 34);
            txtSDT.TabIndex = 5;
            // 
            // dtpNgayTra
            // 
            dtpNgayTra.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            dtpNgayTra.Location = new Point(255, 482);
            dtpNgayTra.Name = "dtpNgayTra";
            dtpNgayTra.Size = new Size(310, 31);
            dtpNgayTra.TabIndex = 6;
            // 
            // dtpNgayNhan
            // 
            dtpNgayNhan.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            dtpNgayNhan.Location = new Point(256, 420);
            dtpNgayNhan.Name = "dtpNgayNhan";
            dtpNgayNhan.Size = new Size(315, 31);
            dtpNgayNhan.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.FlatStyle = FlatStyle.Flat;
            label1.Image = (Image)resources.GetObject("label1.Image");
            label1.Location = new Point(1073, 149);
            label1.Name = "label1";
            label1.Size = new Size(0, 20);
            label1.TabIndex = 8;
            // 
            // lblChucVu
            // 
            lblChucVu.AutoSize = true;
            lblChucVu.BackColor = Color.FromArgb(4, 17, 33);
            lblChucVu.FlatStyle = FlatStyle.Flat;
            lblChucVu.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            lblChucVu.ForeColor = Color.FromArgb(246, 224, 175);
            lblChucVu.ImageAlign = ContentAlignment.MiddleLeft;
            lblChucVu.Location = new Point(1050, 145);
            lblChucVu.Name = "lblChucVu";
            lblChucVu.Size = new Size(45, 23);
            lblChucVu.TabIndex = 19;
            lblChucVu.Text = "_____";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(4, 17, 33);
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(246, 224, 175);
            label2.ImageAlign = ContentAlignment.MiddleLeft;
            label2.Location = new Point(59, 343);
            label2.Name = "label2";
            label2.Size = new Size(50, 28);
            label2.TabIndex = 20;
            label2.Text = "SĐT";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(4, 17, 33);
            label3.FlatStyle = FlatStyle.Flat;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(246, 224, 175);
            label3.ImageAlign = ContentAlignment.MiddleLeft;
            label3.Location = new Point(55, 278);
            label3.Name = "label3";
            label3.Size = new Size(61, 28);
            label3.TabIndex = 21;
            label3.Text = "CCCD";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(4, 17, 33);
            label4.FlatStyle = FlatStyle.Flat;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.FromArgb(246, 224, 175);
            label4.ImageAlign = ContentAlignment.MiddleLeft;
            label4.Location = new Point(55, 216);
            label4.Name = "label4";
            label4.Size = new Size(160, 28);
            label4.TabIndex = 22;
            label4.Text = "Tên khách hàng";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(4, 17, 33);
            label5.FlatStyle = FlatStyle.Flat;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.FromArgb(246, 224, 175);
            label5.ImageAlign = ContentAlignment.MiddleLeft;
            label5.Location = new Point(51, 539);
            label5.Name = "label5";
            label5.Size = new Size(117, 28);
            label5.TabIndex = 23;
            label5.Text = "Loại phòng";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.FromArgb(4, 17, 33);
            label6.FlatStyle = FlatStyle.Flat;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label6.ForeColor = Color.FromArgb(246, 224, 175);
            label6.ImageAlign = ContentAlignment.MiddleLeft;
            label6.Location = new Point(51, 481);
            label6.Name = "label6";
            label6.Size = new Size(161, 28);
            label6.TabIndex = 24;
            label6.Text = "Ngày trả phòng";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.FromArgb(4, 17, 33);
            label7.FlatStyle = FlatStyle.Flat;
            label7.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(246, 224, 175);
            label7.ImageAlign = ContentAlignment.MiddleLeft;
            label7.Location = new Point(51, 420);
            label7.Name = "label7";
            label7.RightToLeft = RightToLeft.No;
            label7.Size = new Size(181, 33);
            label7.TabIndex = 25;
            label7.Text = "Ngày nhận phòng";
            label7.TextAlign = ContentAlignment.MiddleRight;
            label7.UseCompatibleTextRendering = true;
            // 
            // cbLoai
            // 
            cbLoai.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            cbLoai.FormattingEnabled = true;
            cbLoai.Location = new Point(255, 542);
            cbLoai.Name = "cbLoai";
            cbLoai.Size = new Size(316, 36);
            cbLoai.TabIndex = 26;
            // 
            // btnDatPhong
            // 
            btnDatPhong.BackColor = Color.Gold;
            btnDatPhong.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            btnDatPhong.ForeColor = SystemColors.ActiveCaptionText;
            btnDatPhong.Image = (Image)resources.GetObject("btnDatPhong.Image");
            btnDatPhong.ImageAlign = ContentAlignment.MiddleRight;
            btnDatPhong.Location = new Point(93, 645);
            btnDatPhong.Name = "btnDatPhong";
            btnDatPhong.Size = new Size(414, 75);
            btnDatPhong.TabIndex = 27;
            btnDatPhong.Text = "  Đặt phòng";
            btnDatPhong.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDatPhong.UseVisualStyleBackColor = false;
            btnDatPhong.Click += btnDatPhong_Click;
            // 
            // dgvPhong
            // 
            dgvPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPhong.BackgroundColor = Color.FromArgb(5, 21, 36);
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.White;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvPhong.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvPhong.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.White;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvPhong.DefaultCellStyle = dataGridViewCellStyle2;
            dgvPhong.EnableHeadersVisualStyles = false;
            dgvPhong.GridColor = Color.LightSlateGray;
            dgvPhong.Location = new Point(680, 216);
            dgvPhong.Name = "dgvPhong";
            dgvPhong.RowHeadersWidth = 51;
            dgvPhong.RowTemplate.Height = 29;
            dgvPhong.Size = new Size(831, 837);
            dgvPhong.TabIndex = 28;
            dgvPhong.CellContentClick += dgvPhong_CellContentClick;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.Location = new Point(717, 149);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(30, 30);
            pictureBox1.TabIndex = 29;
            pictureBox1.TabStop = false;
            // 
            // btnTim
            // 
            btnTim.FlatAppearance.BorderSize = 0;
            btnTim.FlatStyle = FlatStyle.Flat;
            btnTim.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point);
            btnTim.ForeColor = Color.FromArgb(246, 224, 175);
            btnTim.Image = (Image)resources.GetObject("btnTim.Image");
            btnTim.ImageAlign = ContentAlignment.MiddleLeft;
            btnTim.Location = new Point(1390, 144);
            btnTim.Name = "btnTim";
            btnTim.Size = new Size(86, 44);
            btnTim.TabIndex = 30;
            btnTim.Text = "Tìm";
            btnTim.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnTim.UseVisualStyleBackColor = true;
            btnTim.Click += btnTim_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.FlatStyle = FlatStyle.Flat;
            label9.Image = (Image)resources.GetObject("label9.Image");
            label9.Location = new Point(1071, 148);
            label9.Name = "label9";
            label9.Size = new Size(0, 20);
            label9.TabIndex = 33;
            // 
            // dtpNgayBD
            // 
            dtpNgayBD.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            dtpNgayBD.Location = new Point(763, 149);
            dtpNgayBD.Name = "dtpNgayBD";
            dtpNgayBD.Size = new Size(280, 30);
            dtpNgayBD.TabIndex = 32;
            // 
            // dtpNgayKT
            // 
            dtpNgayKT.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            dtpNgayKT.Location = new Point(1102, 150);
            dtpNgayKT.Name = "dtpNgayKT";
            dtpNgayKT.Size = new Size(282, 30);
            dtpNgayKT.TabIndex = 31;
            // 
            // frmDatPhong
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(1, 12, 25);
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1540, 1098);
            Controls.Add(label9);
            Controls.Add(dtpNgayBD);
            Controls.Add(dtpNgayKT);
            Controls.Add(btnTim);
            Controls.Add(pictureBox1);
            Controls.Add(dgvPhong);
            Controls.Add(btnDatPhong);
            Controls.Add(cbLoai);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(lblChucVu);
            Controls.Add(label1);
            Controls.Add(dtpNgayNhan);
            Controls.Add(dtpNgayTra);
            Controls.Add(txtSDT);
            Controls.Add(txtCCCD);
            Controls.Add(txtTenKH);
            Name = "frmDatPhong";
            Text = "frmDatPhong";
            Load += frmDatPhong_Load;
            ((System.ComponentModel.ISupportInitialize)dgvPhong).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox txtTenKH;
        private TextBox txtCCCD;
        private TextBox txtSDT;
        private DateTimePicker dtpNgayTra;
        private DateTimePicker dtpNgayNhan;
        private Label label1;
        private Label lblChucVu;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private ComboBox cbLoai;
        private Button btnDatPhong;
        private DataGridView dgvPhong;
        private PictureBox pictureBox1;
        private Button btnTim;
        private Label label9;
        private DateTimePicker dtpNgayBD;
        private DateTimePicker dtpNgayKT;
    }
}