using Microsoft.EntityFrameworkCore;
using PhanMemQuanLyKhachSan.Model;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmHoaDonKhach : Form
    {
        public DataContext db = new DataContext();

        private int maDatPhong;

        private decimal tongTienPhong = 0;
        private decimal tongTienDV = 0;

        public frmHoaDonKhach(int maDP)
        {
            InitializeComponent();
            maDatPhong = maDP;
        }

        private void frmHoaDonKhach_Load(object sender, EventArgs e)
        {
            // Nếu chưa có đơn đặt phòng (mã <= 0)
            if (maDatPhong <= 0)
            {
                dgvPhong.DataSource = null;
                dgvDV.DataSource = null;
                TinhTongTien();
                return;
            }

            LoadThongTinKhach();
            LoadPhong();
            LoadDichVu();
            TinhTongTien();
        }

        //================ THÔNG TIN KHÁCH ==================
        private void LoadThongTinKhach()
        {
            // Truy vấn thông tin đặt phòng kèm thông tin Khách Hàng
            var dp = db.DatPhongs
                       .Include(x => x.KhachHang)
                       .FirstOrDefault(x => x.MaDatPhong == maDatPhong);

            if (dp != null)
            {
                var kh = dp.KhachHang;

                // 1. Đọc an toàn các trường chuỗi từ KhachHang (tránh crash do NULL)
                string tenKH = kh != null ? (kh.HoTen ?? "N/A") : "N/A";
                string cccd = kh != null ? (kh.CCCD ?? "Chưa có") : "Chưa có";
                string sdt = kh != null ? (kh.SDT ?? "Chưa có") : "Chưa có";

                // 2. Gán dữ liệu lên Label
                lblTenKH.Text = tenKH;
                lblCCCD.Text = cccd;
                lblSDT.Text = sdt;

                // 3. Hiển thị ngày nhận / trả phòng (vì là DateTime nên gọi trực tiếp)
                lblNgayNhan.Text = dp.NgayNhan > DateTime.MinValue
                    ? dp.NgayNhan.ToString("dd/MM/yyyy HH:mm")
                    : "Chưa xác định";

                lblNgayTra.Text = dp.NgayTra > DateTime.MinValue
                    ? dp.NgayTra.ToString("dd/MM/yyyy HH:mm")
                    : "Chưa xác định";
            }
            else
            {
                MessageBox.Show("Không tìm thấy dữ liệu đặt phòng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //================ PHÒNG ==================
        private void LoadPhong()
        {
            var listDP = db.DatPhongs
                .Include(dp => dp.Phong)
                .ThenInclude(p => p.LoaiPhong)
                .Where(dp => dp.MaDatPhong == maDatPhong)
                .ToList();

            var ds = listDP.Select(dp =>
            {
                DateTime ngayNhan = dp.NgayNhan;
                DateTime ngayTra = dp.NgayTra;

                int soNgay = (ngayTra - ngayNhan).Days;
                if (soNgay <= 0) soNgay = 1;

                string tenPhong = (dp.Phong != null && !string.IsNullOrEmpty(dp.Phong.TenPhong))
                                    ? dp.Phong.TenPhong
                                    : "N/A";

                string loaiPhong = (dp.Phong != null && dp.Phong.LoaiPhong != null && !string.IsNullOrEmpty(dp.Phong.LoaiPhong.TenLoai))
                                    ? dp.Phong.LoaiPhong.TenLoai
                                    : "N/A";

                decimal donGia = (dp.Phong != null && dp.Phong.LoaiPhong != null)
                                    ? dp.Phong.LoaiPhong.Gia
                                    : 0m;

                return new
                {
                    MaPhong = dp.MaPhong,
                    TenPhong = tenPhong,
                    LoaiPhong = loaiPhong,
                    NgayNhan = ngayNhan,
                    NgayTra = ngayTra,
                    DonGia = donGia,
                    SoNgay = soNgay,
                    ThanhTien = donGia * soNgay
                };
            }).ToList();

            dgvPhong.DataSource = ds;
            tongTienPhong = ds.Sum(x => x.ThanhTien);

            if (ds.Count > 0)
            {
                dgvPhong.Columns["MaPhong"].HeaderText = "Mã";
                dgvPhong.Columns["TenPhong"].HeaderText = "Tên phòng";
                dgvPhong.Columns["LoaiPhong"].HeaderText = "Loại phòng";
                dgvPhong.Columns["NgayNhan"].HeaderText = "Ngày nhận";
                dgvPhong.Columns["NgayTra"].HeaderText = "Ngày trả";
                dgvPhong.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvPhong.Columns["SoNgay"].HeaderText = "Số đêm";
                dgvPhong.Columns["ThanhTien"].HeaderText = "Thành tiền";

                dgvPhong.Columns["NgayNhan"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPhong.Columns["NgayTra"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvPhong.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvPhong.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

                // Phân bổ tỷ lệ chiều rộng từng cột
                dgvPhong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvPhong.Columns["MaPhong"].FillWeight = 45;
                dgvPhong.Columns["TenPhong"].FillWeight = 100;
                dgvPhong.Columns["LoaiPhong"].FillWeight = 110;
                dgvPhong.Columns["NgayNhan"].FillWeight = 90;
                dgvPhong.Columns["NgayTra"].FillWeight = 90;
                dgvPhong.Columns["DonGia"].FillWeight = 90;
                dgvPhong.Columns["SoNgay"].FillWeight = 55;
                dgvPhong.Columns["ThanhTien"].FillWeight = 100;
            }

            FormatGridView(dgvPhong);
        }

        //================ DỊCH VỤ (ĐÃ FIX NULL) ==================
        private void LoadDichVu()
        {
            var listCTDV = db.ChiTietDichVus
                .Include(ct => ct.DichVu)
                .Where(ct => ct.MaDatPhong == maDatPhong)
                .ToList();

            var ds = listCTDV.Select(ct =>
            {
                string ngayStr = ct.NgaySuDung.HasValue ? ct.NgaySuDung.Value.ToString("dd/MM/yyyy") : "N/A";
                string gioStr = ct.NgaySuDung.HasValue ? ct.NgaySuDung.Value.ToString("HH:mm") : "N/A";

                string tenDV = (ct.DichVu != null && !string.IsNullOrEmpty(ct.DichVu.TenDV))
                                ? ct.DichVu.TenDV
                                : "Dịch vụ phòng";

                // Ép kiểu (decimal?) trước khi dùng ?? 0m
                decimal donGiaAnToan = ((decimal?)ct.DonGia) ?? 0m;
                decimal thanhTienAnToan = ((decimal?)ct.ThanhTien) ?? 0m;

                return new
                {
                    TenDV = tenDV,
                    NgaySuDung = ngayStr,
                    GioSuDung = gioStr,
                    DonGia = donGiaAnToan,
                    SoLuong = ct.SoLuong,
                    ThanhTien = thanhTienAnToan
                };
            }).ToList();

            dgvDV.DataSource = ds;
            tongTienDV = ds.Sum(x => x.ThanhTien);

            if (ds.Count > 0)
            {
                dgvDV.Columns["TenDV"].HeaderText = "Tên dịch vụ";
                dgvDV.Columns["NgaySuDung"].HeaderText = "Ngày dùng";
                dgvDV.Columns["GioSuDung"].HeaderText = "Giờ dùng";
                dgvDV.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvDV.Columns["SoLuong"].HeaderText = "Số lượng";
                dgvDV.Columns["ThanhTien"].HeaderText = "Thành tiền";

                dgvDV.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvDV.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";

                dgvDV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvDV.Columns["TenDV"].FillWeight = 160;
                dgvDV.Columns["NgaySuDung"].FillWeight = 85;
                dgvDV.Columns["GioSuDung"].FillWeight = 70;
                dgvDV.Columns["DonGia"].FillWeight = 90;
                dgvDV.Columns["SoLuong"].FillWeight = 60;
                dgvDV.Columns["ThanhTien"].FillWeight = 100;
            }

            FormatGridView(dgvDV);
        }

        //================ HÀM CHUẨN HÓA DẠNG BẢNG ==================
        private void FormatGridView(DataGridView dgv)
        {
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            // Màu nền và chữ cho Header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cho phép tự xuống dòng nếu tiêu đề dài
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Tăng chiều cao Header đủ cho 2 dòng chữ
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 45;

            dgv.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgv.GridColor = Color.Gray;

            // Định dạng hàng dữ liệu
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
        }

        //================ TÍNH TỔNG ==================
        private void TinhTongTien()
        {
            lblTP.Text = tongTienPhong.ToString("N0") + " đ";
            lblTDV.Text = tongTienDV.ToString("N0") + " đ";
            lblTienDV.Text = tongTienDV.ToString("N0") + " đ";

            decimal tong = tongTienPhong + tongTienDV;
            lblTong.Text = tong.ToString("N0") + " đ";
        }

        //================ THANH TOÁN ==================
        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (maDatPhong <= 0) return;

            try
            {
                // 1. Cập nhật trạng thái Đặt phòng
                var dp = db.DatPhongs.FirstOrDefault(x => x.MaDatPhong == maDatPhong);
                if (dp == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin đặt phòng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dp.TrangThai = "Đã thanh toán";

                // Lấy tổng tiền thực tế
                decimal tongTienThanhToan = 0;
                if (decimal.TryParse(lblTong.Text.Replace(",", "").Replace(".", "").Replace("đ", "").Replace("VNĐ", "").Trim(), out decimal val))
                {
                    tongTienThanhToan = val;
                }

                // Lấy thời điểm bấm nút thanh toán HIỆN TẠI
                DateTime thoiDiemThanhToan = DateTime.Now;

                // 2. Cập nhật hoặc Tạo mới Hóa đơn
                var hd = db.HoaDons.FirstOrDefault(x => x.MaDatPhong == maDatPhong);
                if (hd != null)
                {
                    // Trường hợp đã có hóa đơn -> Gán lại Ngày lập + Phương thức + Tổng tiền
                    hd.NgayLap = thoiDiemThanhToan;
                    hd.PhuongThucTT = "Chuyển khoản";
                    hd.TongTien = tongTienThanhToan;

                    // Bắt EF Core bắt buộc phải UPDATE cột NgayLap xuống SQL
                    db.Entry(hd).Property(x => x.NgayLap).IsModified = true;
                }
                else
                {
                    // Trường hợp chưa có hóa đơn -> Tạo mới
                    HoaDon hdMoi = new HoaDon
                    {
                        MaDatPhong = maDatPhong,
                        NgayLap = thoiDiemThanhToan,
                        TongTien = tongTienThanhToan,
                        PhuongThucTT = "Chuyển khoản"
                    };
                    db.HoaDons.Add(hdMoi);
                }

                // 3. Lưu xuống Database
                db.SaveChanges();

                MessageBox.Show($"Thanh toán thành công vào lúc {thoiDiemThanhToan:HH:mm dd/MM/yyyy}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnThanhToan.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu thông tin thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}