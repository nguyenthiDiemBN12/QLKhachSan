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
    public partial class frmDoiMK : Form
    {
        public DataContext db = new DataContext();

        private NhanVien quanLyDangNhap;

        // Constructor 1: Mặc định
        public frmDoiMK()
        {
            InitializeComponent();
        }

        //================ CONSTRUCTOR TRUYỀN TÊN ĐĂNG NHẬP (ƯU TIÊN DÙNG) ====================
        public frmDoiMK(string tenDangNhap)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(tenDangNhap))
            {
                txtTenDangNhap.Text = tenDangNhap;
                txtTenDangNhap.ReadOnly = true; // Tự động điền và khóa ô tên đăng nhập
            }
            else
            {
                txtTenDangNhap.Clear();
                txtTenDangNhap.ReadOnly = false;
            }
        }

        //================ CONSTRUCTOR TRUYỀN NHÂN VIÊN ====================
        public frmDoiMK(NhanVien ql)
        {
            InitializeComponent();

            quanLyDangNhap = ql;

            if (ql != null)
            {
                // Điền sẵn tên đăng nhập từ bảng TaiKhoan
                var tk = db.TaiKhoans.FirstOrDefault(x => x.MaNV == ql.MaNV);
                if (tk != null)
                {
                    txtTenDangNhap.Text = tk.TenDangNhap;
                    txtTenDangNhap.ReadOnly = true;
                }
            }
            else
            {
                txtTenDangNhap.Clear();
                txtTenDangNhap.ReadOnly = false;
            }
        }

        //================ ĐỔI MẬT KHẨU ====================
        private void btnDoiMK_Click(object sender, EventArgs e)
        {
            string user = txtTenDangNhap.Text.Trim();
            string mkMoi = txtMKMoi.Text.Trim();
            string xacNhan = txtXacNhan.Text.Trim();

            if (user == "" || mkMoi == "" || xacNhan == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (mkMoi != xacNhan)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (mkMoi.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tìm tài khoản theo tên đăng nhập
            var tk = db.TaiKhoans.FirstOrDefault(x => x.TenDangNhap == user);

            if (tk != null)
            {
                tk.MatKhau = mkMoi;
                db.SaveChanges();

                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Chuyển sang frmDangNhap
                frmDangNhap fDangNhap = new frmDangNhap();
                fDangNhap.Show();
                this.Close();
                return;
            }

            MessageBox.Show("Tên đăng nhập không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //================ QUAY LẠI ====================
        private void btnQuayLai_Click(object sender, EventArgs e)
        {
            frmDangNhap fDangNhap = new frmDangNhap();
            fDangNhap.Show();
            this.Close();
        }

        private void frmDoiMK_Load(object sender, EventArgs e)
        {

        }
    }
}