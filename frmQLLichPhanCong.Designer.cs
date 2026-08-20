namespace PMQuanLyKhachSan
{
    partial class frmQLLichPhanCong
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQLLichPhanCong));
            label4 = new Label();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Book Antiqua", 40.2F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.FromArgb(245, 210, 105);
            label4.Location = new Point(327, 51);
            label4.Name = "label4";
            label4.Size = new Size(984, 80);
            label4.TabIndex = 45;
            label4.Text = "QUẢN LÝ LỊCH PHÂN CÔNG";
            // 
            // frmQLLichPhanCong
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1545, 1122);
            Controls.Add(label4);
            Name = "frmQLLichPhanCong";
            Text = "frmQLLichPhanCong";
            Load += frmQLLichPhanCong_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label4;
    }
}