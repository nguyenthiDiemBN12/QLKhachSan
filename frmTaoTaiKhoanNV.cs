using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmTaoTaiKhoanNV : Form
    {
        public DataContext db = new DataContext();
        private NhanVien nhanVienDangNhap;

        public frmTaoTaiKhoanNV(NhanVien nv)
        {
            InitializeComponent();
            nhanVienDangNhap = nv;
        }

        private void frmTaoTaiKhoanNV_Load(object sender, EventArgs e)
        {
            cbChucVu.DataSource = db.ChucVus.ToList();
            cbChucVu.DisplayMember = "TenChucVu";
            cbChucVu.ValueMember = "MaChucVu";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnTaoTK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
        string.IsNullOrWhiteSpace(Email.Text) ||
        string.IsNullOrWhiteSpace(txtSDT.Text) ||
        string.IsNullOrWhiteSpace(txtTenDangNhap.Text) ||
        string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!radNam.Checked && !radNu.Checked)
            {
                MessageBox.Show("Vui lòng chọn giới tính!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (db.TaiKhoans.Any(x => x.TenDangNhap == txtTenDangNhap.Text.Trim()))
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tạo tài khoản
                TaiKhoan tk = new TaiKhoan()
                {
                    TenDangNhap = txtTenDangNhap.Text.Trim(),
                    MatKhau = txtMatKhau.Text.Trim()
                };

                db.TaiKhoans.Add(tk);
                db.SaveChanges();

                // Tạo nhân viên
                NhanVien nv = new NhanVien()
                {
                    HoTen = txtHoTen.Text.Trim(),
                    NgaySinh = dtpNgaySinh.Value,

                    // Nếu Model bool
                    GioiTinh = radNam.Checked,

                    SDT = txtSDT.Text.Trim(),
                    Email = Email.Text.Trim(),

                    MaTK = tk.MaTK,
                    MaChucVu = Convert.ToInt32(cbChucVu.SelectedValue)
                };

                db.NhanViens.Add(nv);
                db.SaveChanges();

                MessageBox.Show("Tạo tài khoản thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtHoTen.Clear();
                txtEmail.Clear();
                txtSDT.Clear();
                txtTenDangNhap.Clear();
                txtMatKhau.Clear();

                radNam.Checked = false;
                radNu.Checked = false;
                cbChucVu.SelectedIndex = 0;
                dtpNgaySinh.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            frmMainQuanLy frm = new frmMainQuanLy(nhanVienDangNhap);
            frm.Show();
            this.Close();
        }

        private void btnQLNV_Click(object sender, EventArgs e)
        {
            frmQLNhanVien frm = new frmQLNhanVien();
            frm.Show();
            this.Close();
        }
    }
}
