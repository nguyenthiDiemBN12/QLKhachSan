using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmLeTan : Form
    {
        public DataContext db = new DataContext();

        private NhanVien nhanVienDangNhap;

        public frmLeTan(NhanVien nv)
        {
            InitializeComponent();
            nhanVienDangNhap = nv;
        }

        private void BoGocButton(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            btn.Region = new Region(path);
        }
        private void OpenChildForm(Form childForm)
        {
            pnlContent.Controls.Clear();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;

            childForm.Show();
        }
        private void frmLichLamViec_Load(object sender, EventArgs e)
        {
            btnNhanVien.Text = nhanVienDangNhap.HoTen;
            lblChucVu.Text = nhanVienDangNhap.ChucVu.TenChucVu;
            BoGocButton(btnNhanVien, 30);

            OpenChildForm(new AnhBia());
        }

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            frmTTNhanVien frm = new frmTTNhanVien();
            frm.Show();
            this.Hide();
        }


        private void btnThoat_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblChucVu_Click(object sender, EventArgs e)
        {

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            frmDangNhap frm = new frmDangNhap();
            frm.Show();
            this.Hide();
        }

        private void btnLichSu_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmXemLichSu(nhanVienDangNhap));
        }

        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmDatPhong(nhanVienDangNhap));
        }

        private void btnDatDV_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmDatDV(nhanVienDangNhap));
        }

        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            // Lấy đơn đặt phòng vừa được thêm gần nhất trong hệ thống
            var dpGoiNhat = db.DatPhongs
                              .OrderByDescending(x => x.NgayDat)
                              .FirstOrDefault();

            if (dpGoiNhat != null)
            {
                // Mở Form hóa đơn với mã đặt phòng mới nhất
                OpenChildForm(new frmHoaDonKhach(dpGoiNhat.MaDatPhong));
            }
            else
            {
                // Nếu cơ sở dữ liệu hoàn toàn chưa có đơn đặt phòng nào
                OpenChildForm(new frmHoaDonKhach(0));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmQLDatPhong(nhanVienDangNhap));
        }
    }
}
