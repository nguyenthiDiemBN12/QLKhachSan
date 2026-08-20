using PhanMemQuanLyKhachSan.Model;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmQLKhachHang : Form
    {
        private readonly DataContext db = new DataContext();
        private readonly NhanVien nhanVienDangNhap;
        private int maKHChon = -1;

        #region Win32 API Placeholders
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            if (txt != null && txt.IsHandleCreated)
            {
                SendMessage(txt.Handle, EM_SETCUEBANNER, 0, placeholder);
            }
        }
        #endregion

        public frmQLKhachHang(NhanVien nv)
        {
            InitializeComponent();
            this.nhanVienDangNhap = nv;

            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgvKhachHang, new object[] { true });
        }

        private void frmQLKhachHang_Load(object sender, EventArgs e)
        {
            try
            {
                SetPlaceholder(txtHoTen, "Nhập họ tên khách hàng...");
                SetPlaceholder(txtCCCD, "Nhập số CCCD...");
                SetPlaceholder(txtSDT, "Nhập số điện thoại...");

                cboTrangThai.Items.Clear();
                cboTrangThai.Items.AddRange(new string[] {
                    "Tất cả",
                    "Đang lưu trú",
                    "Chưa nhận phòng",
                    "Đã trả phòng",
                    "Đã hủy"
                });
                cboTrangThai.SelectedIndex = 0;

                SetupDataGridViewStyle();
                LoadDataKhachHang();
                CapNhatThongKe();

                cboTrangThai.SelectedIndexChanged += cboTrangThai_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridViewStyle()
        {
            dgvKhachHang.EnableHeadersVisualStyles = false;
            dgvKhachHang.BorderStyle = BorderStyle.None;
            dgvKhachHang.BackgroundColor = Color.FromArgb(20, 30, 45);

            dgvKhachHang.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvKhachHang.ColumnHeadersHeight = 35;
            dgvKhachHang.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 35, 42);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 215, 0);
            dgvKhachHang.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            dgvKhachHang.RowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvKhachHang.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            dgvKhachHang.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvKhachHang.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgvKhachHang.RowTemplate.Height = 28;

            dgvKhachHang.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvKhachHang.GridColor = Color.FromArgb(200, 200, 200);

            dgvKhachHang.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvKhachHang.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // ================= TRUY VẤN DỮ LIỆU BẢNG =================
        private IQueryable<dynamic> GetKhachHangQuery(IQueryable<KhachHang> baseQuery, string filterTrangThai = "Tất cả")
        {
            DateTime now = DateTime.Now;

            return baseQuery.Select(k => new
            {
                k.MaKH,
                k.HoTen,
                k.CCCD,
                k.SDT,
                GioiTinh = k.GioiTinh.HasValue ? (k.GioiTinh.Value ? "Nam" : "Nữ") : "N/A",
                SoLanO = db.DatPhongs.Count(dp => dp.MaKH == k.MaKH && dp.TrangThai != "Đã hủy"),
                TongChiTieu = db.HoaDons
                    .Where(hd => db.DatPhongs.Any(dp => dp.MaDatPhong == hd.MaDatPhong && dp.MaKH == k.MaKH))
                    .Sum(hd => (decimal?)hd.TongTien) ?? 0m,

                // TÍNH TRẠNG THÁI THEO DÚNG CHUẨN THỜI GIAN THỰC & BỔ SUNG TRẠNG THÁI "ĐÃ HỦY"
                TrangThai = filterTrangThai == "Chưa nhận phòng" ? "Chưa nhận phòng" :
                            filterTrangThai == "Đang lưu trú" ? "Đang lưu trú" :
                            filterTrangThai == "Đã trả phòng" ? "Đã trả phòng" :
                            filterTrangThai == "Đã hủy" ? "Đã hủy" :

                            // 1. Đang lưu trú
                            (db.DatPhongs.Any(dp => dp.MaKH == k.MaKH &&
                                                    dp.TrangThai != "Đã hủy" &&
                                                    (dp.TrangThai == "Đang thuê" || dp.TrangThai == "Đang ở" || (dp.NgayNhan <= now && now <= dp.NgayTra)))
                                ? "Đang lưu trú"

                                // 2. Chưa nhận phòng
                                : db.DatPhongs.Any(dp => dp.MaKH == k.MaKH &&
                                                         dp.TrangThai != "Đã hủy" &&
                                                         dp.TrangThai != "Đã trả phòng" &&
                                                         dp.NgayNhan > now)
                                    ? "Chưa nhận phòng"

                                    // 3. Đã hủy (Nếu tất cả các đơn đặt phòng của khách này đều có trạng thái "Đã hủy")
                                    : (db.DatPhongs.Any(dp => dp.MaKH == k.MaKH) && db.DatPhongs.Where(dp => dp.MaKH == k.MaKH).All(dp => dp.TrangThai == "Đã hủy"))
                                        ? "Đã hủy"

                                        // 4. Đã trả phòng / Mặc định
                                        : "Đã trả phòng"),

                TaiKhoan = (string.IsNullOrEmpty(k.TenDangNhap) || k.TenDangNhap.StartsWith("khach_")) ? "Chưa có tài khoản" : k.TenDangNhap
            });
        }

        private void LoadDataKhachHang()
        {
            dgvKhachHang.DataSource = GetKhachHangQuery(db.KhachHangs, "Tất cả").ToList();
            SetupColumnsGridView();
        }

        private void SetupColumnsGridView()
        {
            if (dgvKhachHang.Columns["MaKH"] == null) return;

            dgvKhachHang.Columns["MaKH"].HeaderText = "Mã KH";
            dgvKhachHang.Columns["HoTen"].HeaderText = "Họ tên";
            dgvKhachHang.Columns["CCCD"].HeaderText = "CCCD";
            dgvKhachHang.Columns["SDT"].HeaderText = "SĐT";
            dgvKhachHang.Columns["GioiTinh"].HeaderText = "Giới tính";
            dgvKhachHang.Columns["SoLanO"].HeaderText = "Số lần ở";
            dgvKhachHang.Columns["TongChiTieu"].HeaderText = "Tổng chi tiêu";
            dgvKhachHang.Columns["TrangThai"].HeaderText = "Trạng thái";
            dgvKhachHang.Columns["TaiKhoan"].HeaderText = "Tài khoản";

            dgvKhachHang.Columns["TongChiTieu"].DefaultCellStyle.Format = "N0";

            if (dgvKhachHang.Columns["btnLichSu"] == null)
            {
                DataGridViewButtonColumn btnLichSu = new DataGridViewButtonColumn
                {
                    Name = "btnLichSu",
                    HeaderText = "Thao tác",
                    Text = "⏱ Lịch sử",
                    UseColumnTextForButtonValue = true,
                    FlatStyle = FlatStyle.Flat
                };
                dgvKhachHang.Columns.Add(btnLichSu);
            }

            dgvKhachHang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ================= LỌC TRẠNG THÁI =================
        private void cboTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            string trangThaiChon = cboTrangThai.SelectedItem?.ToString();
            var query = db.KhachHangs.AsQueryable();
            DateTime now = DateTime.Now;

            if (trangThaiChon == "Đang lưu trú")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH &&
                                                               dp.TrangThai != "Đã hủy" &&
                                                               (dp.TrangThai == "Đang thuê" || dp.TrangThai == "Đang ở" || (dp.NgayNhan <= now && now <= dp.NgayTra))));
            }
            else if (trangThaiChon == "Chưa nhận phòng")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH &&
                                                               dp.TrangThai != "Đã hủy" &&
                                                               dp.TrangThai != "Đã trả phòng" &&
                                                               dp.NgayNhan > now));
            }
            else if (trangThaiChon == "Đã trả phòng")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH && dp.TrangThai == "Đã trả phòng"));
            }
            else if (trangThaiChon == "Đã hủy")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH) &&
                                         db.DatPhongs.Where(dp => dp.MaKH == k.MaKH).All(dp => dp.TrangThai == "Đã hủy"));
            }

            dgvKhachHang.DataSource = GetKhachHangQuery(query, trangThaiChon).ToList();
            SetupColumnsGridView();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string hoTen = txtHoTen.Text.Trim().ToLower();
            string cccd = txtCCCD.Text.Trim();
            string sdt = txtSDT.Text.Trim();

            var query = db.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(hoTen))
                query = query.Where(k => k.HoTen.ToLower().Contains(hoTen));
            if (!string.IsNullOrEmpty(cccd))
                query = query.Where(k => k.CCCD.Contains(cccd));
            if (!string.IsNullOrEmpty(sdt))
                query = query.Where(k => k.SDT.Contains(sdt));

            string trangThaiChon = cboTrangThai.SelectedItem?.ToString() ?? "Tất cả";
            DateTime now = DateTime.Now;

            if (trangThaiChon == "Đang lưu trú")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH &&
                                                               dp.TrangThai != "Đã hủy" &&
                                                               (dp.TrangThai == "Đang thuê" || dp.TrangThai == "Đang ở" || (dp.NgayNhan <= now && now <= dp.NgayTra))));
            }
            else if (trangThaiChon == "Chưa nhận phòng")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH &&
                                                               dp.TrangThai != "Đã hủy" &&
                                                               dp.TrangThai != "Đã trả phòng" &&
                                                               dp.NgayNhan > now));
            }
            else if (trangThaiChon == "Đã trả phòng")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH && dp.TrangThai == "Đã trả phòng"));
            }
            else if (trangThaiChon == "Đã hủy")
            {
                query = query.Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH) &&
                                         db.DatPhongs.Where(dp => dp.MaKH == k.MaKH).All(dp => dp.TrangThai == "Đã hủy"));
            }

            dgvKhachHang.DataSource = GetKhachHangQuery(query, trangThaiChon).ToList();
            SetupColumnsGridView();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (maKHChon == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng từ bảng để chỉnh sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var kh = db.KhachHangs.FirstOrDefault(k => k.MaKH == maKHChon);
                if (kh != null)
                {
                    kh.HoTen = txtHoTen.Text.Trim();
                    kh.CCCD = txtCCCD.Text.Trim();
                    kh.SDT = txtSDT.Text.Trim();

                    db.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnLamMoi_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (maKHChon == -1)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần xóa từ bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chỉ chặn xóa nếu có các đơn đặt phòng THÀNH CÔNG (khác "Đã hủy")
            bool hasValidBooking = db.DatPhongs.Any(dp => dp.MaKH == maKHChon && dp.TrangThai != "Đã hủy");
            if (hasValidBooking)
            {
                MessageBox.Show("Khách hàng này đã có lịch sử giao dịch/đặt phòng, không thể xóa để đảm bảo toàn vẹn dữ liệu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa thông tin khách hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var kh = db.KhachHangs.FirstOrDefault(k => k.MaKH == maKHChon);
                    if (kh != null)
                    {
                        // Dọn dẹp các đơn bị hủy liên quan (nếu có) trước khi xóa khách hàng
                        var cancelledBookings = db.DatPhongs.Where(dp => dp.MaKH == maKHChon && dp.TrangThai == "Đã hủy");
                        db.DatPhongs.RemoveRange(cancelledBookings);

                        db.KhachHangs.Remove(kh);
                        db.SaveChanges();

                        MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnLamMoi_Click(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            maKHChon = -1;
            txtHoTen.Clear();
            txtCCCD.Clear();
            txtSDT.Clear();

            cboTrangThai.SelectedIndexChanged -= cboTrangThai_SelectedIndexChanged;
            cboTrangThai.SelectedIndex = 0;
            cboTrangThai.SelectedIndexChanged += cboTrangThai_SelectedIndexChanged;

            LoadDataKhachHang();
            CapNhatThongKe();
        }

        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKhachHang.Rows[e.RowIndex];
                if (row.Cells["MaKH"].Value != null)
                {
                    maKHChon = Convert.ToInt32(row.Cells["MaKH"].Value);
                    txtHoTen.Text = row.Cells["HoTen"].Value?.ToString();
                    txtCCCD.Text = row.Cells["CCCD"].Value?.ToString();
                    txtSDT.Text = row.Cells["SDT"].Value?.ToString();
                }
            }
        }

        private void dgvKhachHang_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvKhachHang.Columns[e.ColumnIndex].Name == "btnLichSu")
            {
                var cellMaKH = dgvKhachHang.Rows[e.RowIndex].Cells["MaKH"].Value;
                if (cellMaKH != null && int.TryParse(cellMaKH.ToString(), out int maKH))
                {
                    frmXemLichSu frm = new frmXemLichSu(maKH);
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();
                }
            }
        }

        // ================= CẬP NHẬT THỐNG KÊ =================
        private void CapNhatThongKe()
        {
            DateTime today = DateTime.Today;
            DateTime now = DateTime.Now;

            // 1. Số lượng khách Đang lưu trú
            int dangLuuTruCount = db.DatPhongs
                .Where(dp => dp.TrangThai != "Đã hủy" &&
                             (dp.TrangThai == "Đang thuê" || dp.TrangThai == "Đang ở" || (dp.NgayNhan <= now && now <= dp.NgayTra)))
                .Select(dp => dp.MaKH)
                .Distinct()
                .Count();
            lblDangLuuTru.Text = dangLuuTruCount.ToString("N0");

            // 2. Số lượng khách Trực tuyến (Có tài khoản + Đang lưu trú hoặc Chưa nhận phòng)
            int trucTuyenCount = db.KhachHangs
                .Where(k => !string.IsNullOrEmpty(k.TenDangNhap) && !k.TenDangNhap.StartsWith("khach_"))
                .Where(k => db.DatPhongs.Any(dp => dp.MaKH == k.MaKH && dp.TrangThai != "Đã hủy" &&
                                               ((dp.TrangThai == "Đang thuê" || dp.TrangThai == "Đang ở" || (dp.NgayNhan <= now && now <= dp.NgayTra)) || dp.NgayNhan > now)))
                .Count();
            lblKhachTrucTuyen.Text = trucTuyenCount.ToString("N0");

            // 3. Khách mới hôm nay
            int homNayCount = db.DatPhongs
                .Where(dp => dp.NgayDat.Date == today)
                .Select(dp => dp.MaKH)
                .Distinct()
                .Count();
            lblKhachMoi.Text = homNayCount.ToString("N0");

            // 4. Tổng số khách hàng
            lblTongKhach.Text = db.KhachHangs.Count().ToString("N0");
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            frmDatPhong frm = new frmDatPhong(nhanVienDangNhap);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();

            LoadDataKhachHang();
            CapNhatThongKe();
        }
    }
}