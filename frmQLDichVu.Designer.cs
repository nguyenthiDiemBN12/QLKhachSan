namespace PMQuanLyKhachSan
{
    partial class frmQLDichVu
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQLDichVu));
            label1 = new Label();
            label7 = new Label();
            label2 = new Label();
            label3 = new Label();
            btnThem = new Button();
            btnXoa = new Button();
            btnSua = new Button();
            btnLamMoi = new Button();
            dgvDichVu = new DataGridView();
            txtMaDV = new TextBox();
            txtTenDV = new TextBox();
            txtGiaDV = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvDichVu).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Book Antiqua", 40.2F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(245, 210, 105);
            label1.Location = new Point(460, 50);
            label1.Name = "label1";
            label1.Size = new Size(662, 80);
            label1.TabIndex = 1;
            label1.Text = "QUẢN LÝ DỊCH VỤ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Transparent;
            label7.FlatStyle = FlatStyle.Flat;
            label7.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(246, 224, 175);
            label7.ImageAlign = ContentAlignment.MiddleLeft;
            label7.Location = new Point(128, 242);
            label7.Name = "label7";
            label7.Size = new Size(122, 28);
            label7.TabIndex = 38;
            label7.Text = "Mã Dịch Vụ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.FlatStyle = FlatStyle.Flat;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(246, 224, 175);
            label2.ImageAlign = ContentAlignment.MiddleLeft;
            label2.Location = new Point(128, 324);
            label2.Name = "label2";
            label2.Size = new Size(125, 28);
            label2.TabIndex = 39;
            label2.Text = "Tên Dịch Vụ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.FlatStyle = FlatStyle.Flat;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(246, 224, 175);
            label3.ImageAlign = ContentAlignment.MiddleLeft;
            label3.Location = new Point(128, 406);
            label3.Name = "label3";
            label3.Size = new Size(129, 28);
            label3.TabIndex = 40;
            label3.Text = "Giá  Dịch Vụ";
            // 
            // btnThem
            // 
            btnThem.BackColor = Color.Gold;
            btnThem.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            btnThem.Location = new Point(1247, 350);
            btnThem.Name = "btnThem";
            btnThem.Size = new Size(150, 40);
            btnThem.TabIndex = 46;
            btnThem.Text = "Thêm";
            btnThem.UseVisualStyleBackColor = false;
            btnThem.Click += btnThem_Click;
            // 
            // btnXoa
            // 
            btnXoa.BackColor = Color.Gold;
            btnXoa.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            btnXoa.Location = new Point(1247, 291);
            btnXoa.Name = "btnXoa";
            btnXoa.Size = new Size(150, 40);
            btnXoa.TabIndex = 45;
            btnXoa.Text = "Xóa";
            btnXoa.UseVisualStyleBackColor = false;
            btnXoa.Click += btnXoa_Click;
            // 
            // btnSua
            // 
            btnSua.BackColor = Color.Gold;
            btnSua.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            btnSua.Location = new Point(1247, 230);
            btnSua.Name = "btnSua";
            btnSua.Size = new Size(150, 40);
            btnSua.TabIndex = 44;
            btnSua.Text = "Sửa";
            btnSua.UseVisualStyleBackColor = false;
            btnSua.Click += btnSua_Click;
            // 
            // btnLamMoi
            // 
            btnLamMoi.BackColor = Color.Gold;
            btnLamMoi.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            btnLamMoi.Location = new Point(1247, 410);
            btnLamMoi.Name = "btnLamMoi";
            btnLamMoi.Size = new Size(150, 40);
            btnLamMoi.TabIndex = 62;
            btnLamMoi.Text = "Làm mới";
            btnLamMoi.UseVisualStyleBackColor = false;
            btnLamMoi.Click += btnLamMoi_Click;
            // 
            // dgvDichVu
            // 
            dgvDichVu.BackgroundColor = Color.FromArgb(1, 17, 32);
            dgvDichVu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDichVu.Location = new Point(40, 559);
            dgvDichVu.Name = "dgvDichVu";
            dgvDichVu.RowHeadersWidth = 51;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDichVu.RowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvDichVu.RowTemplate.Height = 29;
            dgvDichVu.Size = new Size(1458, 486);
            dgvDichVu.TabIndex = 63;
            dgvDichVu.CellClick += dgvDichVu_CellClick;
            // 
            // txtMaDV
            // 
            txtMaDV.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            txtMaDV.Location = new Point(316, 246);
            txtMaDV.Name = "txtMaDV";
            txtMaDV.Size = new Size(523, 31);
            txtMaDV.TabIndex = 64;
            // 
            // txtTenDV
            // 
            txtTenDV.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            txtTenDV.Location = new Point(316, 331);
            txtTenDV.Name = "txtTenDV";
            txtTenDV.Size = new Size(523, 31);
            txtTenDV.TabIndex = 65;
            // 
            // txtGiaDV
            // 
            txtGiaDV.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point);
            txtGiaDV.Location = new Point(316, 413);
            txtGiaDV.Name = "txtGiaDV";
            txtGiaDV.Size = new Size(523, 31);
            txtGiaDV.TabIndex = 66;
            txtGiaDV.TextChanged += txtGiaDV_TextChanged;
            // 
            // frmQLDichVu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1537, 1118);
            Controls.Add(txtGiaDV);
            Controls.Add(txtTenDV);
            Controls.Add(txtMaDV);
            Controls.Add(dgvDichVu);
            Controls.Add(btnLamMoi);
            Controls.Add(btnThem);
            Controls.Add(btnXoa);
            Controls.Add(btnSua);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label7);
            Controls.Add(label1);
            Name = "frmQLDichVu";
            Text = "frmQLDichVu";
            Load += frmQLDichVu_Load;
            ((System.ComponentModel.ISupportInitialize)dgvDichVu).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label7;
        private Label label2;
        private Label label3;
        private Button btnThem;
        private Button btnXoa;
        private Button btnSua;
        private Button btnLamMoi;
        private DataGridView dgvDichVu;
        private TextBox txtMaDV;
        private TextBox txtTenDV;
        private TextBox txtGiaDV;
    }
}