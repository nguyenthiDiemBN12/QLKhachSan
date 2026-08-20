using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace PMQuanLyKhachSan
{
    public partial class frmDangNhap : Form
    {
        public DataContext db = new DataContext();
        public NhanVien nhanVienDangNhap;

        public frmDangNhap()
        {
            InitializeComponent();
            this.FormClosed += frmDangNhapNV_FormClosed;
        }

        //================ ĐĂNG NHẬP ====================
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtTenDangNhap.Text.Trim();
            string password = txtMatKhau.Text.Trim();

            // 1. Validation cơ bản: Nhập thiếu thông tin
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra TÀI KHOẢN KHÔNG TỒN TẠI
            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == username);
            if (taiKhoan == null)
            {
                MessageBox.Show("Tài khoản không tồn tại trên hệ thống!",
                    "Lỗi đăng nhập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtTenDangNhap.Focus();
                return;
            }

            // 3. Kiểm tra MẬT KHẨU
            if (taiKhoan.MatKhau != password)
            {
                MessageBox.Show("Mật khẩu không chính xác!",
                    "Lỗi đăng nhập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtMatKhau.Focus();
                return;
            }

            // 4. Lấy thông tin nhân viên & chức vụ
            var nv = db.NhanViens
                .Include(x => x.ChucVu)
                .FirstOrDefault(x => x.MaTK == taiKhoan.MaTK);

            if (nv == null || nv.ChucVu == null)
            {
                MessageBox.Show("Tài khoản chưa được gán thông tin nhân viên hoặc chức vụ!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            nhanVienDangNhap = nv;
            string tenChucVu = nv.ChucVu.TenChucVu;

            // 5. PHÂN QUYỀN CHUYỂN FORM: Chỉ chấp nhận "Quản lý" và "Lễ tân"
            if (tenChucVu.Equals("Quản lý", StringComparison.OrdinalIgnoreCase))
            {
                frmMainQuanLy frm = new frmMainQuanLy(nv);
                frm.Show();
                this.Hide();
            }
            else if (tenChucVu.Equals("Lễ tân", StringComparison.OrdinalIgnoreCase))
            {
                frmLeTan frm = new frmLeTan(nv);
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Tài khoản của bạn không có quyền truy cập hệ thống!",
                    "Cảnh báo truy cập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        //================ XỬ LÝ QUÊN / ĐỔI MẬT KHẨU ====================
        private void btnDoiMK_Click(object sender, EventArgs e)
        {
            string username = txtTenDangNhap.Text.Trim();

            // 1. Kiểm tra chưa nhập tên đăng nhập
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Vui lòng nhập Tên đăng nhập trước khi thực hiện đổi / quên mật khẩu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtTenDangNhap.Focus();
                return;
            }

            // 2. Kiểm tra Tên đăng nhập có tồn tại trong bảng TaiKhoan hay không
            var taiKhoan = db.TaiKhoans.FirstOrDefault(tk => tk.TenDangNhap == username);
            if (taiKhoan == null)
            {
                MessageBox.Show("Tên đăng nhập không tồn tại trong hệ thống!",
                    "Lỗi xác thực",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtTenDangNhap.Focus();
                return;
            }

            // 3. Tìm nhân viên sở hữu tài khoản này
            var nv = db.NhanViens.FirstOrDefault(x => x.MaTK == taiKhoan.MaTK);

            // 4. Nếu hợp lệ, mở Form Đổi mật khẩu và truyền thông tin nhân viên sang
            frmDoiMK frm = new frmDoiMK(nv);
            frm.Show();
            this.Hide();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmDangNhapNV_Load(object sender, EventArgs e)
        {
        }

        private void frmDangNhapNV_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose();
        }
    }
}