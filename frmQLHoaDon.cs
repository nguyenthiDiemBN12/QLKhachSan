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
    public partial class frmQLHoaDon : Form
    {
        public DataContext db = new DataContext();
        public frmQLHoaDon()
        {
            InitializeComponent();
        }

        private void frmQLHoaDon_Load(object sender, EventArgs e)
        {

        }
    }
}
