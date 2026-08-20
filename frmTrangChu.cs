using Microsoft.EntityFrameworkCore;
using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PMQuanLyKhachSan
{
    public partial class frmTrangChu : Form
    {
        public DataContext db = new DataContext();

        private NhanVien nhanVienDangNhap;

        public frmTrangChu(NhanVien nv)
        {
            InitializeComponent();
            nhanVienDangNhap = nv;
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoadTongSoPhong()
        {
            DateTime homNay = DateTime.Now.Date;
            int soPhongDatTruocHomNay = db.DatPhongs
                .Where(dp => dp.TrangThai == "Đã thanh toán" && dp.NgayDat.Date == homNay)
                .Select(dp => dp.MaPhong)

                .Count();

            lblPhong.Text = soPhongDatTruocHomNay.ToString();
        }

        // 1. Chỉnh hàm LoadDoanhThu trả về giá trị decimal để tái sử dụng cho hàm tính tỷ lệ
        private decimal LoadDoanhThu()
        {
            decimal tongTienPhong = 0;
            decimal tongTienDV = 0;

            DateTime homNay = DateTime.Now.Date;

            // Tiền phòng hôm nay
            var dsPhong = (
                from dp in db.DatPhongs
                join p in db.Phongs on dp.MaPhong equals p.MaPhong
                join lp in db.LoaiPhongs on p.MaLoai equals lp.MaLoai
                where dp.NgayDat.Date == homNay
                   && (dp.TrangThai == "Đã thanh toán")
                select new
                {
                    lp.Gia,
                    dp.NgayNhan,
                    dp.NgayTra
                }).ToList();

            foreach (var item in dsPhong)
            {
                int soNgay = (item.NgayTra - item.NgayNhan).Days;
                if (soNgay <= 0) soNgay = 1;
                tongTienPhong += item.Gia * soNgay;
            }

            // Tiền dịch vụ hôm nay
            tongTienDV = db.ChiTietDichVus
                .Where(x =>
                    x.NgaySuDung.HasValue &&
                    x.NgaySuDung.Value.Date == homNay &&
                    (x.DatPhong.TrangThai == "Đã trả phòng"))
                .ToList()
                .Sum(x => x.ThanhTien);

            decimal tongDoanhThu = tongTienPhong + tongTienDV;
            lblDoanhThu.Text = tongDoanhThu.ToString("N0") + " đ";

            return tongDoanhThu; // Trả về tổng để dùng ở hàm dưới
        }

        // 2. Hàm LoadTyleSoVoiHomQua() cực kỳ gọn nhẹ
        private void LoadTyleSoVoiHomQua(decimal dtHomNay)
        {
            DateTime homQua = DateTime.Now.Date.AddDays(-1);

            // Tính doanh thu hôm qua
            decimal tienPhongHomQua = (
                from dp in db.DatPhongs
                join p in db.Phongs on dp.MaPhong equals p.MaPhong
                join lp in db.LoaiPhongs on p.MaLoai equals lp.MaLoai
                where dp.NgayDat.Date == homQua
                   && (dp.TrangThai == "Đã thanh toán")
                select new { lp.Gia, dp.NgayNhan, dp.NgayTra }
            ).ToList().Sum(x => Math.Max(1, (x.NgayTra - x.NgayNhan).Days) * x.Gia);

            decimal tienDVHomQua = db.ChiTietDichVus
                .Where(x => x.NgaySuDung.HasValue && x.NgaySuDung.Value.Date == homQua &&
                           (x.DatPhong.TrangThai == "Đã trả phòng"))
                .ToList().Sum(x => x.ThanhTien);

            decimal dtHomQua = tienPhongHomQua + tienDVHomQua;

            // So sánh và hiển thị
            if (dtHomQua == 0)
            {
                lblTang.Text = dtHomNay > 0 ? "+ 100% so với hôm qua" : "0% so với hôm qua";
                lblTang.ForeColor = dtHomNay > 0 ? Color.LightGreen : Color.Gray;
                return;
            }

            decimal phanTram = ((dtHomNay - dtHomQua)) / dtHomQua;

            if (phanTram > 0)
            {
                lblTang.Text = $"+ {phanTram:0.##}% so với hôm qua";
                lblTang.ForeColor = Color.LightGreen;
            }
            else if (phanTram < 0)
            {
                lblTang.Text = $"- {Math.Abs(phanTram):0.##}% so với hôm qua";
                lblTang.ForeColor = Color.Tomato;
            }
            else
            {
                lblTang.Text = "0% so với hôm qua";
                lblTang.ForeColor = Color.Gray;
            }
        }

        private void LoadKhachHomNay()
        {
            lblKhach.Text = db.DatPhongs
                  .Count(x => x.NgayNhan.Date == DateTime.Now.Date)
                  .ToString();
        }

        private void LoadDichVuHomNay()
        {
            lblDV.Text = db.ChiTietDichVus
               .Count(x =>
                    x.NgaySuDung.HasValue &&
                    x.NgaySuDung.Value.Date == DateTime.Now.Date)
               .ToString();
        }

        private void LoadChartDoanhThu()
        {
            chartDoanhThu.Series.Clear();

            ChartArea area = chartDoanhThu.ChartAreas[0];
            area.AxisX.Title = "";
            area.AxisY.Title = "";

            //========= Trục X =========
            area.AxisX.Interval = 1;
            area.AxisX.IsLabelAutoFit = false;
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            area.AxisX.LabelStyle.IsStaggered = false;

            //========= Trục Y =========
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 600000000;
            area.AxisY.Interval = 100000000;
            area.AxisY.LabelStyle.Format = "#,0,,M";

            area.AxisY.MajorGrid.LineColor = Color.Gray;
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            area.BackColor = Color.Transparent;

            Series s = new Series("DoanhThu");
            s.ChartType = SeriesChartType.Line;
            s.BorderWidth = 4;

            s.MarkerStyle = MarkerStyle.Circle;
            s.MarkerSize = 10;

            s.Color = Color.Gold;
            s.MarkerColor = Color.Gold;

            // Bắt buộc chuỗi dữ liệu sắp xếp theo chỉ mục từng điểm từ trái qua phải
            s.IsXValueIndexed = true;

            chartDoanhThu.Series.Add(s);

            DateTime now = DateTime.Now;

            for (int i = 6; i >= 0; i--)
            {
                DateTime thang = new DateTime(now.Year, now.Month, 1).AddMonths(-i);

                //---------------- Tiền phòng ----------------
                var dsPhong = (
                    from dp in db.DatPhongs
                    join phong in db.Phongs on dp.MaPhong equals phong.MaPhong
                    join lp in db.LoaiPhongs on phong.MaLoai equals lp.MaLoai
                    where (dp.TrangThai == "Đã thanh toán"
                        || dp.TrangThai == "Đã trả phòng")
                       && dp.NgayNhan.Month == thang.Month
                       && dp.NgayNhan.Year == thang.Year
                    select new
                    {
                        lp.Gia,
                        dp.NgayNhan,
                        dp.NgayTra
                    }
                ).ToList();

                decimal tienPhong = 0;
                foreach (var item in dsPhong)
                {
                    int songay = (item.NgayTra - item.NgayNhan).Days;
                    if (songay <= 0) songay = 1;
                    tienPhong += item.Gia * songay;
                }

                //---------------- Tiền dịch vụ ----------------
                var dsDV = db.ChiTietDichVus
                    .Join(db.DatPhongs,
                        ct => ct.MaDatPhong,
                        dp => dp.MaDatPhong,
                        (ct, dp) => new { ct, dp })
                    .ToList();

                decimal tienDV = dsDV
                    .Where(x =>
                        x.ct.NgaySuDung.HasValue &&
                        x.ct.NgaySuDung.Value.Month == thang.Month &&
                        x.ct.NgaySuDung.Value.Year == thang.Year &&
                        (x.dp.TrangThai == "Đã thanh toán"
                            || x.dp.TrangThai == "Đã trả phòng"
                        ))
                    .Sum(x => x.ct.ThanhTien);

                decimal tong = tienPhong + tienDV;

                // Gán trực tiếp Tên Tháng làm nhãn X và Doanh Thu làm Y
                s.Points.AddXY("Tháng " + thang.Month, (double)tong);
            }
        }

        private void LoadChartPhong()
        {
            chartPhong.Series.Clear();

            Series s = new Series();
            s.ChartType = SeriesChartType.Pie;
            chartPhong.Series.Add(s);

            int tongPhong = db.Phongs.Count();
            DateTime homNay = DateTime.Now.Date;

            // 1. ĐÃ ĐẶT: Đơn đã thanh toán/xác nhận VÀ ngày nhận phòng là HÔM NAY HOẶC TƯƠNG LAI
            int daDat = db.DatPhongs
                .Where(x => x.TrangThai == "Đã thanh toán" && x.NgayNhan.Date >= homNay)
                .Select(x => x.MaPhong)
                .Distinct()
                .Count();

            // 2. ĐANG THUÊ: Khách đang ở trong khách sạn
            int dangO = db.DatPhongs
                .Where(x => x.TrangThai == "Đang thuê")
                .Select(x => x.MaPhong)
                .Distinct()
                .Count();

            // 3. ĐANG DỌN: Các phòng vừa trả phòng trong ngày hôm nay
            int dangDon = db.DatPhongs
                .Where(x => x.TrangThai == "Đã trả phòng" && x.NgayTra.Date == homNay)
                .Select(x => x.MaPhong)
                .Distinct()
                .Count();

            // 4. TRỐNG = Tổng số phòng - (Đã đặt + Đang thuê + Đang dọn)
            int trong = tongPhong - daDat - dangO - dangDon;
            if (trong < 0) trong = 0;

            // --- VẼ BIỂU ĐỒ ---
            if (daDat > 0)
            {
                int idx = s.Points.AddXY("Đã đặt", daDat);
                s.Points[idx].Color = Color.Gold;
            }

            if (dangO > 0)
            {
                int idx = s.Points.AddXY("Đang thuê", dangO);
                s.Points[idx].Color = Color.OrangeRed;
            }

            if (dangDon > 0)
            {
                int idx = s.Points.AddXY("Đang dọn", dangDon);
                s.Points[idx].Color = Color.DeepSkyBlue;
            }

            if (trong > 0)
            {
                int idx = s.Points.AddXY("Trống", trong);
                s.Points[idx].Color = Color.ForestGreen;
            }
        }

        private void LoadDgvDatPhong()
        {
            DateTime homNay = DateTime.Now.Date;

            var dsDatPhong = (
                from dp in db.DatPhongs
                join kh in db.KhachHangs on dp.MaKH equals kh.MaKH
                join p in db.Phongs on dp.MaPhong equals p.MaPhong
                where dp.NgayNhan.Date == homNay // Lọc các phòng có ngày nhận là hôm nay
                select new
                {
                    MaDatPhong = dp.MaDatPhong,
                    TenKH = kh.HoTen,         // Đổi MaKH thành Tên khách hàng (hoặc kh.HoTen tùy thuộc tên cột trong bảng KhachHangs của bạn)
                    TenPhong = p.TenPhong,     // Đổi MaPhong thành Tên phòng
                    NgayNhan = dp.NgayNhan,
                    NgayTra = dp.NgayTra,
                    TrangThai = dp.TrangThai
                    // Đã bỏ NgayDat theo đúng yêu cầu
                }
            ).ToList();

            // Gán dữ liệu vào DataGridView
            dgvDatPhong.DataSource = dsDatPhong;

            // Đổi lại tiêu đề cột hiển thị trên DataGridView cho đẹp
            dgvDatPhong.Columns["MaDatPhong"].HeaderText = "Mã đặt phòng";
            dgvDatPhong.Columns["TenKH"].HeaderText = "Tên khách hàng";
            dgvDatPhong.Columns["TenPhong"].HeaderText = "Tên phòng";
            dgvDatPhong.Columns["NgayNhan"].HeaderText = "Ngày nhận";
            dgvDatPhong.Columns["NgayTra"].HeaderText = "Ngày trả";
            dgvDatPhong.Columns["TrangThai"].HeaderText = "Trạng thái";

            dgvDatPhong.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvDatPhong.Columns["NgayTra"].DefaultCellStyle.Format = "dd/MM/yyyy";

            // Ẩn hẳn cột tiêu đề dòng (cột màu trắng bên trái)
            dgvDatPhong.RowHeadersVisible = false;
        }

        // 1. Hàm phụ trợ đọc ảnh an toàn (đặt trong class frmTrangChu)
        private Image LoadImageFromFile(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(Application.StartupPath, "Images", fileName);
                if (System.IO.File.Exists(path))
                {
                    return Image.FromFile(path);
                }
            }
            catch
            {
                // Tránh crash nếu có lỗi đọc file
            }
            return null;
        }

        // 2. Hàm Load dữ liệu chính
        private void LoadHoatDongGanDay()
        {
            // Cấu hình giao diện bảng
            dgvHDMoi.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHDMoi.ColumnHeadersVisible = false;
            dgvHDMoi.RowHeadersVisible = false;

            if (dgvHDMoi.Columns.Count == 0)
            {
                DataGridViewImageColumn colIcon = new DataGridViewImageColumn
                {
                    Name = "colIcon",
                    HeaderText = "",
                    ImageLayout = DataGridViewImageCellLayout.Zoom,
                    Width = 42,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                dgvHDMoi.Columns.Add(colIcon);

                dgvHDMoi.Columns.Add("colNoiDung", "");
                dgvHDMoi.Columns["colNoiDung"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                dgvHDMoi.Columns.Add("colThoiGian", "");
                dgvHDMoi.Columns["colThoiGian"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvHDMoi.Columns["colThoiGian"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            dgvHDMoi.Rows.Clear();

            Image iconDatPhong = LoadImageFromFile("icon_bed.png");
            Image iconDichVu = LoadImageFromFile("icon_service.png");
            Image iconHoaDon = LoadImageFromFile("icon_card.png");
            Image iconTaiKhoan = LoadImageFromFile("icon_user.png");

            // TRUY VẤN LINQ
            var dsDatPhong = db.DatPhongs
                .AsEnumerable()
                .Select(dp => new
                {
                    LoaiHD = "DatPhong",
                    NoiDung = "Đặt phòng mới cho khách " + dp.KhachHang?.HoTen + " - Phòng " + dp.Phong?.TenPhong,
                    ThoiGian = (DateTime?)dp.NgayDat
                });

            var dsDichVu = db.ChiTietDichVus
                .Where(ct => ct.NgaySuDung.HasValue)
                .AsEnumerable()
                .Select(ct => new
                {
                    LoaiHD = "DichVu",
                    NoiDung = "Đặt dịch vụ " + ct.DichVu?.TenDV + " - Phòng " + ct.DatPhong?.Phong?.TenPhong,
                    ThoiGian = ct.NgaySuDung
                });

            var dsHoaDon = db.HoaDons
                .AsEnumerable()
                .Select(hd => new
                {
                    LoaiHD = "HoaDon",
                    // SỬA LỖI 1: Ép kiểu nullable hoặc dùng Null-coalescing ?? cho TongTien
                    NoiDung = "Thanh toán hóa đơn HD" + hd.MaHD + " - " + (hd.TongTien ?? 0).ToString("N0") + " đ",
                    ThoiGian = hd.NgayLap
                });

            // GỘP DỮ LIỆU & LẤY TOP 10
            var hoatDongMoiNhat = dsDatPhong
                .Concat(dsDichVu)
                .Concat(dsHoaDon)
                .OrderByDescending(x => x.ThoiGian)
                .Take(10)
                .ToList();

            // HIỂN THỊ
            foreach (var item in hoatDongMoiNhat)
            {
                Image iconTuongUng = iconDatPhong;

                switch (item.LoaiHD)
                {
                    case "DichVu":
                        iconTuongUng = iconDichVu;
                        break;
                    case "HoaDon":
                        iconTuongUng = iconHoaDon;
                        break;
                    case "TaiKhoan":
                        iconTuongUng = iconTaiKhoan;
                        break;
                }

                // SỬA LỖI 2: Đọc value từ nullable DateTime?
                string stringThoiGian = item.ThoiGian.HasValue ? item.ThoiGian.Value.ToString("HH:mm dd/MM/yyyy") : "";
                dgvHDMoi.Rows.Add(iconTuongUng, item.NoiDung, stringThoiGian);
            }
        }
        private void LoadDashboard()
        {
            LoadTongSoPhong();

            decimal dtHomNay = LoadDoanhThu();
            LoadTyleSoVoiHomQua(dtHomNay);

            LoadKhachHomNay();

            LoadDichVuHomNay();

            LoadChartDoanhThu();

            LoadChartPhong();

            LoadDgvDatPhong();

            LoadHoatDongGanDay();

            Color viyenVang = Color.FromArgb(240, 204, 119);
            int banKinhBoGoc = 15; // Bán kính bo góc (có thể đổi thành 10, 15, 20 tùy thích)
            int doDayVien = 2;     // Độ dày đường viền (px)

            // Tự động gán bo góc + kẻ viền cho TẤT CẢ Panel trên Form
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel panel)
                {
                    panel.Paint += (s, pe) => DrawRoundedPanel(panel, pe, banKinhBoGoc, viyenVang, doDayVien);
                    panel.Invalidate(); // Refresh lại panel để vẽ
                }
            }
        }

private void DrawRoundedPanel(Panel panel, PaintEventArgs e, int cornerRadius, Color borderColor, int borderThickness)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // Bật khử răng cưa cho góc bo tròn mịn

        Rectangle rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        int diameter = cornerRadius * 2;

        using (GraphicsPath path = new GraphicsPath())
        {
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            // Đặt vùng hiển thị của Panel theo đường bo góc (để controls bên trong không bị lòi ra góc)
            panel.Region = new Region(path);

            // Vẽ đường viền khung màu vàng (240, 204, 119)
            using (Pen pen = new Pen(borderColor, borderThickness))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, path);
            }
        }
    }

    private void frmTrangChu_Load(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void chartDoanhThu_Click(object sender, EventArgs e)
        {

        }

        private void lblSoPhong_Click(object sender, EventArgs e)
        {

        }
    }
}
