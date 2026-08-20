using Microsoft.EntityFrameworkCore;
using PhanMemQuanLyKhachSan.Model;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmQLDatPhong : Form
    {
        public DataContext db = new DataContext();

        private NhanVien nhanVienHienTai;
        private int maDatPhong;

        // ================= BẢNG MÀU PASTEL SO LE =================
        private readonly Color colGreenLight = Color.FromArgb(232, 245, 233);
        private readonly Color colGreenDark = Color.FromArgb(165, 214, 167);

        private readonly Color colRedLight = Color.FromArgb(255, 235, 238);
        private readonly Color colRedDark = Color.FromArgb(239, 154, 154);

        private readonly Color colBlueLight = Color.FromArgb(227, 242, 253);
        private readonly Color colBlueDark = Color.FromArgb(144, 202, 249);

        private readonly Color colYellowLight = Color.FromArgb(255, 248, 225);
        private readonly Color colYellowDark = Color.FromArgb(255, 224, 130);

        private readonly Color colOrangeLight = Color.FromArgb(255, 243, 224);
        private readonly Color colOrangeDark = Color.FromArgb(255, 204, 128);

        public frmQLDatPhong()
        {
            InitializeComponent();
        }

        public frmQLDatPhong(int maDP)
        {
            InitializeComponent();
            this.maDatPhong = maDP;
        }

        public frmQLDatPhong(NhanVien nv)
        {
            InitializeComponent();
            this.nhanVienHienTai = nv;
        }

        private void frmQLDatPhong_Load(object sender, EventArgs e)
        {
            dgvLichSu.CellFormatting += dgvLichSu_CellFormatting;
            dgvLichSu.CellPainting += dgvLichSu_CellPainting; // Đổ màu nền ô nút bấm khi không dùng
            LoadDGV();
        }

        private void LoadDGV()
        {
            FormatGridStyles();

            // Refresh lại DbContext để đọc dữ liệu mới nhất từ CSDL
            db = new DataContext();

            var query = db.DatPhongs.AsQueryable();
            if (maDatPhong > 0)
            {
                query = query.Where(x => x.MaDatPhong == maDatPhong);
            }

            CapNhatThongKe();
            HienThiDanhSach(query);
        }

        // ================= 1. CẬP NHẬT THỐNG KÊ =================
        private void CapNhatThongKe()
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var listDP = db.DatPhongs.Include(x => x.Phong).ToList();

            int checkInTodayDaTT = listDP.Count(x => x.NgayNhan.Date == today && x.TrangThai == "Đã thanh toán");
            int checkInTodayChoTT = listDP.Count(x => x.NgayNhan.Date == today && (x.TrangThai == "Đã đặt" || x.TrangThai == "Chờ thanh toán"));

            int checkInTomorrowDaTT = listDP.Count(x => x.NgayNhan.Date == tomorrow && x.TrangThai == "Đã thanh toán");
            int checkInTomorrowChoTT = listDP.Count(x => x.NgayNhan.Date == tomorrow && (x.TrangThai == "Đã đặt" || x.TrangThai == "Chờ thanh toán"));

            int khachDuoi24h = listDP
                .Where(x => x.NgayNhan.Date == today && (x.TrangThai == "Đã đặt" || x.TrangThai == "Chờ thanh toán"))
                .Select(x => x.MaKH)
                .Distinct()
                .Count();

            int phongCanChuanBi = listDP.Count(x => x.NgayNhan.Date == tomorrow && x.TrangThai != "Đã hủy" && x.TrangThai != "Đã trả phòng");
            int sapCheckOut = listDP.Count(x => x.NgayTra.Date == today && x.TrangThai != "Đã hủy" && x.TrangThai != "Đã trả phòng");

            if (lblDaTTHN != null) lblDaTTHN.Text = checkInTodayDaTT.ToString();
            if (lblChuaTTHN != null) lblChuaTTHN.Text = checkInTodayChoTT.ToString();

            if (lblDaTTNM != null) lblDaTTNM.Text = checkInTomorrowDaTT.ToString();
            if (lblChuaTTNM != null) lblChuaTTNM.Text = checkInTomorrowChoTT.ToString();

            if (lblDenHanTT != null) lblDenHanTT.Text = khachDuoi24h.ToString();
            if (lblCanCB != null) lblCanCB.Text = phongCanChuanBi.ToString();
            if (lblSapOut != null) lblSapOut.Text = sapCheckOut.ToString();
        }

        // ================= 2. HIỂN THỊ DANH SÁCH =================
        private void HienThiDanhSach(IQueryable<DatPhong> query)
        {
            DateTime today = DateTime.Today;
            DateTime tomorrow = today.AddDays(1);

            var listDatPhong = query
                .Include(x => x.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .ToList();

            var dsChiTietDV = db.ChiTietDichVus.Include(x => x.DichVu).ToList();
            var dsHoaDon = db.HoaDons.ToList();

            var listSorted = listDatPhong.OrderBy(x =>
            {
                string tt = x.TrangThai ?? "";
                if (tt == "Đã hủy" || tt == "Đã trả phòng") return 10;

                DateTime nn = x.NgayNhan != default ? x.NgayNhan.Date : DateTime.MinValue;
                DateTime nt = x.NgayTra != default ? x.NgayTra.Date : DateTime.MinValue;

                if (nn == today)
                {
                    bool daTT = (tt == "Đã thanh toán");
                    return daTT ? 2 : 1;
                }

                if (nt == today) return 3;

                if (nn == tomorrow)
                {
                    bool daTT = (tt == "Đã thanh toán");
                    return daTT ? 5 : 4;
                }

                return 6;
            }).ThenByDescending(x => x.NgayDat).ToList();

            var dsHienThi = listSorted.Select(x =>
            {
                DateTime nn = x.NgayNhan != default ? x.NgayNhan : DateTime.Today;
                DateTime nt = x.NgayTra != default ? x.NgayTra : DateTime.Today;

                int soDem = (nt - nn).Days;
                if (soDem <= 0) soDem = 1;

                decimal giaPhong = (x.Phong != null && x.Phong.LoaiPhong != null) ? x.Phong.LoaiPhong.Gia : 0m;
                decimal tienPhong = giaPhong * soDem;

                var listDV = dsChiTietDV.Where(ct => ct.MaDatPhong == x.MaDatPhong).ToList();
                string tenDichVu = listDV.Count > 0
                    ? string.Join(", ", listDV.Select(ct => ct.DichVu != null ? (ct.DichVu.TenDV ?? "") : "").Where(t => !string.IsNullOrEmpty(t)))
                    : "Không có";

                decimal tienDichVu = listDV.Sum(ct => (decimal?)ct.ThanhTien ?? 0m);
                decimal tongTien = tienPhong + tienDichVu;

                var hoaDon = dsHoaDon.FirstOrDefault(hd => hd.MaDatPhong == x.MaDatPhong);
                string pttt = "";

                if (x.TrangThai != "Đã đặt" && x.TrangThai != "Đã hủy" && hoaDon != null)
                {
                    pttt = hoaDon.PhuongThucTT ?? "";
                }

                return new
                {
                    MaDatPhong = x.MaDatPhong,
                    Phong = x.Phong != null ? (x.Phong.TenPhong ?? "N/A") : "N/A",
                    LoaiPhong = (x.Phong != null && x.Phong.LoaiPhong != null) ? (x.Phong.LoaiPhong.TenLoai ?? "N/A") : "N/A",
                    NgayDat = x.NgayDat,
                    NgayNhan = nn,
                    NgayTra = nt,
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

            dgvLichSu.ClearSelection();
        }

        // ================= 3. TÔ MÀU VÀ XỬ LÝ SELECTION =================
        private void dgvLichSu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvLichSu.Rows[e.RowIndex];
            if (row.Cells["NgayNhan"].Value == null || row.Cells["NgayTra"].Value == null) return;

            string trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "";
            bool isEvenRow = (e.RowIndex % 2 == 0);

            Color targetColor = Color.White;

            // NẾU ĐÃ HỦY HOẶC ĐÃ TRẢ PHÒNG -> LOẠI BỎ MÀU NỔI BẬT (Dùng màu trắng / xám nhạt thường)
            if (trangThai == "Đã hủy" || trangThai == "Đã trả phòng")
            {
                targetColor = isEvenRow ? Color.White : Color.FromArgb(248, 249, 250);
                row.DefaultCellStyle.ForeColor = (trangThai == "Đã hủy") ? Color.Gray : Color.DimGray;
            }
            else
            {
                DateTime ngayNhan = Convert.ToDateTime(row.Cells["NgayNhan"].Value).Date;
                DateTime ngayTra = Convert.ToDateTime(row.Cells["NgayTra"].Value).Date;

                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                if (ngayNhan == today)
                {
                    targetColor = (trangThai == "Đã thanh toán")
                        ? (isEvenRow ? colGreenLight : colGreenDark)
                        : (isEvenRow ? colRedLight : colRedDark);
                }
                else if (ngayTra == today)
                {
                    targetColor = isEvenRow ? colBlueLight : colBlueDark;
                }
                else if (ngayNhan == tomorrow)
                {
                    targetColor = (trangThai == "Đã thanh toán")
                        ? (isEvenRow ? colYellowLight : colYellowDark)
                        : (isEvenRow ? colOrangeLight : colOrangeDark);
                }
                else
                {
                    targetColor = isEvenRow ? Color.White : Color.FromArgb(245, 245, 245);
                }

                row.DefaultCellStyle.ForeColor = Color.Black;
            }

            row.DefaultCellStyle.BackColor = targetColor;
            row.DefaultCellStyle.SelectionBackColor = targetColor;
            row.DefaultCellStyle.SelectionForeColor = row.DefaultCellStyle.ForeColor;
        }

        // Ẩn/xóa nút ở những ô không đủ điều kiện thao tác
        private void dgvLichSu_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string colName = dgvLichSu.Columns[e.ColumnIndex].Name;

                if (colName == "btnThanhToan" || colName == "btnNhanPhong" || colName == "btnTraPhong")
                {
                    string tt = dgvLichSu.Rows[e.RowIndex].Cells["TrangThai"].Value?.ToString() ?? "";

                    bool showButton = false;
                    if (colName == "btnThanhToan" && (tt == "Đã đặt" || tt == "Chờ thanh toán")) showButton = true;
                    if (colName == "btnNhanPhong" && tt == "Đã thanh toán") showButton = true;
                    if (colName == "btnTraPhong" && tt == "Đang thuê") showButton = true;

                    if (!showButton)
                    {
                        e.PaintBackground(e.CellBounds, true);
                        e.Handled = true; // Không vẽ nút
                    }
                }
            }
        }

        // ================= 4. THIẾT LẬP HEADER VÀ KHAI BÁO 3 CỘT NÚT =================
        private void FormatGridStyles()
        {
            dgvLichSu.RowHeadersVisible = false;
            dgvLichSu.EnableHeadersVisualStyles = false;

            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(212, 175, 55),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvLichSu.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvLichSu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvLichSu.ColumnHeadersHeight = 40;

            dgvLichSu.GridColor = Color.LightGray;
            dgvLichSu.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgvLichSu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
            dgvLichSu.Columns["PhuongThucTT"].HeaderText = "Phương thức TT";

            // 1. Cột Thanh toán
            if (dgvLichSu.Columns["btnThanhToan"] == null)
            {
                DataGridViewButtonColumn colTT = new DataGridViewButtonColumn();
                colTT.Name = "btnThanhToan";
                colTT.HeaderText = "Thanh toán";
                colTT.Text = "Thanh toán";
                colTT.UseColumnTextForButtonValue = true;
                colTT.FlatStyle = FlatStyle.Flat;
                dgvLichSu.Columns.Add(colTT);
            }

            // 2. Cột Nhận phòng
            if (dgvLichSu.Columns["btnNhanPhong"] == null)
            {
                DataGridViewButtonColumn colNhan = new DataGridViewButtonColumn();
                colNhan.Name = "btnNhanPhong";
                colNhan.HeaderText = "Nhận phòng";
                colNhan.Text = "Nhận phòng";
                colNhan.UseColumnTextForButtonValue = true;
                colNhan.FlatStyle = FlatStyle.Flat;
                dgvLichSu.Columns.Add(colNhan);
            }

            // 3. Cột Trả phòng
            if (dgvLichSu.Columns["btnTraPhong"] == null)
            {
                DataGridViewButtonColumn colTra = new DataGridViewButtonColumn();
                colTra.Name = "btnTraPhong";
                colTra.HeaderText = "Trả phòng";
                colTra.Text = "Trả phòng";
                colTra.UseColumnTextForButtonValue = true;
                colTra.FlatStyle = FlatStyle.Flat;
                dgvLichSu.Columns.Add(colTra);
            }

            foreach (DataGridViewColumn col in dgvLichSu.Columns)
            {
                col.HeaderCell.Style.BackColor = Color.FromArgb(15, 23, 42);
                col.HeaderCell.Style.ForeColor = Color.FromArgb(212, 175, 55);
            }

            dgvLichSu.Columns["NgayDat"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvLichSu.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvLichSu.Columns["NgayTra"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvLichSu.Columns["DonGia"].DefaultCellStyle.Format = "N0";
            dgvLichSu.Columns["TongTien"].DefaultCellStyle.Format = "N0";

            dgvLichSu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLichSu.Columns["MaDatPhong"].FillWeight = 45;
            dgvLichSu.Columns["Phong"].FillWeight = 60;
            dgvLichSu.Columns["LoaiPhong"].FillWeight = 85;
            dgvLichSu.Columns["NgayDat"].FillWeight = 70;
            dgvLichSu.Columns["NgayNhan"].FillWeight = 70;
            dgvLichSu.Columns["NgayTra"].FillWeight = 70;
            dgvLichSu.Columns["SoDem"].FillWeight = 40;
            dgvLichSu.Columns["DonGia"].FillWeight = 70;
            dgvLichSu.Columns["DichVu"].FillWeight = 100;
            dgvLichSu.Columns["TongTien"].FillWeight = 80;
            dgvLichSu.Columns["TrangThai"].FillWeight = 75;
            dgvLichSu.Columns["PhuongThucTT"].FillWeight = 80;

            dgvLichSu.Columns["btnThanhToan"].FillWeight = 75;
            dgvLichSu.Columns["btnNhanPhong"].FillWeight = 75;
            dgvLichSu.Columns["btnTraPhong"].FillWeight = 75;
        }

        // ================= 5. SỰ KIỆN CLICK CHUYỂN TRẠNG THÁI =================
        private void dgvLichSu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvLichSu.Columns[e.ColumnIndex].Name;
            var cellValue = dgvLichSu.Rows[e.RowIndex].Cells["MaDatPhong"].Value;
            var trangThaiVal = dgvLichSu.Rows[e.RowIndex].Cells["TrangThai"].Value?.ToString();

            if (cellValue == null || !int.TryParse(cellValue.ToString(), out int maDP)) return;

            // 1. THAO TÁC XÁC NHẬN THANH TOÁN (Chuyển Đã đặt -> Đã thanh toán)
            if (colName == "btnThanhToan")
            {
                if (trangThaiVal == "Đã đặt" || trangThaiVal == "Chờ thanh toán")
                {
                    DialogResult result = MessageBox.Show($"Xác nhận đã thu tiền/tiền cọc cho mã đặt {maDP}?", "Xác nhận Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        var dp = db.DatPhongs.FirstOrDefault(x => x.MaDatPhong == maDP);
                        if (dp != null)
                        {
                            dp.TrangThai = "Đã thanh toán";
                            db.SaveChanges();

                            MessageBox.Show("Xác nhận thanh toán thành công! Giờ bạn có thể bấm 'Nhận phòng'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDGV();
                        }
                    }
                }
                return;
            }

            // 2. THAO TÁC NHẬN PHÒNG (Chuyển Đã thanh toán -> Đang thuê)
            if (colName == "btnNhanPhong")
            {
                if (trangThaiVal == "Đã thanh toán")
                {
                    DialogResult result = MessageBox.Show($"Xác nhận làm thủ tục NHẬN PHÒNG cho mã đặt {maDP}?", "Xác nhận Check-in", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        var dp = db.DatPhongs.FirstOrDefault(x => x.MaDatPhong == maDP);
                        if (dp != null)
                        {
                            dp.TrangThai = "Đang thuê";
                            db.SaveChanges();

                            MessageBox.Show("Khách đã nhận phòng thành công! Trạng thái chuyển sang 'Đang thuê'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDGV();
                        }
                    }
                }
                return;
            }

            // 3. THAO TÁC TRẢ PHÒNG (Trực tiếp chuyển sang "Đã trả phòng")
            if (colName == "btnTraPhong")
            {
                if (trangThaiVal == "Đang thuê")
                {
                    DialogResult result = MessageBox.Show($"Xác nhận TRẢ PHÒNG cho mã đặt {maDP}?", "Xác nhận Check-out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        var dp = db.DatPhongs.FirstOrDefault(x => x.MaDatPhong == maDP);
                        if (dp != null)
                        {
                            dp.TrangThai = "Đã trả phòng";
                            db.SaveChanges();

                            MessageBox.Show($"Đã hoàn tất thủ tục trả phòng cho mã đặt {maDP}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Load lại để cập nhật màu sắc và vị trí hiển thị
                            LoadDGV();
                        }
                    }
                }
                return;
            }

            // 4. CLICK VÀO CÁC CỘT KHÁC NẾU MUỐN XEM CHI TIẾT HÓA ĐƠN
            frmHoaDonKhach frmDetail = new frmHoaDonKhach(maDP);

            if (trangThaiVal == "Đã thanh toán" || trangThaiVal == "Đang thuê" || trangThaiVal == "Đang ở" || trangThaiVal == "Đã trả phòng" || trangThaiVal == "Đã hủy")
            {
                Control[] btns = frmDetail.Controls.Find("btnThanhToan", true);
                if (btns.Length > 0 && btns[0] is Button btn)
                {
                    btn.Enabled = false;
                }
            }

            frmDetail.StartPosition = FormStartPosition.CenterScreen;
            frmDetail.ShowDialog();
        }
    }
}