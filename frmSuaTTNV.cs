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
    public partial class frmSuaTTNV : Form
    {
        public DataContext db = new DataContext();
        public frmSuaTTNV()
        {
            InitializeComponent();
        }

        private void frmSuaTTNV_Load(object sender, EventArgs e)
        {

        }
    }
}
