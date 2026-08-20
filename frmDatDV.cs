using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;

namespace PMQuanLyKhachSan
{
    public partial class frmDatDV : Form
    {
        public DataContext db = new DataContext();

        private KhachHang khachDangNhap;
        private NhanVien nhanVienDangNhap;

        private bool laKhachHang;

        // Model hiển thị dùng chung cho grid "phòng đã đặt", để 1 chỗ duy
        // nhất định nghĩa hình dạng dữ liệu, tránh lặp lại anonymous type
        // ở 2-3 nơi khác nhau.
        private class PhongDaDatVM
        {
            public int MaDatPhong { get; set; }
            public string HoTen { get; set; }
            public string TenPhong { get; set; }
            public string LoaiPhong { get; set; }
            public DateTime NgayNhan { get; set; }
            public DateTime NgayTra { get; set; }
            public string TrangThai { get; set; }
        }

        // Cấu hình nhãn + số lượng tối thiểu/tối đa theo từng loại dịch vụ.
        // Thay cho switch dài, dữ liệu hoá để dễ thêm/sửa dịch vụ sau này
        // mà không phải sửa logic code.
        private static readonly Dictionary<string, (string Label, int Min, int Max)> CauHinhDichVu =
            new Dictionary<string, (string Label, int Min, int Max)>
            {
                ["Phục vụ đồ ăn cho tiệc đêm"] = ("Số suất ăn", 1, 100),
                ["Thuê xe máy"] = ("Số lượng xe", 1, 20),
                ["Spa"] = ("Số người", 1, 20),
                ["Ăn sáng tại phòng"] = ("Số suất ăn", 1, 20),
            };

        //================ KHÁCH HÀNG ===================
        public frmDatDV(KhachHang kh)
        {
            InitializeComponent();

            khachDangNhap = kh;
            laKhachHang = true;

            this.FormClosed += frmDatDV_FormClosed;
        }

        //================ NHÂN VIÊN ====================
        public frmDatDV(NhanVien nv)
        {
            InitializeComponent();

            nhanVienDangNhap = nv;
            laKhachHang = false;

            this.FormClosed += frmDatDV_FormClosed;
        }

        private void frmDatDV_Load(object sender, EventArgs e)
        {
            txtMaDP.ReadOnly = true;

            dtpNgaySD.Value = DateTime.Now;
            dtpGioSD.Value = DateTime.Now;

            dtpGioSD.Format = DateTimePickerFormat.Time;
            dtpGioSD.ShowUpDown = true;

            numSL.Minimum = 1;
            numSL.Maximum = 100;
            numSL.Value = 1;

            LoadComboBox();

            LoadDGVDichVu();

            LoadDGVPhongDaDat();

            if (laKhachHang)
            {
                txtTenKH.Text = khachDangNhap.HoTen;
                txtTenKH.ReadOnly = true; // Khách hàng không được sửa tên của mình
            }
            else
            {
                txtTenKH.Clear();
                txtTenKH.ReadOnly = false; // Nhân viên có thể tự gõ tên để tìm kiếm

                txtTenKH.KeyDown -= txtTenKH_KeyDown;
                txtTenKH.KeyDown += txtTenKH_KeyDown;
            }

            dgvPhongDaDat.CellClick -= dgvPhongDaDat_CellClick;
            dgvPhongDaDat.CellClick += dgvPhongDaDat_CellClick;

            if (dgvDSDV.Rows.Count > 0)
            {
                dgvDSDV.Rows[0].Selected = true;
                dgvDSDV.CurrentCell = dgvDSDV.Rows[0].Cells["TenDV"];
            }
        }

        //=================================================
        // LOAD COMBOBOX DỊCH VỤ
        //=================================================
        private void LoadComboBox()
        {
            cbLoaiDV.DataSource = db.DichVus.ToList();
            cbLoaiDV.DisplayMember = "TenDV";
            cbLoaiDV.ValueMember = "MaDV";
        }

        //=================================================
        // LOAD DGV DỊCH VỤ
        //=================================================
        private void LoadDGVDichVu()
        {
            dgvDSDV.AutoGenerateColumns = false;
            dgvDSDV.Columns.Clear();
            dgvDSDV.DataSource = null;

            dgvDSDV.Columns.Add("MaDV", "Mã DV");
            dgvDSDV.Columns.Add("TenDV", "Tên dịch vụ");
            dgvDSDV.Columns.Add("Gia", "Đơn giá");

            dgvDSDV.Columns["MaDV"].DataPropertyName = "MaDV";
            dgvDSDV.Columns["TenDV"].DataPropertyName = "TenDV";
            dgvDSDV.Columns["Gia"].DataPropertyName = "Gia";

            dgvDSDV.DataSource = db.DichVus
                .Select(x => new { x.MaDV, x.TenDV, x.Gia })
                .ToList();

            dgvDSDV.Columns["Gia"].DefaultCellStyle.Format = "N0";
            dgvDSDV.RowHeadersVisible = false;

            // 1. TẮT visual styles mặc định của Windows (BẮT BUỘC để đổi màu Header)
            dgvDSDV.EnableHeadersVisualStyles = false;

            // 2. Tùy chỉnh kiểu dáng thanh Header
            dgvDSDV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55); // Màu xám đậm kim loại nổi bật
            dgvDSDV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;               // Chữ màu vàng giúp nổi bật
            dgvDSDV.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold); // Font chữ to & đậm
            dgvDSDV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa chữ trên Header

            // 3. Tăng chiều cao thanh Header cho thoáng và chuyên nghiệp
            dgvDSDV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDSDV.ColumnHeadersHeight = 35;

            // 4. Đường viền giữa các ô tiêu đề nổi rõ hơn
            dgvDSDV.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgvDSDV.GridColor = Color.Gray; // Màu đường lưới giữa các ô

            // Đặt màu nền cho tất cả các dòng mặc định (Dòng lẻ: Trắng)
            dgvDSDV.RowsDefaultCellStyle.BackColor = Color.White;
            dgvDSDV.RowsDefaultCellStyle.ForeColor = Color.Black; // Chữ màu đen

            // Đặt màu nền so le cho các dòng xen kẽ (Dòng chẵn: Xám nhạt)
            dgvDSDV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvDSDV.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
        }

        private List<PhongDaDatVM> LayDanhSachPhongDaDat(string tenTimKiem = null)
        {
            var query = db.DatPhongs
                .Include(x => x.KhachHang)
                .Include(x => x.Phong)
                .ThenInclude(x => x.LoaiPhong)
                .Where(x => x.NgayTra > DateTime.Now);

            if (laKhachHang)
            {
                query = query.Where(x => x.MaKH == khachDangNhap.MaKH);
            }

            if (!string.IsNullOrEmpty(tenTimKiem))
            {
                query = query.Where(x => x.KhachHang.HoTen.ToLower().Contains(tenTimKiem));
            }

            return query
                .Select(x => new PhongDaDatVM
                {
                    MaDatPhong = x.MaDatPhong,
                    HoTen = x.KhachHang.HoTen,
                    TenPhong = x.Phong.TenPhong,
                    LoaiPhong = x.Phong.LoaiPhong.TenLoai,
                    NgayNhan = x.NgayNhan,
                    NgayTra = x.NgayTra,
                    TrangThai = x.TrangThai
                })
                .ToList();
        }

        //=================================================
        // LOAD DGV PHÒNG ĐÃ ĐẶT
        //=================================================
        private void LoadDGVPhongDaDat()
        {
            dgvPhongDaDat.AutoGenerateColumns = false;
            dgvPhongDaDat.Columns.Clear();
            dgvPhongDaDat.DataSource = null;

            dgvPhongDaDat.Columns.Add("MaDatPhong", "Mã ĐP");
            dgvPhongDaDat.Columns.Add("HoTen", "Tên khách");
            dgvPhongDaDat.Columns.Add("TenPhong", "Phòng");
            dgvPhongDaDat.Columns.Add("LoaiPhong", "Loại phòng");
            dgvPhongDaDat.Columns.Add("NgayNhan", "Ngày nhận");
            dgvPhongDaDat.Columns.Add("NgayTra", "Ngày trả");
            dgvPhongDaDat.Columns.Add("TrangThai", "Trạng thái");

            dgvPhongDaDat.Columns["MaDatPhong"].DataPropertyName = "MaDatPhong";
            dgvPhongDaDat.Columns["HoTen"].DataPropertyName = "HoTen";
            dgvPhongDaDat.Columns["TenPhong"].DataPropertyName = "TenPhong";
            dgvPhongDaDat.Columns["LoaiPhong"].DataPropertyName = "LoaiPhong";
            dgvPhongDaDat.Columns["NgayNhan"].DataPropertyName = "NgayNhan";
            dgvPhongDaDat.Columns["NgayTra"].DataPropertyName = "NgayTra";
            dgvPhongDaDat.Columns["TrangThai"].DataPropertyName = "TrangThai";

            dgvPhongDaDat.DataSource = LayDanhSachPhongDaDat();

            dgvPhongDaDat.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPhongDaDat.Columns["NgayTra"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPhongDaDat.RowHeadersVisible = false;

            // 1. TẮT visual styles mặc định của Windows (BẮT BUỘC để đổi màu Header)
            dgvPhongDaDat.EnableHeadersVisualStyles = false;

            // 2. Tùy chỉnh kiểu dáng thanh Header
            dgvPhongDaDat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55); // Màu xám đậm kim loại nổi bật
            dgvPhongDaDat.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;               // Chữ màu vàng giúp nổi bật
            dgvPhongDaDat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold); // Font chữ to & đậm
            dgvPhongDaDat.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa chữ trên Header

            // 3. Tăng chiều cao thanh Header cho thoáng và chuyên nghiệp
            dgvPhongDaDat.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPhongDaDat.ColumnHeadersHeight = 35;

            // 4. Đường viền giữa các ô tiêu đề nổi rõ hơn
            dgvPhongDaDat.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgvPhongDaDat.GridColor = Color.Gray; // Màu đường lưới giữa các ô

            // Đặt màu nền cho tất cả các dòng mặc định (Dòng lẻ: Trắng)
            dgvPhongDaDat.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPhongDaDat.RowsDefaultCellStyle.ForeColor = Color.Black; // Chữ màu đen

            // Đặt màu nền so le cho các dòng xen kẽ (Dòng chẵn: Xám nhạt)
            dgvPhongDaDat.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPhongDaDat.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
        }

        // LỌC THÔNG TIN PHÒNG ĐÃ ĐẶT CHO NHÂN VIÊN KHI ẤN ENTER
        private void txtTenKH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true; // Chặn tiếng bíp khó chịu từ hệ thống khi nhấn Enter

            string tenTimKiem = txtTenKH.Text.Trim().ToLower();
            dgvPhongDaDat.DataSource = LayDanhSachPhongDaDat(tenTimKiem);
        }

        // CLICK TRỰC TIẾP DÒNG TRÊN BẢNG ĐỂ CHỌN PHÒNG
        private void dgvPhongDaDat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            txtMaDP.Text = dgvPhongDaDat.Rows[e.RowIndex].Cells["MaDatPhong"].Value.ToString();
            txtTenKH.Text = dgvPhongDaDat.Rows[e.RowIndex].Cells["HoTen"].Value.ToString();
        }

        private void dgvPhongDaDat_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Để trống để giữ nguyên cấu trúc thiết kế form cũ của bạn
        }

        private void dgvDSDV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int maDV = Convert.ToInt32(dgvDSDV.Rows[e.RowIndex].Cells["MaDV"].Value);
            cbLoaiDV.SelectedValue = maDV;
        }

        private void cbLoaiDV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cbLoaiDV.SelectedItem is DichVu dv))
                return;

            // Đồng bộ chọn trên DataGridView
            foreach (DataGridViewRow row in dgvDSDV.Rows)
            {
                if (Convert.ToInt32(row.Cells["MaDV"].Value) == dv.MaDV)
                {
                    row.Selected = true;
                    dgvDSDV.CurrentCell = row.Cells["TenDV"];
                    break;
                }
            }

            if (CauHinhDichVu.TryGetValue(dv.TenDV, out var cauHinh))
            {
                lblSL.Text = cauHinh.Label;
                numSL.Enabled = true;
                numSL.Minimum = cauHinh.Min;
                numSL.Maximum = cauHinh.Max;
            }
            else
            {
                lblSL.Text = "Số lượng";
                numSL.Enabled = false;
                numSL.Minimum = 1;
                numSL.Maximum = 20;
            }
        }

        private void btnDatDV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaDP.Text))
            {
                MessageBox.Show("Vui lòng chọn một phòng đã đặt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(cbLoaiDV.SelectedItem is DichVu dv))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maDP = Convert.ToInt32(txtMaDP.Text);
            var datPhongInfo = db.DatPhongs.FirstOrDefault(x => x.MaDatPhong == maDP);

            if (datPhongInfo == null)
            {
                MessageBox.Show("Không tìm thấy thông tin đặt phòng tương ứng!", "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime ngaySD = dtpNgaySD.Value.Date;
            DateTime ngayNhan = datPhongInfo.NgayNhan.Date;
            DateTime ngayTra = datPhongInfo.NgayTra.Date;

            if (ngaySD < ngayNhan || ngaySD > ngayTra)
            {
                MessageBox.Show($"Ngày sử dụng dịch vụ không hợp lệ!\n" +
                                $"Thời gian sử dụng phải nằm trong khoảng thuê phòng " +
                                $"(từ {ngayNhan:dd/MM/yyyy} đến {ngayTra:dd/MM/yyyy}).",
                                "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Ghép Ngày + Giờ sử dụng
                DateTime ngayGioSuDung = dtpNgaySD.Value.Date + dtpGioSD.Value.TimeOfDay;

                ChiTietDichVu ct = new ChiTietDichVu
                {
                    MaDatPhong = maDP,
                    MaDV = dv.MaDV,
                    SoLuong = (int)numSL.Value,
                    DonGia = dv.Gia,
                    NgaySuDung = ngayGioSuDung // Đã bổ sung chuẩn theo Database
                };

                ct.ThanhTien = ct.SoLuong * ct.DonGia;

                db.ChiTietDichVus.Add(ct);
                db.SaveChanges();

                MessageBox.Show("Đặt dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset dữ liệu sau khi lưu thành công
                txtMaDP.Clear();
                if (!laKhachHang) txtTenKH.Clear();
                cbLoaiDV.SelectedIndex = 0;
                dtpNgaySD.Value = DateTime.Now;
                dtpGioSD.Value = DateTime.Now;
                numSL.Value = 1;
                dgvPhongDaDat.ClearSelection();
                dgvDSDV.ClearSelection();
            }
            catch (Exception ex)
            {
                string innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Lỗi khi lưu vào Database:\n{innerError}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDSDV_SelectionChanged(object sender, EventArgs e)
        {
            // Đã xóa bỏ toàn bộ gán txtMaDP và txtTenKH tự động tại đây để tránh lỗi đồng bộ dữ liệu
        }

        private void frmDatDV_FormClosed(object sender, FormClosedEventArgs e)
        {
            db?.Dispose();
        }
    }
}