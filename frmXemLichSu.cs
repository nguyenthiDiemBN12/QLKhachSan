using Microsoft.EntityFrameworkCore;
using PhanMemQuanLyKhachSan.Model;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmXemLichSu : Form
    {
        public DataContext db = new DataContext();
        private NhanVien nhanVienDangNhap;
        private int? maKHSelected = null; // Lưu MaKH nếu được mở từ frmQLKhachHang

        // Constructor 1: Mở từ Menu / Nhân viên (Xem tất cả)
        public frmXemLichSu(NhanVien nv)
        {
            InitializeComponent();
            nhanVienDangNhap = nv;
        }

        // Constructor 2: Mở từ frmQLKhachHang (Lọc riêng cho 1 Khách hàng)
        public frmXemLichSu(int maKH)
        {
            InitializeComponent();
            maKHSelected = maKH;
        }

        //================ LOAD FORM ==================
        private void frmXemLichSu_Load(object sender, EventArgs e)
        {
            KhoiTaoForm();
        }

        private void frmXemLichSu_Load_1(object sender, EventArgs e)
        {
            KhoiTaoForm();
        }

        private void KhoiTaoForm()
        {
            cbTrangThai.Items.Clear();
            cbTrangThai.Items.Add("Tất cả");
            cbTrangThai.Items.Add("Đã đặt");
            cbTrangThai.Items.Add("Đang ở");
            cbTrangThai.Items.Add("Đã trả phòng");
            cbTrangThai.Items.Add("Đã hủy");
            cbTrangThai.Items.Add("Đã thanh toán");

            cbTrangThai.SelectedIndex = 0;
            dtpThoiGian.Value = DateTime.Today;
            dtpThoiGian.Checked = false;

            // Nạp dữ liệu
            LoadDGV();
        }

        //================ LOAD DANH SÁCH ==================
        private void LoadDGV()
        {
            FormatGridStyles();

            var query = db.DatPhongs.AsQueryable();

            // Nếu được mở cho riêng 1 khách hàng -> Lọc theo MaKH
            if (maKHSelected.HasValue)
            {
                query = query.Where(x => x.MaKH == maKHSelected.Value);
            }

            HienThiDanhSach(query);
        }

        //================ TÌM KIẾM ==================
        private void btnTim_Click_1(object sender, EventArgs e)
        {
            var query = db.DatPhongs.AsQueryable();

            // Nếu có truyền MaKH, giữ nguyên điều kiện lọc theo khách hàng này
            if (maKHSelected.HasValue)
            {
                query = query.Where(x => x.MaKH == maKHSelected.Value);
            }

            // Lọc theo Ngày đặt
            if (dtpThoiGian.Checked)
            {
                DateTime ngay = dtpThoiGian.Value.Date;
                query = query.Where(x => x.NgayDat.Date == ngay);
            }

            // Lọc theo Trạng thái (Xử lý đồng nhất giữa "Đang ở" và "Đang thuê")
            if (cbTrangThai.SelectedIndex > 0)
            {
                string tt = cbTrangThai.Text.Trim();
                if (tt == "Đang ở" || tt == "Đang thuê")
                {
                    query = query.Where(x => x.TrangThai == "Đang ở" || x.TrangThai == "Đang thuê");
                }
                else
                {
                    query = query.Where(x => x.TrangThai == tt);
                }
            }

            // Lọc theo Mã đặt phòng
            if (!string.IsNullOrWhiteSpace(txtMaDP.Text))
            {
                if (int.TryParse(txtMaDP.Text.Trim(), out int maDP))
                {
                    query = query.Where(x => x.MaDatPhong == maDP);
                }
                else
                {
                    MessageBox.Show("Mã đặt phòng phải là số nguyên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            FormatGridStyles();
            HienThiDanhSach(query);
        }

        //================ LÀM MỚI ==================
        private void btnLamMoi_Click_1(object sender, EventArgs e)
        {
            txtMaDP.Clear();
            cbTrangThai.SelectedIndex = 0;
            dtpThoiGian.Value = DateTime.Today;
            dtpThoiGian.Checked = false;

            LoadDGV();
        }

        //================ HÀM XỬ LÝ DỮ LIỆU & HIỂN THỊ ==================
        private void HienThiDanhSach(IQueryable<DatPhong> query)
        {
            var listDatPhong = query
                .Include(x => x.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .OrderByDescending(x => x.NgayDat)
                .ToList();

            var listMaDP = listDatPhong.Select(x => x.MaDatPhong).ToList();

            // LẤY DỮ LIỆU AN TOÀN TRÁNH CRASH "DATA IS NULL" TỪ SQL
            var dsChiTietDV = db.ChiTietDichVus
                .Include(x => x.DichVu)
                .Where(ct => listMaDP.Contains(ct.MaDatPhong))
                .Select(ct => new
                {
                    ct.MaDatPhong,
                    TenDV = ct.DichVu != null ? ct.DichVu.TenDV : "",
                    ThanhTien = ((decimal?)ct.ThanhTien) ?? 0m
                })
                .ToList();

            var dsHoaDon = db.HoaDons
                .Where(hd => listMaDP.Contains(hd.MaDatPhong))
                .ToList();

            var dsHienThi = listDatPhong.Select(x =>
            {
                DateTime ngayNhan = x.NgayNhan != default ? x.NgayNhan : DateTime.Today;
                DateTime ngayTra = x.NgayTra != default ? x.NgayTra : DateTime.Today;

                int soDem = (ngayTra - ngayNhan).Days;
                if (soDem <= 0) soDem = 1;

                decimal giaPhong = (x.Phong != null && x.Phong.LoaiPhong != null) ? x.Phong.LoaiPhong.Gia : 0m;
                decimal tienPhong = giaPhong * soDem;

                var listDV = dsChiTietDV.Where(ct => ct.MaDatPhong == x.MaDatPhong).ToList();
                string tenDichVu = listDV.Count > 0
                    ? string.Join(", ", listDV.Select(ct => ct.TenDV).Where(t => !string.IsNullOrEmpty(t)))
                    : "Không có";

                decimal tienDichVu = listDV.Sum(ct => ct.ThanhTien);
                decimal tongTien = tienPhong + tienDichVu;

                var hoaDon = dsHoaDon.FirstOrDefault(hd => hd.MaDatPhong == x.MaDatPhong);
                string pttt = "";

                if (hoaDon != null && !string.IsNullOrEmpty(hoaDon.PhuongThucTT))
                {
                    pttt = hoaDon.PhuongThucTT;
                }
                else if (x.TrangThai == "Đã thanh toán")
                {
                    pttt = "Chuyển khoản";
                }

                return new
                {
                    MaDatPhong = x.MaDatPhong,
                    Phong = x.Phong != null ? (x.Phong.TenPhong ?? "N/A") : "N/A",
                    LoaiPhong = (x.Phong != null && x.Phong.LoaiPhong != null) ? (x.Phong.LoaiPhong.TenLoai ?? "N/A") : "N/A",
                    NgayDat = x.NgayDat,
                    NgayNhan = ngayNhan,
                    NgayTra = ngayTra,
                    SoDem = soDem,
                    DonGia = giaPhong,
                    DichVu = tenDichVu,
                    TongTien = tongTien,
                    TrangThai = x.TrangThai ?? "Đã đặt",
                    PhuongThucTT = pttt
                };
            }).ToList();

            dgvLichSu.DataSource = dsHienThi;

            SetGridHeader();

            bool isSearching = txtMaDP.Text.Length > 0 || cbTrangThai.SelectedIndex > 0 || dtpThoiGian.Checked;
            if (dsHienThi.Count == 0 && isSearching)
            {
                MessageBox.Show("Không tìm thấy đơn đặt phòng hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //================ ĐỊNH DẠNG GIAO DIỆN GRIDVIEW ==================
        private void FormatGridStyles()
        {
            dgvLichSu.Columns.Clear();

            // BỎ CỘT TRỐNG MẶC ĐỊNH Ở ĐẦU BẢNG (ROW HEADERS)
            dgvLichSu.RowHeadersVisible = false;

            dgvLichSu.EnableHeadersVisualStyles = false;
            dgvLichSu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55);
            dgvLichSu.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;
            dgvLichSu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvLichSu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvLichSu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvLichSu.ColumnHeadersHeight = 35;

            dgvLichSu.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgvLichSu.GridColor = Color.Gray;

            dgvLichSu.RowsDefaultCellStyle.BackColor = Color.White;
            dgvLichSu.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvLichSu.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvLichSu.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
        }

        private void SetGridHeader()
        {
            if (dgvLichSu.Columns["MaDatPhong"] == null) return;

            dgvLichSu.Columns["MaDatPhong"].HeaderText = "Mã đặt";
            dgvLichSu.Columns["Phong"].HeaderText = "Phòng";
            dgvLichSu.Columns["LoaiPhong"].HeaderText = "Loại phòng";
            dgvLichSu.Columns["NgayDat"].HeaderText = "Ngày đặt";
            dgvLichSu.Columns["NgayNhan"].HeaderText = "Ngày nhận";
            dgvLichSu.Columns["NgayTra"].HeaderText = "Ngày trả";
            dgvLichSu.Columns["SoDem"].HeaderText = "Số đêm";
            dgvLichSu.Columns["DonGia"].HeaderText = "Đơn giá";
            dgvLichSu.Columns["DichVu"].HeaderText = "Dịch vụ";
            dgvLichSu.Columns["TongTien"].HeaderText = "Tổng tiền";
            dgvLichSu.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvLichSu.Columns["PhuongThucTT"].HeaderText = "Phương thức thanh toán";

            dgvLichSu.Columns["NgayDat"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvLichSu.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvLichSu.Columns["NgayTra"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvLichSu.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            dgvLichSu.Columns["TongTien"].DefaultCellStyle.Format = "N0";

            // CỘT 1: XEM CHI TIẾT
            if (dgvLichSu.Columns["btnXemChiTiet"] == null)
            {
                DataGridViewButtonColumn btnChiTiet = new DataGridViewButtonColumn();
                btnChiTiet.Name = "btnXemChiTiet";
                btnChiTiet.HeaderText = "Chi tiết";
                btnChiTiet.Text = "Xem chi tiết";
                btnChiTiet.UseColumnTextForButtonValue = true;
                btnChiTiet.FlatStyle = FlatStyle.Standard;

                dgvLichSu.Columns.Add(btnChiTiet);
            }

            // CỘT 2: HỦY PHÒNG
            if (dgvLichSu.Columns["btnHuyPhong"] == null)
            {
                DataGridViewButtonColumn btnHuy = new DataGridViewButtonColumn();
                btnHuy.Name = "btnHuyPhong";
                btnHuy.HeaderText = "Hủy phòng";
                btnHuy.Text = "Hủy";
                btnHuy.UseColumnTextForButtonValue = true;
                btnHuy.FlatStyle = FlatStyle.Standard;

                dgvLichSu.Columns.Add(btnHuy);
            }

            dgvLichSu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        //================ CLICK NÚT TRÊN CỘT THAO TÁC ==================
        private void dgvLichSu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var cellValue = dgvLichSu.Rows[e.RowIndex].Cells["MaDatPhong"].Value;
            if (cellValue == null || !int.TryParse(cellValue.ToString(), out int maDP)) return;

            string colName = dgvLichSu.Columns[e.ColumnIndex].Name;

            // 1. XỬ LÝ NÚT XEM CHI TIẾT
            if (colName == "btnXemChiTiet")
            {
                var trangThaiVal = dgvLichSu.Rows[e.RowIndex].Cells["TrangThai"].Value?.ToString();

                frmHoaDonKhach frm = new frmHoaDonKhach(maDP);

                if (trangThaiVal == "Đã thanh toán" || trangThaiVal == "Đang thuê" || trangThaiVal == "Đang ở" || trangThaiVal == "Đã trả phòng" || trangThaiVal == "Đã hủy")
                {
                    Control[] btns = frm.Controls.Find("btnThanhToan", true);
                    if (btns.Length > 0 && btns[0] is Button btn)
                    {
                        btn.Enabled = false;
                    }
                }

                frm.StartPosition = FormStartPosition.CenterScreen;

                // Mở Form hóa đơn chi tiết xong, nếu có thanh toán thì load lại DataGridView
                frm.ShowDialog();
                LoadDGV();
            }

            // 2. XỬ LÝ NÚT HỦY PHÒNG
            else if (colName == "btnHuyPhong")
            {
                XuLyHuyPhong(maDP, e.RowIndex);
            }
        }

        // HÀM XỬ LÝ HỦY PHÒNG VỚI QUY ĐỊNH 24H
        private void XuLyHuyPhong(int maDP, int rowIndex)
        {
            var datPhong = db.DatPhongs.FirstOrDefault(x => x.MaDatPhong == maDP);

            if (datPhong == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu đặt phòng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra nếu đơn đã ở các trạng thái không được hủy
            if (datPhong.TrangThai == "Đã hủy")
            {
                MessageBox.Show("Đơn đặt phòng này đã được hủy trước đó rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (datPhong.TrangThai == "Đã trả phòng")
            {
                MessageBox.Show("Đơn đặt phòng này đã hoàn tất, không thể hủy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // KIỂM TRA ĐIỀU KIỆN 24H TRƯỚC NGÀY NHẬN PHÒNG
            DateTime thoiGianHienTai = DateTime.Now;
            DateTime thoiGianNhanPhong = datPhong.NgayNhan;

            // Tính số giờ chênh lệch
            double soGioConLai = (thoiGianNhanPhong - thoiGianHienTai).TotalHours;

            if (soGioConLai < 24)
            {
                MessageBox.Show($"Không thể hủy phòng!\n\nTheo quy định, không hỗ trợ hủy phòng trong vòng 24 giờ trước thời điểm nhận phòng.\n(Ngày nhận: {thoiGianNhanPhong:dd/MM/yyyy})",
                                "Chặn hủy phòng", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // XÁC NHẬN HỦY
            DialogResult dialogResult = MessageBox.Show($"Bạn có chắc chắn muốn hủy đơn đặt phòng [Mã: {maDP}] không?",
                                                        "Xác nhận hủy phòng",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    datPhong.TrangThai = "Đã hủy";
                    db.SaveChanges();

                    MessageBox.Show("Hủy đặt phòng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại danh sách sau khi hủy
                    LoadDGV();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật trạng thái hủy: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}