using Microsoft.EntityFrameworkCore;
using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; // 💡 Đảm bảo đã import System.Drawing để dùng Graphics, Color, Font
using System.Linq;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmQLPhong : Form
    {
        private NhanVien nhanVienDangNhap;

        public frmQLPhong()
        {
            InitializeComponent();
        }

        public frmQLPhong(NhanVien nv) : this()
        {
            nhanVienDangNhap = nv;
        }

        private void frmQLPhong_Load(object sender, EventArgs e)
        {
            // ⚡ Tối ưu đồ họa chống giật lag
            this.DoubleBuffered = true;

            // 🔒 Khóa các ô nhập liệu chỉ cho phép đọc (Không cho phép chỉnh sửa Giá và Mô tả)
            txtMaPhong.ReadOnly = true;
            txtGiaPhong.ReadOnly = true;
            txtMoTa.ReadOnly = true;

            if (txtTrangThai != null)
            {
                txtTrangThai.ReadOnly = true;
            }

            // 🎨 TÍCH HỢP TRANG TRÍ DATAGRIDVIEW ĐẸP
            SetupDataGridViewStyle();

            LoadComboboxLoaiPhong();

            // 🔥 Cập nhật trạng thái thực tế & Load dữ liệu lên GridView
            CapNhatTrangThaiPhongThucTe();
            LoadDataPhong();
            LamMoiForm();
        }

        #region Helper Methods (Các hàm trợ giúp)

        // 🎨 Hàm tùy chỉnh giao diện DataGridView đẹp & chuyên nghiệp
        private void SetupDataGridViewStyle()
        {
            if (dgvPhong == null) return;

            // Cấu hình tính năng cơ bản
            dgvPhong.ReadOnly = true;
            dgvPhong.AllowUserToAddRows = false;
            dgvPhong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPhong.MultiSelect = false;
            dgvPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 1. TẮT visual styles mặc định của Windows (BẮT BUỘC để đổi màu Header)
            dgvPhong.EnableHeadersVisualStyles = false;

            // 2. Tùy chỉnh kiểu dáng thanh Header
            dgvPhong.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55); // Màu xám đậm kim loại
            dgvPhong.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;               // Chữ màu vàng nổi bật
            dgvPhong.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold); // Font chữ to & đậm
            dgvPhong.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa tiêu đề Header

            // 3. Tăng chiều cao thanh Header cho thoáng và chuyên nghiệp
            dgvPhong.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPhong.ColumnHeadersHeight = 35;

            // 4. Đường viền giữa các ô tiêu đề nổi rõ hơn
            dgvPhong.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgvPhong.GridColor = Color.Gray; // Màu đường lưới giữa các ô

            // 5. Đặt màu nền cho các dòng dữ liệu (Hiệu ứng dòng so le/Alternating)
            dgvPhong.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPhong.RowsDefaultCellStyle.ForeColor = Color.Black;

            dgvPhong.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Xám nhẹ sang trọng
            dgvPhong.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            // 6. Đổi màu khi chọn (Highlight) cho dịu mắt
            dgvPhong.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvPhong.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // Định dạng cột hiển thị cho DataGridView
        private void FormatGridViewColumns()
        {
            if (dgvPhong == null) return;

            if (dgvPhong.Columns["MaPhong"] != null) dgvPhong.Columns["MaPhong"].HeaderText = "Mã phòng";
            if (dgvPhong.Columns["TenPhong"] != null) dgvPhong.Columns["TenPhong"].HeaderText = "Tên phòng";
            if (dgvPhong.Columns["TenLoai"] != null) dgvPhong.Columns["TenLoai"].HeaderText = "Loại phòng";
            if (dgvPhong.Columns["Gia"] != null) dgvPhong.Columns["Gia"].HeaderText = "Giá phòng";
            if (dgvPhong.Columns["MoTa"] != null) dgvPhong.Columns["MoTa"].HeaderText = "Mô tả";
            if (dgvPhong.Columns["TrangThai"] != null) dgvPhong.Columns["TrangThai"].HeaderText = "Trạng thái";
            if (dgvPhong.Columns["NgayDen"] != null) dgvPhong.Columns["NgayDen"].HeaderText = "Ngày đến";
            if (dgvPhong.Columns["NgayDi"] != null) dgvPhong.Columns["NgayDi"].HeaderText = "Ngày đi";

            if (dgvPhong.Columns["MaLoai"] != null) dgvPhong.Columns["MaLoai"].Visible = false;
        }

        // Cập nhật trạng thái phòng thực tế dựa trên lịch đặt phòng hôm nay
        public void CapNhatTrangThaiPhongThucTe()
        {
            try
            {
                using (var db = new DataContext())
                {
                    DateTime homNay = DateTime.Now.Date;

                    var dsDatPhongHomNay = db.DatPhongs
                        .Where(dp => dp.NgayNhan.Date <= homNay
                                  && dp.NgayTra.Date >= homNay
                                  && dp.TrangThai != "Đã trả phòng"
                                  && dp.TrangThai != "Đã hủy")
                        .ToList();

                    var dsPhong = db.Phongs.ToList();

                    foreach (var p in dsPhong)
                    {
                        var datPhong = dsDatPhongHomNay.FirstOrDefault(dp => dp.MaPhong == p.MaPhong);

                        if (datPhong != null)
                        {
                            p.TrangThai = datPhong.TrangThai; // "Đang ở" hoặc "Đã đặt"
                        }
                        else if (p.TrangThai != "Bảo trì")
                        {
                            p.TrangThai = "Trống";
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi CapNhatTrangThaiPhongThucTe: {ex.Message}");
            }
        }

        #endregion

        #region Load Data & Events

        // Tải danh sách Loại Phòng lên các ComboBox
        private void LoadComboboxLoaiPhong()
        {
            try
            {
                using (var db = new DataContext())
                {
                    var listLoai = db.LoaiPhongs.AsNoTracking().ToList();

                    cboLoaiPhong.DataSource = listLoai;
                    cboLoaiPhong.DisplayMember = "TenLoai";
                    cboLoaiPhong.ValueMember = "MaLoai";

                    var listLoaiTimKiem = new List<LoaiPhong>(listLoai);
                    listLoaiTimKiem.Insert(0, new LoaiPhong { MaLoai = 0, TenLoai = "--- Tất cả ---" });

                    cboTimLoaiPhong.SelectedIndexChanged -= cboTimLoaiPhong_SelectedIndexChanged;
                    cboTimLoaiPhong.DataSource = listLoaiTimKiem;
                    cboTimLoaiPhong.DisplayMember = "TenLoai";
                    cboTimLoaiPhong.ValueMember = "MaLoai";
                    cboTimLoaiPhong.SelectedIndexChanged += cboTimLoaiPhong_SelectedIndexChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách loại phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tải toàn bộ danh sách phòng lên DataGridView
        public void LoadDataPhong()
        {
            try
            {
                using (var db = new DataContext())
                {
                    DateTime homNay = DateTime.Now.Date;

                    var dsDatPhongHomNay = db.DatPhongs
                        .AsNoTracking()
                        .Where(dp => dp.NgayNhan.Date <= homNay
                                  && dp.NgayTra.Date >= homNay
                                  && dp.TrangThai != "Đã trả phòng"
                                  && dp.TrangThai != "Đã hủy")
                        .ToList();

                    var dsPhong = db.Phongs
                        .AsNoTracking()
                        .Include(p => p.LoaiPhong)
                        .ToList();

                    var listPhong = dsPhong.Select(p =>
                    {
                        var datPhong = dsDatPhongHomNay.FirstOrDefault(dp => dp.MaPhong == p.MaPhong);

                        return new
                        {
                            p.MaPhong,
                            p.TenPhong,
                            TenLoai = p.LoaiPhong != null ? p.LoaiPhong.TenLoai : "",
                            Gia = p.LoaiPhong != null ? p.LoaiPhong.Gia.ToString("N0") + " VNĐ" : "0 VNĐ",
                            MoTa = p.LoaiPhong != null ? p.LoaiPhong.MoTa : "",
                            TrangThai = p.TrangThai,
                            NgayDen = datPhong != null ? datPhong.NgayNhan.ToString("dd/MM/yyyy") : "-",
                            NgayDi = datPhong != null ? datPhong.NgayTra.ToString("dd/MM/yyyy") : "-",
                            p.MaLoai
                        };
                    }).ToList();

                    if (dgvPhong != null)
                    {
                        dgvPhong.DataSource = listPhong;
                        FormatGridViewColumns();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvPhong_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPhong != null)
            {
                DataGridViewRow row = dgvPhong.Rows[e.RowIndex];

                txtMaPhong.Text = row.Cells["MaPhong"].Value?.ToString();
                txtTenPhong.Text = row.Cells["TenPhong"].Value?.ToString();

                if (row.Cells["MaLoai"].Value != null)
                {
                    cboLoaiPhong.SelectedValue = Convert.ToInt32(row.Cells["MaLoai"].Value);
                }

                if (row.Cells["TrangThai"].Value != null && txtTrangThai != null)
                {
                    txtTrangThai.Text = row.Cells["TrangThai"].Value.ToString();
                }
            }
        }

        // Tải thông tin Giá và Mô tả hiển thị (Chỉ đọc) khi chọn Loại phòng
        private void cboLoaiPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLoaiPhong.SelectedValue != null && int.TryParse(cboLoaiPhong.SelectedValue.ToString(), out int maLoai))
            {
                using (var db = new DataContext())
                {
                    var loaiPhong = db.LoaiPhongs.AsNoTracking().FirstOrDefault(l => l.MaLoai == maLoai);
                    if (loaiPhong != null)
                    {
                        txtGiaPhong.Text = loaiPhong.Gia.ToString("N0") + " VNĐ";
                        txtMoTa.Text = loaiPhong.MoTa;
                    }
                }
            }
        }

        private void cboTimLoaiPhong_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimTheoLoaiPhong();
        }

        #endregion

        #region CRUD Operations (Thêm, Sửa, Xóa, Làm mới)

        // 🎯 Nút THÊM PHÒNG
        private void btThem_Click(object sender, EventArgs e)
        {
            try
            {
                string tenPhong = txtTenPhong.Text.Trim();

                if (string.IsNullOrWhiteSpace(tenPhong))
                {
                    MessageBox.Show("Vui lòng nhập tên phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenPhong.Focus();
                    return;
                }

                if (cboLoaiPhong.SelectedValue == null || !int.TryParse(cboLoaiPhong.SelectedValue.ToString(), out int maLoai))
                {
                    MessageBox.Show("Vui lòng chọn loại phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var db = new DataContext())
                {
                    // Kiểm tra trùng tên phòng
                    bool tonTai = db.Phongs.Any(p => p.TenPhong.ToLower() == tenPhong.ToLower());
                    if (tonTai)
                    {
                        MessageBox.Show("Tên phòng đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTenPhong.Focus();
                        return;
                    }

                    // Thêm phòng mới (Không can thiệp bảng LoaiPhong)
                    Phong phongMoi = new Phong
                    {
                        TenPhong = tenPhong,
                        MaLoai = maLoai,
                        TrangThai = "Trống"
                    };

                    db.Phongs.Add(phongMoi);
                    db.SaveChanges();

                    txtMaPhong.Text = phongMoi.MaPhong.ToString();
                    MessageBox.Show("Thêm phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                CapNhatTrangThaiPhongThucTe();
                LoadDataPhong();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm phòng:\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🎯 Nút SỬA PHÒNG
        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMaPhong.Text, out int maPhong))
                {
                    MessageBox.Show("Vui lòng chọn phòng cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var db = new DataContext())
                {
                    var pSua = db.Phongs.Find(maPhong);
                    if (pSua != null)
                    {
                        pSua.TenPhong = txtTenPhong.Text.Trim();
                        int maLoaiMoi = Convert.ToInt32(cboLoaiPhong.SelectedValue);
                        pSua.MaLoai = maLoaiMoi;

                        if (txtTrangThai != null && !string.IsNullOrWhiteSpace(txtTrangThai.Text))
                        {
                            pSua.TrangThai = txtTrangThai.Text.Trim();
                        }

                        db.SaveChanges();
                        MessageBox.Show("Cập nhật thông tin phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CapNhatTrangThaiPhongThucTe();
                        LoadDataPhong();
                        LamMoiForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi sửa thông tin: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🎯 Nút XÓA PHÒNG
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMaPhong.Text, out int maPhong))
                {
                    MessageBox.Show("Vui lòng chọn phòng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Bạn có chắc chắn muốn xóa phòng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    using (var db = new DataContext())
                    {
                        var pXoa = db.Phongs.Find(maPhong);
                        if (pXoa != null)
                        {
                            db.Phongs.Remove(pXoa);
                            db.SaveChanges();
                            MessageBox.Show("Xóa phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            CapNhatTrangThaiPhongThucTe();
                            LoadDataPhong();
                            LamMoiForm();
                        }
                    }
                }
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Không thể xóa phòng này vì đang chứa lịch sử đặt phòng!", "Lỗi ràng buộc", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🎯 Nút LÀM MỚI
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoiForm();
            CapNhatTrangThaiPhongThucTe();
            LoadDataPhong();
        }

        private void LamMoiForm()
        {
            txtMaPhong.Clear();
            txtTenPhong.Clear();
            txtGiaPhong.Clear();
            txtMoTa.Clear();

            if (txtTrangThai != null)
            {
                txtTrangThai.Text = "Trống";
            }

            if (cboLoaiPhong.Items.Count > 0)
            {
                cboLoaiPhong.SelectedIndex = 0;
            }

            if (dgvPhong != null)
            {
                dgvPhong.ClearSelection();
            }

            txtTenPhong.Focus();
        }

        #endregion

        #region Search & Filter (Tìm kiếm & Tra cứu)

        // 🎯 Tìm theo loại phòng
        private void TimTheoLoaiPhong()
        {
            try
            {
                if (cboTimLoaiPhong.SelectedValue == null) return;

                if (!int.TryParse(cboTimLoaiPhong.SelectedValue.ToString(), out int maLoaiTim))
                    return;

                using (var db = new DataContext())
                {
                    DateTime homNay = DateTime.Now.Date;

                    var dsDatPhongHomNay = db.DatPhongs
                        .AsNoTracking()
                        .Where(dp => dp.NgayNhan.Date <= homNay
                                  && dp.NgayTra.Date >= homNay
                                  && dp.TrangThai != "Đã trả phòng"
                                  && dp.TrangThai != "Đã hủy")
                        .ToList();

                    var query = db.Phongs
                        .AsNoTracking()
                        .Include(p => p.LoaiPhong)
                        .AsQueryable();

                    if (maLoaiTim > 0)
                        query = query.Where(p => p.MaLoai == maLoaiTim);

                    var result = query.ToList().Select(p =>
                    {
                        var datPhong = dsDatPhongHomNay.FirstOrDefault(dp => dp.MaPhong == p.MaPhong);

                        return new
                        {
                            p.MaPhong,
                            p.TenPhong,
                            TenLoai = p.LoaiPhong != null ? p.LoaiPhong.TenLoai : "",
                            Gia = p.LoaiPhong != null ? p.LoaiPhong.Gia.ToString("N0") + " VNĐ" : "0 VNĐ",
                            MoTa = p.LoaiPhong != null ? p.LoaiPhong.MoTa : "",
                            TrangThai = p.TrangThai,
                            NgayDen = datPhong != null ? datPhong.NgayNhan.ToString("dd/MM/yyyy") : "-",
                            NgayDi = datPhong != null ? datPhong.NgayTra.ToString("dd/MM/yyyy") : "-",
                            p.MaLoai
                        };
                    }).ToList();

                    if (dgvPhong != null)
                    {
                        dgvPhong.DataSource = result;
                        FormatGridViewColumns();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🎯 Tra cứu tình trạng phòng theo khoảng thời gian
        private void btnTraCuu_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime tuNgay = dtpTuNgay.Value.Date;
                DateTime denNgay = dtpDenNgay.Value.Date;

                if (tuNgay > denNgay)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var db = new DataContext())
                {
                    var dsDatPhongLoc = db.DatPhongs
                        .AsNoTracking()
                        .Where(dp => dp.TrangThai != "Đã trả phòng"
                                  && dp.TrangThai != "Đã hủy"
                                  && dp.NgayNhan.Date < denNgay.AddDays(1)
                                  && dp.NgayTra.Date > tuNgay)
                        .OrderBy(dp => dp.NgayNhan)
                        .ToList();

                    var dsPhong = db.Phongs.AsNoTracking().Include(p => p.LoaiPhong).ToList();

                    var listResult = dsPhong.Select(p =>
                    {
                        var datPhong = dsDatPhongLoc.FirstOrDefault(dp => dp.MaPhong == p.MaPhong);

                        string trangThaiHienThi = p.TrangThai;
                        string ngayDen = "-";
                        string ngayDi = "-";

                        if (p.TrangThai == "Bảo trì")
                        {
                            trangThaiHienThi = "Bảo trì";
                        }
                        else if (datPhong != null)
                        {
                            trangThaiHienThi = datPhong.TrangThai;
                            ngayDen = datPhong.NgayNhan.ToString("dd/MM/yyyy");
                            ngayDi = datPhong.NgayTra.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            trangThaiHienThi = "Trống";
                        }

                        return new
                        {
                            p.MaPhong,
                            p.TenPhong,
                            TenLoai = p.LoaiPhong != null ? p.LoaiPhong.TenLoai : "",
                            Gia = p.LoaiPhong != null ? p.LoaiPhong.Gia.ToString("N0") + " VNĐ" : "0 VNĐ",
                            MoTa = p.LoaiPhong != null ? p.LoaiPhong.MoTa : "",
                            TrangThai = trangThaiHienThi,
                            NgayDen = ngayDen,
                            NgayDi = ngayDi,
                            p.MaLoai
                        };
                    }).ToList();

                    if (dgvPhong != null)
                    {
                        dgvPhong.DataSource = listResult;
                        FormatGridViewColumns();
                    }
                }

                MessageBox.Show($"Đã tra cứu tình trạng phòng từ {tuNgay:dd/MM/yyyy} đến {denNgay:dd/MM/yyyy}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tra cứu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void dgvPhong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

     
    }
}