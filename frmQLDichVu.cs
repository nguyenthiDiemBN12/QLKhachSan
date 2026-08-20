using Microsoft.EntityFrameworkCore;
using PhanMemQuanLyKhachSan.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PMQuanLyKhachSan
{
    public partial class frmQLDichVu : Form
    {
        private readonly DataContext db = new DataContext();
        private NhanVien nhanVienDangNhap;
        public frmQLDichVu(NhanVien nv)
        {
            InitializeComponent();
            this.nhanVienDangNhap = nv;
        }

        private void frmQLDichVu_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            txtMaDV.ReadOnly = true;
          

            SetupDataGridViewStyle();
            LoadDataDichVu();
        }

        #region Tối Ưu Giao Diện & Helper

        // 🎨 Cấu hình phong cách sang trọng cho DataGridView
        private void SetupDataGridViewStyle()
        {
            if (dgvDichVu == null) return;

            dgvDichVu.ReadOnly = true;
            dgvDichVu.AllowUserToAddRows = false;
            dgvDichVu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDichVu.MultiSelect = false;
            dgvDichVu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Tắt visual styles mặc định để đổi màu Header
            dgvDichVu.EnableHeadersVisualStyles = false;

            // Style thanh Tiêu đề (Header)
            dgvDichVu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 50, 55); // Xám đậm kim loại
            dgvDichVu.ColumnHeadersDefaultCellStyle.ForeColor = Color.Gold;               // Chữ vàng sang trọng
            dgvDichVu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvDichVu.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDichVu.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvDichVu.ColumnHeadersHeight = 35;

            // Đường viền và lưới
            dgvDichVu.AdvancedColumnHeadersBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
            dgvDichVu.GridColor = Color.Gray;

            // Style dòng dữ liệu so le
            dgvDichVu.RowsDefaultCellStyle.BackColor = Color.White;
            dgvDichVu.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvDichVu.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvDichVu.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;

            // Màu khi chọn dòng (Highlight)
            dgvDichVu.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 122, 204);
            dgvDichVu.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // 🏷️ Đặt tên hiển thị và định dạng cột
        private void FormatGridViewColumns()
        {
            if (dgvDichVu == null) return;

            if (dgvDichVu.Columns["MaDV"] != null)
            {
                dgvDichVu.Columns["MaDV"].HeaderText = "Mã Dịch Vụ";
                dgvDichVu.Columns["MaDV"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvDichVu.Columns["TenDV"] != null)
            {
                dgvDichVu.Columns["TenDV"].HeaderText = "Tên Dịch Vụ";
            }

            if (dgvDichVu.Columns["Gia"] != null)
            {
                dgvDichVu.Columns["Gia"].HeaderText = "Giá Dịch Vụ (VNĐ)";
                dgvDichVu.Columns["Gia"].DefaultCellStyle.Format = "#,##0"; // Format tiền tệ dễ nhìn
                dgvDichVu.Columns["Gia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        // 🧹 Hàm làm mới ô nhập liệu
        private void LamMoiForm()
        {
            txtMaDV.Clear();
            txtTenDV.Clear();
            txtGiaDV.Clear();
            txtTenDV.Focus();
        }

        // 🗑️ Giải phóng kết nối CSDL khi đóng Form
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            db.Dispose();
            base.OnFormClosed(e);
        }

        #endregion

        #region Tải Dữ Liệu & Sự Kiện GridView

        // ⚡ Nạp danh sách dịch vụ lên DataGridView
        private void LoadDataDichVu()
        {
            try
            {
                var listDV = db.DichVus
                    .AsNoTracking() // 💡 Tối ưu tốc độ đọc dữ liệu (Không track entity)
                    .Select(dv => new
                    {
                        dv.MaDV,
                        dv.TenDV,
                        dv.Gia
                    })
                    .ToList();

                if (dgvDichVu != null)
                {
                    dgvDichVu.DataSource = listDV;
                    FormatGridViewColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🖱️ Chọn dòng trên DataGridView đổ dữ liệu lên TextBox
        private void dgvDichVu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDichVu != null)
            {
                DataGridViewRow row = dgvDichVu.Rows[e.RowIndex];

                txtMaDV.Text = row.Cells["MaDV"].Value?.ToString();
                txtTenDV.Text = row.Cells["TenDV"].Value?.ToString();

                if (row.Cells["Gia"].Value != null)
                {
                    decimal gia = Convert.ToDecimal(row.Cells["Gia"].Value);
                    txtGiaDV.Text = gia.ToString("#,##0"); // Hiển thị số có dấu phẩy phân cách
                }
            }
        }

        // 💸 Tự động định dạng dấu phẩy khi người dùng gõ phím vào txtGiaDV
        private void txtGiaDV_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGiaDV.Text)) return;

            txtGiaDV.TextChanged -= txtGiaDV_TextChanged;

            int selectionStart = txtGiaDV.SelectionStart;
            int lengthBefore = txtGiaDV.Text.Length;

            string rawValue = txtGiaDV.Text.Replace(",", "").Trim();
            if (decimal.TryParse(rawValue, out decimal number))
            {
                txtGiaDV.Text = number.ToString("#,##0");
                int lengthAfter = txtGiaDV.Text.Length;
                txtGiaDV.SelectionStart = Math.Max(0, selectionStart + (lengthAfter - lengthBefore));
            }

            txtGiaDV.TextChanged += txtGiaDV_TextChanged;
        }

        #endregion

        #region Xử Lý Các Nút Chức Năng (CRUD)

        // ➕ NÚT THÊM
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string tenDV = txtTenDV.Text.Trim();
                string giaRaw = txtGiaDV.Text.Replace(",", "").Trim(); // Bỏ dấu phẩy để parse

                if (string.IsNullOrWhiteSpace(tenDV))
                {
                    MessageBox.Show("Vui lòng nhập Tên dịch vụ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTenDV.Focus();
                    return;
                }

                if (!decimal.TryParse(giaRaw, out decimal gia) || gia < 0)
                {
                    MessageBox.Show("Giá dịch vụ phải là số hợp lệ (>= 0)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGiaDV.Focus();
                    return;
                }

                if (db.DichVus.Any(d => d.TenDV.ToLower() == tenDV.ToLower()))
                {
                    MessageBox.Show("Tên dịch vụ này đã tồn tại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dvMoi = new DichVu
                {
                    TenDV = tenDV,
                    Gia = gia
                };

                db.DichVus.Add(dvMoi);
                db.SaveChanges();

                MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataDichVu();
                LamMoiForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thêm dịch vụ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✏️ NÚT SỬA
        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMaDV.Text, out int maDV))
                {
                    MessageBox.Show("Vui lòng chọn dịch vụ cần sửa từ danh sách!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string tenDV = txtTenDV.Text.Trim();
                string giaRaw = txtGiaDV.Text.Replace(",", "").Trim();

                if (string.IsNullOrWhiteSpace(tenDV))
                {
                    MessageBox.Show("Tên dịch vụ không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(giaRaw, out decimal gia) || gia < 0)
                {
                    MessageBox.Show("Giá dịch vụ không hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (db.DichVus.Any(d => d.TenDV.ToLower() == tenDV.ToLower() && d.MaDV != maDV))
                {
                    MessageBox.Show("Tên dịch vụ này trùng với dịch vụ khác!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dvSua = db.DichVus.Find(maDV);
                if (dvSua != null)
                {
                    dvSua.TenDV = tenDV;
                    dvSua.Gia = gia;

                    db.SaveChanges();
                    MessageBox.Show("Cập nhật dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataDichVu();
                    LamMoiForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi sửa dịch vụ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ❌ NÚT XÓA
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMaDV.Text, out int maDV))
                {
                    MessageBox.Show("Vui lòng chọn dịch vụ muốn xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmResult = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa dịch vụ '{txtTenDV.Text}' không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmResult == DialogResult.Yes)
                {
                    var dvXoa = db.DichVus.Find(maDV);
                    if (dvXoa != null)
                    {
                        db.DichVus.Remove(dvXoa);
                        db.SaveChanges();

                        MessageBox.Show("Xóa dịch vụ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataDichVu();
                        LamMoiForm();
                    }
                }
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Không thể xóa dịch vụ này vì đã có chi tiết sử dụng dịch vụ trong dữ liệu khách hàng!",
                                "Lỗi ràng buộc hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xóa dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🔄 NÚT LÀM MỚI
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoiForm();
            LoadDataDichVu();
            if (dgvDichVu != null) dgvDichVu.ClearSelection();
        }

        #endregion
    }
}