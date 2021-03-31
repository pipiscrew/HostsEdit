using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostsEdit
{
    public partial class AddHost : Form
    {
        public AddHost()
        {
            InitializeComponent();
        }

        public DialogResult ShowDialog(out string result)
        {
            DialogResult dialogResult = base.ShowDialog();

            result = Regex.Replace(txtHost.Text, @"https|http|(?!\.|-)\W+", "");

            return dialogResult;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            txtHost.Text = txtHost.Text.Trim();

            if (txtHost.Text.Length == 0)
                return;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
