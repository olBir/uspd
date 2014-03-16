using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableUtils;

namespace HelpUtils
{
    public partial class RefreshData : Form
    {
        FillTable f = new FillTable();
        public RefreshData()
        {
            InitializeComponent();
        }

        private void refreshDataButton_Click(object sender, EventArgs e)
        {
                f.changeValuesInDb();
        }


    }
}
