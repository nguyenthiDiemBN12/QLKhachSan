using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmDatPhong : Form
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private const string TRANG_THAI_DANG_THUE = "Đang có khách";
        private const string TRANG_THAI_TRONG = "Trống";
        private const string TRANG_THAI_DA_HUY = "Đã hủy";
        private const string TRANG_THAI_DA_TRA = "Đã trả phòng";
        private const string TRANG_THAI_DA_DAT = "Đã đặt";

        public DataContext db = new DataContext();

        private KhachHang khachDangDat;
        private NhanVien nhanVienDat;
        private bool laKhachHang;

        private class PhongVM
        {
            public int MaPhong { get; set; }
            public string TenPhong { get; set; }
            public string LoaiPhong { get; set; }
            public decimal Gia { get; set; }
            public string TrangThai { get; set; }
        }

        //================ KHÁCH HÀNG ===================
        public frmDatPhong(KhachHang kh)
        {
            InitializeComponent();
            khachDangDat = kh;
            laKhachHang = true;
        }

        //================ NHÂN VIÊN ====================
        public frmDatPhong(NhanVien nv)
        {
            InitializeComponent();
            nhanVienDat = nv;
            laKhachHang = false;
        }

        //================ LOAD FORM ====================
        private void frmDatPhong_Load(object sender, EventArgs e)
        {
            this.FormClosed += frmDatPhong_FormClosed;

            cbLoai.DataSource = db.LoaiPhongs.ToList();
            cbLoai.DisplayMember = "TenLoai";
            cbLoai.ValueMember = "MaLoai";

            LoadDGV();

            if (laKhachHang)
            {
                txtTenKH.Text = khachDangDat.HoTen;
                txtCCCD.Text = khachDangDat.CCCD;
                txtSDT.Text = khachDangDat.SDT;

                txtTenKH.ReadOnly = true;
                txtCCCD.ReadOnly = true;
                txtSDT.ReadOnly = true;
            }
            else
            {
                txtTenKH.Clear();
                txtCCCD.Clear();
                txtSDT.Clear();

                txtTenKH.ReadOnly = false;
                txtCCCD.ReadOnly = false;
                txtSDT.ReadOnly = false;
            }

            dgvPhong.EnableHeadersVisualStyles = false;

            Color mauNenToi = Color.FromArgb(10, 25, 47);
            dgvPhong.RowHeadersDefaultCellStyle.BackColor = mauNenToi;
            dgvPhong.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvPhong.RowHeadersDefaultCellStyle.ForeColor = Color.White;

            int preference = 1; // 1 = Dark Theme
            DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref preference, sizeof(int));
        }

        //================ GẮN DỮ LIỆU VÀO GRID ====================
        private void BindGrid(List<PhongVM> ds)
        {
            dgvPhong.DataSource = null;
            dgvPhong.Columns.Clear();
            dgvPhong.DataSource = ds;

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
            {
                Name = "Chon",
                HeaderText = "Chọn"
            };
            dgvPhong.Columns.Insert(0, chk);
            dgvPhong.Columns["MaPhong"].HeaderText = "Mã phòng";
            dgvPhong.Columns["TenPhong"].HeaderText = "Tên phòng";
            dgvPhong.Columns["LoaiPhong"].HeaderText = "Loại phòng";

            var colGia = dgvPhong.Columns["Gia"];
            if (colGia != null)
            {
                colGia.HeaderText = "Giá";
                colGia.DefaultCellStyle.Format = "N0";
            }

            dgvPhong.Columns["TrangThai"].HeaderText = "Trạng thái";
        }

        //================ LOAD DANH SÁCH PHÒNG ====================
        private void LoadDGV()
        {
            var ds = db.Phongs
                .Select(p => new PhongVM
                {
                    MaPhong = p.MaPhong,
                    TenPhong = p.TenPhong,
                    LoaiPhong = p.LoaiPhong.TenLoai,
                    Gia = p.LoaiPhong.Gia,
                    TrangThai = p.TrangThai
                })
                .ToList();

            BindGrid(ds);
            dgvPhong.RowHeadersVisible = false;

            dgvPhong.EnableHeadersVisualStyles = false;
            dgvPhong.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55);
            dgvPhong.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;
            dgvPhong.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvPhong.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvPhong.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPhong.ColumnHeadersHeight = 35;

            dgvPhong.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgvPhong.GridColor = Color.Gray;

            dgvPhong.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPhong.RowsDefaultCellStyle.ForeColor = Color.Black;

            dgvPhong.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPhong.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
        }

        //================ TÌM PHÒNG ====================
        private void btnTim_Click(object sender, EventArgs e)
        {
            DateTime ngayBD = dtpNgayBD.Value.Date;
            DateTime ngayKT = dtpNgayKT.Value.Date;

            if (ngayKT <= ngayBD)
            {
                MessageBox.Show("Ngày kết thúc phải lớn hơn ngày bắt đầu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbLoai.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maLoai = (int)cbLoai.SelectedValue;

            var maPhongDangThue = db.DatPhongs
                .Where(dp =>
                    dp.TrangThai != TRANG_THAI_DA_HUY &&
                    dp.TrangThai != TRANG_THAI_DA_TRA &&
                    ngayBD < dp.NgayTra &&
                    ngayKT > dp.NgayNhan)
                .Select(dp => dp.MaPhong)
                .Distinct()
                .ToList();

            var ds = db.Phongs
                .Where(p => p.MaLoai == maLoai)
                .Select(p => new PhongVM
                {
                    MaPhong = p.MaPhong,
                    TenPhong = p.TenPhong,
                    LoaiPhong = p.LoaiPhong.TenLoai,
                    Gia = p.LoaiPhong.Gia,
                    TrangThai = maPhongDangThue.Contains(p.MaPhong) ? TRANG_THAI_DANG_THUE : TRANG_THAI_TRONG
                })
                .ToList();

            BindGrid(ds);
        }

        //================ ĐẶT PHÒNG ====================
        private void btnDatPhong_Click(object sender, EventArgs e)
        {
            DateTime ngayNhan = dtpNgayNhan.Value.Date;
            DateTime ngayTra = dtpNgayTra.Value.Date;

            if (ngayTra <= ngayNhan)
            {
                MessageBox.Show("Ngày trả phòng phải lớn hơn ngày nhận phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. LẤY CÁC PHÒNG ĐƯỢC CHỌN TỪ GRID
            var phongDaChon = new List<(int MaPhong, string TenPhong)>();

            foreach (DataGridViewRow row in dgvPhong.Rows)
            {
                if (row.IsNewRow) continue;

                bool chon = row.Cells["Chon"].Value != null && Convert.ToBoolean(row.Cells["Chon"].Value);
                if (!chon) continue;

                int maPhong = Convert.ToInt32(row.Cells["MaPhong"].Value);
                string tenPhong = row.Cells["TenPhong"].Value?.ToString() ?? "";
                phongDaChon.Add((maPhong, tenPhong));
            }

            if (phongDaChon.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một phòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. KIỂM TRA TRÙNG LỊCH ĐẶT PHÒNG
            var maPhongIds = phongDaChon.Select(p => p.MaPhong).ToList();
            var dsMaPhongTrung = db.DatPhongs
                .Where(dp =>
                    maPhongIds.Contains(dp.MaPhong) &&
                    dp.TrangThai != TRANG_THAI_DA_HUY &&
                    dp.TrangThai != TRANG_THAI_DA_TRA &&
                    ngayNhan < dp.NgayTra &&
                    ngayTra > dp.NgayNhan)
                .Select(dp => dp.MaPhong)
                .Distinct()
                .ToList();

            // Báo lỗi đúng theo thông điệp yêu cầu nếu có phòng trùng
            if (dsMaPhongTrung.Count > 0)
            {
                var dsTenPhongTrung = phongDaChon
                    .Where(p => dsMaPhongTrung.Contains(p.MaPhong))
                    .Select(p => p.TenPhong);

                string danhSachTenPhong = string.Join(", ", dsTenPhongTrung);

                MessageBox.Show($"Phòng {danhSachTenPhong} đã có người đặt trước, vui lòng chọn phòng khác!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. XỬ LÝ VÀ KIỂM TRA THÔNG TIN KHÁCH HÀNG
            KhachHang kh;
            if (laKhachHang)
            {
                kh = khachDangDat;
            }
            else
            {
                string tenKH = txtTenKH.Text.Trim();
                string cccd = txtCCCD.Text.Trim();
                string sdt = txtSDT.Text.Trim();

                if (string.IsNullOrEmpty(tenKH) || string.IsNullOrEmpty(cccd) || string.IsNullOrEmpty(sdt))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra tên khách hàng (chỉ chữ cái và khoảng trắng)
                if (Regex.IsMatch(tenKH, @"\d"))
                {
                    MessageBox.Show("Tên khách hàng không hợp lệ! Tên không được chứa chữ số.",
                                    "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenKH.Focus();
                    return;
                }

                if (!Regex.IsMatch(tenKH, @"^[a-zA-Zàáảãạăắằẳẵặâấầẩẫậèéẻẽẹêếềểễệđìíỉĩịòóỏõọôốồổỗộơớờởỡợùúủũụưứừửữựỳýỷỹỵ\s]+$"))
                {
                    MessageBox.Show("Tên khách hàng không hợp lệ! Tên không được chứa ký tự đặc biệt.",
                                    "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenKH.Focus();
                    return;
                }

                // Kiểm tra CCCD (chỉ gồm chữ số)
                if (!Regex.IsMatch(cccd, @"^\d+$"))
                {
                    MessageBox.Show("Mã CCCD/CMND không hợp lệ! Chỉ được nhập số, không được chứa chữ cái hoặc ký tự đặc biệt.",
                                    "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCCCD.Focus();
                    return;
                }

                // Kiểm tra SĐT (đúng 10 số, bắt đầu bằng số 0)
                if (!Regex.IsMatch(sdt, @"^0\d{9}$"))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ! SĐT phải chứa đúng 10 chữ số và bắt đầu bằng số 0.",
                                    "Cảnh báo dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSDT.Focus();
                    return;
                }

                kh = db.KhachHangs.FirstOrDefault(x => x.CCCD == cccd);

                if (kh == null)
                {
                    kh = new KhachHang()
                    {
                        HoTen = tenKH,
                        CCCD = cccd,
                        SDT = sdt,
                        NgaySinh = new DateTime(2000, 1, 1),
                        GioiTinh = true,
                        TenDangNhap = null, 
                        MatKhau = null
                    };

                    db.KhachHangs.Add(kh);
                    db.SaveChanges();
                }
            }

            // 4. LƯU ĐẶT PHÒNG VÀO CSDL
            foreach (var (maPhong, _) in phongDaChon)
            {
                db.DatPhongs.Add(new DatPhong
                {
                    MaKH = kh.MaKH,
                    MaPhong = maPhong,
                    NgayDat = DateTime.Now,
                    NgayNhan = ngayNhan,
                    NgayTra = ngayTra,
                    TrangThai = TRANG_THAI_DA_DAT
                });
            }

            try
            {
                db.SaveChanges();
                MessageBox.Show("Đặt phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(LayThongDiepLoi(ex), "Lỗi lưu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadDGV();
        }

        private static string LayThongDiepLoi(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;

            return ex.Message;
        }

        private void dgvPhong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void frmDatPhong_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose();
        }
    }
}